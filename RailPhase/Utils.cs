using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

namespace RailPhase
{
    public static class Utils
    {
        public static Assembly AssemblyByName(string name)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                if (assembly.GetName().Name == name)
                {
                    return assembly;
                }
            }

            return null;
        }

        // Copypasted from http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String MakeRelativePath(String toPath, String fromPath)
        {
            Uri fromUri = new Uri("file://" + fromPath);
            Uri toUri = new Uri("file://" + toPath);

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath;
        }

        /// <summary>
        /// Converts a system path into unix format. Performs no action when called on a unix system.
        /// </summary>
        /// <param name="path">A path, as returned by some System.IO.* method</param>
        /// <returns>Returns the given path, with the directory separator ('\' on Windows) replaced by '/'.</returns>
        public static string SystemToUnixPath(string path)
        {
            if (Path.DirectorySeparatorChar == '/')
            {
                return path;
            }
            else
            {
                return path.Replace(Path.DirectorySeparatorChar, '/');
            }
        }
        
        public static string StripHtmlTags(string html)
        {
            var text = Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();
            text = Regex.Replace(text, @"\s{2,}", " ");
            return text;
        }

        public static string ToRFC822String(this DateTime timestamp)
        {
            return timestamp.ToString("ddd',' d MMM yyyy HH':'mm':'ss", CultureInfo.InvariantCulture);
        }

        public static string ToHumanDate(this DateTime timestamp)
        {
            return timestamp.ToString("dddd',' d MMMM yyyy");
        }
    }
}
