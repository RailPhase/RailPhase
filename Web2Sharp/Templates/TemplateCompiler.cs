using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Web2Sharp.Templates
{
    /// <summary>
    /// Represents a syntax error in a template.
    /// </summary>
    /// <seealso cref="Template.FromString(string)"/>
    /// <seealso cref="Template.FromFile(string)"/>
    public class TemplateParserException: Exception
    {
        public TemplateParserException(string message):
            base(message)
        {

        }
    }

    public abstract partial class Template
    {
        internal static Type CompileCsharpTemplate(string csharp, string name, List<string> assemblyReferences)
        {
            // Compile the given code and load it as an assembly without writing to a file.
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = false;
            parameters.CompilerOptions = "/optimize";

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
            Type templateClass = assembly.GetType("Web2Sharp.TemplateCache."+name);

            return templateClass;
        }

        static string BuildBlock(string name, string content, Type contextType, IEnumerable<string> localBlocks, string templateName)
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
                foreach (var field in contextType.GetFields())
                {
                    if(!field.IsStatic)
                        s.AppendLine("var " + field.Name + " = context." + field.Name + ";");
                }
                foreach (var property in contextType.GetProperties())
                {
                    if (property.CanRead)
                        s.AppendLine("var " + property.Name + " = context." + property.Name + ";");
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
            var parser = Parser.Parser.FromText(template);
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
            assemblyReferences.Add("Web2Sharp.dll");

            if(contextType != null)
                assemblyReferences.Add(contextType.Assembly.Location);

            Type extendsType = null;
            if(parser.ResultExtends != null)
            {
                // Compile and load the extended template if it is not already in the cache
                LoadFile(parser.ResultExtends, out extendsType);
                assemblyReferences.Add(extendsType.Assembly.Location);
            }

            var s = new StringBuilder();
            s.AppendLine("// Automagically generated code by Web2Sharp.Templates.TemplateParser");
            s.AppendLine("using System;");
            s.AppendLine("using System.Text;");
            s.AppendLine("using System.Collections.Generic;");
            s.AppendLine("using Web2Sharp.Templates;");

            // Add custom usings from template
            foreach (var ns in parser.ResultUsings)
            {
                var assembly = Utils.AssemblyByName(ns);
                if (assembly != null)
                    assemblyReferences.Add(assembly.Location);
                s.AppendLine("using " + ns + ";");
            }

            s.AppendLine("namespace Web2Sharp.TemplateCache {");

            s.AppendLine("public class " + name + " {");

            // Implement blocks as callable functions
            foreach (var block in parser.ResultBlocks)
            {
                s.Append(BuildBlock( "Block_" + block.Key, block.Value, contextType, parser.ResultBlocks.Keys, name));
            }

            if (extendsType == null)
            {
                s.Append(BuildBlock("Block_Root", parser.ResultText, contextType, parser.ResultBlocks.Keys, name));
            }
            else
            {
                // Extending Templates don't use their own root block, but call the one of the extended template, with the local blocks as overrides.
                string extendingContent = "return " + extendsType.FullName + ".Block_Root(context, blockRenderers);";
                s.Append(BuildBlock("Block_Root", extendingContent, contextType, parser.ResultBlocks.Keys, name));
            }

            s.AppendLine("public static string Render(object context) { return Block_Root(context, new Dictionary<string,BlockRenderer>()); }");



            s.AppendLine("} // class");
            s.AppendLine("} // namespace");

            return CompileCsharpTemplate(s.ToString(), name, assemblyReferences);
        }
    }
}
