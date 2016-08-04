using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RailPhase
{
    /// <summary>
    /// Provides a configurable <see cref="View"/> that serves static files from the filesystem.
    /// </summary>
    public class ServeStatic
    {
        private static Dictionary<string, string> ContentTypesByExtension = new Dictionary<string, string>()
        {
            {"txt", "text/plain"},
            {"htm", "text/html"},
            {"html", "text/html"},
            {"css", "text/css"},
            {"js", "application/javascript"},
            {"json", "application/json"},
            {"jpg", "image/jpeg"},
            {"jpeg", "image/jpeg"},
            {"png", "image/png"},
            {"gif", "image/gif"},
            {"tif", "image/tiff"},
            {"tiff", "image/tiff"},
            {"bmp", "image/bmp"},
            {"svg", "image/svg+xml"},
            {"swf", "application/x-shockwave-flash"},
            {"flv", "video/x-flv"},
            {"pdf", "application/pdf"},
        };

        /// <summary>
        /// Serves a static file from the filesystem.
        /// </summary>
        /// <remarks>
        /// If you want to server static files, you can use <see cref="App.AddStaticDirectory"/>, which encapsulates this method as a <see cref="View"/>.
        /// </remarks>
        /// <param name="context">The current request context of the file to be served.</param>
        /// <param name="urlPrefix">The part of the request URL that corresponds to the local filesystem folder. The request URL has to start with this string. The part after it will be treated as the filename to be served.</param>
        /// <param name="rootDirectory">The root directory of the served files.</param>
        public static void ServeStaticFiles(Context context, string urlPrefix, string rootDirectory)
        {
            string root = Path.GetFullPath(rootDirectory);
            string relativeFilePath = Utils.MakeRelativePath(context.Request.Url.AbsolutePath, urlPrefix);
            string filePath = Path.GetFullPath(Path.Combine(rootDirectory, relativeFilePath));

            // No ../ allowed! Make sure only files inside the rootDirectory are server.
            if (!filePath.StartsWith(root) || !File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                context.WriteResponse("<h1>404 Not Found</h1>");
            }
            else
            {
                // Try to find the content type for this file extension
                var extension = Path.GetExtension(filePath).ToLower();
                extension = extension.Replace(".", "");
                string contentType = null;
                ContentTypesByExtension.TryGetValue(extension, out contentType);

                // If we found a content type for this file, add it to the HTTP response, otherwise omit it.
                if (contentType != null)
                    context.Response.ContentType = contentType;

                // Copy the file contents to the response stream
                using (var stream = File.OpenRead(filePath))
                {
                    stream.CopyTo(context.ResponseStream);
                }
            }
        }
    }
}
