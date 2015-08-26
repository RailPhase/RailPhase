using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Web2Sharp.Templates
{
    public abstract partial class Template
    {
        public static TemplateRenderer FromFile(string filename)
        {
            Type t;
            return LoadFile(filename, out t);
        }

        static long TemplateNameCounter = 0;
        static Dictionary<string, TemplateRenderer> TemplateRendererFileCache = new Dictionary<string, TemplateRenderer>();
        static Dictionary<string, Type> TemplateTypeFileCache = new Dictionary<string, Type>();

        static TemplateRenderer LoadFile(string filename, out Type templateType)
        {
            var fileInfo = new FileInfo(filename);

            // Use the absolute file path to make sure files are only cached once even if different relative paths are used
            var normalizedFilename = fileInfo.FullName;

            if (TemplateRendererFileCache.ContainsKey(normalizedFilename))
            {
                templateType = TemplateTypeFileCache[normalizedFilename];
                return TemplateRendererFileCache[normalizedFilename];
            }
            else
            {
                if (!File.Exists(normalizedFilename))
                {
                    throw new System.IO.FileNotFoundException(filename);
                }

                TemplateRenderer templateRenderer = CompileTemplateFile(normalizedFilename, out templateType);
                TemplateRendererFileCache[normalizedFilename] = templateRenderer;
                return templateRenderer;
            }
        }

        static TemplateRenderer CompileTemplateFile(string filename, out Type templateType)
        {
            TemplateNameCounter++;
            string templateName = "Template" + TemplateNameCounter.ToString();

            string templateText = File.ReadAllText(filename);

            Type type = CompileTemplateString(templateText, templateName);
            TemplateTypeFileCache[filename] = type;
            TemplateRenderer renderDelegate = (TemplateRenderer)type.GetMethod("Render").CreateDelegate(typeof(TemplateRenderer));

            templateType = type;
            return renderDelegate;
        }
    }
}
