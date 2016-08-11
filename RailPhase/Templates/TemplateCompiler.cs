using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Diagnostics;
using System.IO;
using RailPhase.Templates.Parser;

namespace RailPhase
{
    /// <summary>
    /// Represents a syntax error in a template.
    /// </summary>
    /// <seealso cref="Template.FromString(string)"/>
    /// <seealso cref="Template.FromFile(string)"/>
    public class TemplateParserException: Exception
    {
        /// <summary>
        /// Create a new TemplateParserException. Used internally.
        /// </summary>
        public TemplateParserException(string message):
            base(message)
        {

        }
    }
    
    public abstract partial class Template
    {
        internal static Type CompileCsharpTemplate(string csharp, string name, List<string> assemblyReferences)
        {
            var assemblyPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            // Compile the given code and load it as an assembly without writing to a file.
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = false;
            parameters.CompilerOptions = "/optimize";
            parameters.OutputAssembly = name;

            parameters.ReferencedAssemblies.AddRange(assemblyReferences.ToArray());

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, csharp);

            // Report any errors. Unlikely to throw because the given code should be auto-generated
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error in line {0} ({1}): {2}", error.Line, error.ErrorNumber, error.ErrorText));
                }

                throw new TemplateParserException(sb.ToString());
            }
            
            Assembly assembly = results.CompiledAssembly;
            Type templateClass = assembly.GetType("RailPhase.TemplateCache."+name);

            return templateClass;
        }

        static string BuildBlock(string name, string content, Type contextType, IEnumerable<string> localBlocks, string templateName, List<string> assemblyReferences)
        {
            var s = new StringBuilder();

            s.AppendLine("public static string "+name+ "(object contextObj, Dictionary<string,BlockRenderer> blockRenderers) {");

            // override local blocks by setting the renderers to the local methods
            foreach (var block in localBlocks)
            {
                s.AppendLine("if(!blockRenderers.ContainsKey(\"" + block + "\")) blockRenderers[\"" + block + "\"] = Block_" + block + ";");
            }

            if(contextType != null)
            {
                s.AppendLine("var context = (" + contextType.FullName + ")contextObj;");

                // Import all public fields and properties so that they are available locally
                foreach (var field in contextType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!field.IsStatic)
                    {
                        s.AppendLine("var " + field.Name + " = context." + field.Name + ";");
                        if(!assemblyReferences.Contains(field.FieldType.Assembly.Location))
                        {
                            assemblyReferences.Add(field.FieldType.Assembly.Location);
                        }
                    }
                }
                foreach (var property in contextType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (property.CanRead)
                        s.AppendLine("var " + property.Name + " = context." + property.Name + ";");
                    if (!assemblyReferences.Contains(property.PropertyType.Assembly.Location))
                    {
                        assemblyReferences.Add(property.PropertyType.Assembly.Location);
                    }
                }
            }
            else
            {
                s.AppendLine("var context = contextObj;");
            }

            s.AppendLine("StringBuilder output = new StringBuilder();");

            s.AppendLine(content);

            s.AppendLine("return output.ToString();");
            s.AppendLine("} // "+name+"()");

            return s.ToString();
        }

        internal static Type ParseTemplateString(string template, string name)
        {
            var parser = Parser.FromText(template);
            var success = parser.Parse();

            if(!success)
            {
                throw new TemplateParserException("Syntax Error!");
            }
            
            Type contextType = null;

            if (parser.ResultContextType != null)
            {
                contextType = Type.GetType(parser.ResultContextType);
                if (contextType == null)
                    throw new TemplateParserException("Unable to resolve context type '" + parser.ResultContextType + "'");
            }

            var assemblyReferences = new List<string>();
            assemblyReferences.Add(Utils.AssemblyByName("System").Location);
            assemblyReferences.Add(Utils.AssemblyByName("System.Core").Location);
            assemblyReferences.Add(Utils.AssemblyByName("RailPhase").Location);


            if (contextType != null)
                assemblyReferences.Add(contextType.Assembly.Location);

            Type extendsType = null;
            if(parser.ResultExtends != null)
            {
                // Compile and load the extended template if it is not already in the cache
                LoadFile(parser.ResultExtends, out extendsType);
                assemblyReferences.Add(extendsType.Assembly.GetName().Name);
            }

            var s = new StringBuilder();
            s.AppendLine("// Automagically generated code by RailPhase.Templates.TemplateParser");
            s.AppendLine("using System;");
            s.AppendLine("using System.Text;");
            s.AppendLine("using System.Collections.Generic;");
            s.AppendLine("using System.Linq;");

            s.AppendLine("using RailPhase;");

            // Add custom usings from template
            foreach (var ns in parser.ResultUsings)
            {
                var assembly = Utils.AssemblyByName(ns);
                if (assembly != null)
                {
                    assemblyReferences.Add(assembly.Location);
                    s.AppendLine("using " + ns + ";");
                }
                else if (ns.Contains(","))
                {
                    var usingElements = ns.Split(',');
                    
                    if(usingElements.Length == 2)
                    {
                        var namespaceName = usingElements[0].Trim();
                        var assemblyName = usingElements[1].Trim();

                        assemblyReferences.Add(Utils.AssemblyByName(assemblyName).Location);
                        s.AppendLine("using " + namespaceName + ";");
                    }
                }
                else
                {
                    s.AppendLine("using " + ns + ";");
                }
            }

            s.AppendLine("namespace RailPhase.TemplateCache {");

            s.AppendLine("public class " + name + " {");

            // Implement blocks as callable functions
            foreach (var block in parser.ResultBlocks)
            {
                s.Append(BuildBlock("Block_" + block.Key, block.Value, contextType, parser.ResultBlocks.Keys, name, assemblyReferences));
            }

            if (extendsType == null)
            {
                s.Append(BuildBlock("Block_Root", parser.ResultText, contextType, parser.ResultBlocks.Keys, name, assemblyReferences));
            }
            else
            {
                // Extending Templates don't use their own root block, but call the one of the extended template, with the local blocks as overrides.
                string extendingContent = "return " + extendsType.FullName + ".Block_Root(context, blockRenderers);";
                s.Append(BuildBlock("Block_Root", extendingContent, contextType, parser.ResultBlocks.Keys, name, assemblyReferences));
            }

            s.AppendLine("public static string Render(object context) { return Block_Root(context, new Dictionary<string,BlockRenderer>()); }");

            s.AppendLine("} // class");
            s.AppendLine("} // namespace");

            return CompileCsharpTemplate(s.ToString(), name, assemblyReferences);
        }
    }
}
