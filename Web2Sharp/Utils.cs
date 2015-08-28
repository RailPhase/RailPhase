using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Web2Sharp
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
        /// <param name="absPath">Absolute path.</param>
        /// <param name="relTo">Directory that defines the start of the relative path.</param> 
        /// <returns>The relative path from the start directory to the end path.</returns>
        public static string MakeRelativePath(string absPath, string relTo)
        {
            string[] absParts = absPath.Split(Path.DirectorySeparatorChar);
            string[] relParts = relTo.Split(Path.DirectorySeparatorChar);

            // Get the shortest of the two paths
            int len = absParts.Length < relParts.Length
                ? absParts.Length : relParts.Length;

            // Use to determine where in the loop we exited
            int lastCommonRoot = -1;
            int index;

            // Find common root
            for (index = 0; index < len; index++)
            {
                if (absParts[index].Equals(relParts[index], StringComparison.OrdinalIgnoreCase))
                    lastCommonRoot = index;
                else
                    break;
            }

            // If we didn't find a common prefix then throw
            if (lastCommonRoot == -1)
                throw new ArgumentException("The path of the two files doesn't have any common base.");

            // Build up the relative path
            var relativePath = new StringBuilder();

            // Add on the ..
            for (index = lastCommonRoot + 1; index < relParts.Length; index++)
            {
                relativePath.Append("..");
                relativePath.Append(Path.DirectorySeparatorChar);
            }

            // Add on the folders
            for (index = lastCommonRoot + 1; index < absParts.Length - 1; index++)
            {
                relativePath.Append(absParts[index]);
                relativePath.Append(Path.DirectorySeparatorChar);
            }
            relativePath.Append(absParts[absParts.Length - 1]);

            return relativePath.ToString();
        }

        public static string MakeUnixPath(string path)
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

        public static string ToRFC822String(this DateTime timestamp)
        {
            return timestamp.ToString("ddd',' d MMM yyyy HH':'mm':'ss")
                + " "
                + timestamp.ToString("zzzz").Replace(":", "");
        }

        public static string ToHumanDate(this DateTime timestamp)
        {
            return timestamp.ToString("dddd',' d MMMM yyyy");
        }

        public static string StripHtmlTags(string html)
        {
            var text = Regex.Replace(html, @"<[^>]+>|&nbsp;", "").Trim();
            text = Regex.Replace(text, @"\s{2,}", " ");
            return text;
        }
    }
}
