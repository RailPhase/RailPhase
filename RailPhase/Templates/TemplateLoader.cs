using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RailPhase.Templates
{
    /// <summary>
    /// Provides functions to work with templates.
    /// </summary>
    public abstract partial class Template
    {
        /// <summary>
        /// Loads a <see cref="TemplateRenderer"/> from a file.
        /// </summary>
        public static TemplateRenderer FromFile(string filename)
        {
            Type t;
            return LoadFile(filename, out t);
        }

        /// <summary>
        /// Loads a <see cref="TemplateRenderer"/> from a string.
        /// </summary>
        public static TemplateRenderer FromString(string text)
        {
            Type t;
            return CompileTemplateString(text, out t);
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

                string templateText = File.ReadAllText(normalizedFilename);
                Type type;
                TemplateRenderer templateRenderer = CompileTemplateString(templateText, out type);
                TemplateRendererFileCache[normalizedFilename] = templateRenderer;
                TemplateTypeFileCache[normalizedFilename] = type;
                templateType = type;
                return templateRenderer;
            }
        }

        static TemplateRenderer CompileTemplateString(string templateText, out Type templateType)
        {
            TemplateNameCounter++;
            string templateName = "Template" + TemplateNameCounter.ToString();

            Type type = ParseTemplateString(templateText, templateName);
            TemplateRenderer renderDelegate = (TemplateRenderer)type.GetMethod("Render").CreateDelegate(typeof(TemplateRenderer));

            templateType = type;
            return renderDelegate;
        }
    }
}
