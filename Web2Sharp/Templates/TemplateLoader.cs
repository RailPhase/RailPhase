using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Web2Sharp.Templates
{
    public static class TemplateLoader
    {
        static long TemplateNameCounter = 0;
        public static Template FromFile(string filename)
        {
            TemplateNameCounter++;
            string templateName = "Template" + TemplateNameCounter.ToString();

            string templateText = File.ReadAllText(filename);

            string csharp = TemplateParser.CompileToCSharp(templateText, templateName);

            Template renderDelegate = TemplateCompiler.CompileCsharpTemplate(csharp, templateName);

            return renderDelegate;
        }
    }
}
