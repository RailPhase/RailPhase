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
    public static class TemplateCompiler
    {
        public static Template CompileCsharpTemplate(string csharp, string name)
        {
            // Compile the given code and load it as an assembly without writing to a file.
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateInMemory = false;
            CompilerResults results = provider.CompileAssemblyFromSource(parameters, csharp);

            // Report any errors. Unlikely to throw because the given code should be auto-generated
            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }

                throw new InvalidOperationException(sb.ToString());
            }
            
            Assembly assembly = results.CompiledAssembly;
            Type templateClass = assembly.GetType("Web2Sharp.TemplateCache."+name);
            MethodInfo renderMethod = templateClass.GetMethod("Render");

            return (Template)renderMethod.CreateDelegate(typeof(Template));
        }
    }
}
