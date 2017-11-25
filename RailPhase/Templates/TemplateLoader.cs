using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace RailPhase
{
    /// <summary>
    /// Provides functions to work with templates.
    /// </summary>
    public abstract partial class Template
    {
        /// <summary>
        /// This is the prefix of all template names.
        /// </summary>
        static readonly string TemplateNamePrefix = "RailPhase_TemplateRendererCache_";

        /// <summary>
        /// The directory path where all template cache files are located.
        /// </summary>
        public static readonly string TemplateCachePath;

        static Template()
        {
            // Remove any old template cache files
            var railPhaseAssembly = typeof(Template).Assembly;
            var assemblyUri = new UriBuilder(railPhaseAssembly.CodeBase);
            var assemblyLocation = Uri.UnescapeDataString(assemblyUri.Path);
            var TemplateCachePath = Path.GetDirectoryName(assemblyLocation);
            var localDir = new DirectoryInfo(TemplateCachePath);
            var oldCacheFiles = localDir.EnumerateFiles(TemplateNamePrefix + "*");
            foreach(var oldCacheFile in oldCacheFiles)
            {
                oldCacheFile.Delete();
            }
        }

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
            return CompileTemplateString(text, out t, TemplateName());
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

                var name = TemplateName(normalizedFilename);

                TemplateRenderer templateRenderer = CompileTemplateString(templateText, out type, name);
                TemplateRendererFileCache[normalizedFilename] = templateRenderer;
                TemplateTypeFileCache[normalizedFilename] = type;
                templateType = type;
                return templateRenderer;
            }
        }

        static Regex templateNameDisallowedCaracters = new Regex("[^A-Za-z_0-9]");

        private static string TemplateName(string filePath = "")
        {
            TemplateNameCounter++;
            var fileName = Path.GetFileName(filePath);
            // Remove any characters that are not allowed
            var cleanName = templateNameDisallowedCaracters.Replace(fileName, "_");

            // The resulting name is the common template name prefix plus the cleaned file name plus a unique number
            return TemplateNamePrefix + cleanName + TemplateNameCounter.ToString();
        }

        static TemplateRenderer CompileTemplateString(string templateText, out Type templateType, string name)
        {
            Type type = ParseTemplateString(templateText, name);
            TemplateRenderer renderDelegate = (TemplateRenderer)type.GetMethod("Render").CreateDelegate(typeof(TemplateRenderer));

            templateType = type;
            return renderDelegate;
        }
    }
}
