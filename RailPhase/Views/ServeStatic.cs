using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RailPhase
{
    class ServeStatic
    {
        public static Dictionary<string, string> ContentTypesByExtension = new Dictionary<string, string>()
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

        public static void ServeStaticFiles(Context context, string urlPrefix, string rootDirectory)
        {
            string root = Path.GetFullPath(rootDirectory);
            string relativeFilePath = Utils.MakeRelativePath(context.Request.Url.AbsolutePath, urlPrefix);
            string filePath = Path.GetFullPath(Path.Combine(rootDirectory, relativeFilePath));
            
            // No ../ allowed! Make sure only files inside the rootDirectory are server.
            if(!filePath.StartsWith(root) || !File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                context.WriteResponse("<h1>404 Not Found</h1>");
            }

            // Try to find the content type for this file extension
            var extension = Path.GetExtension(filePath).ToLower();
            extension = extension.Replace(".", "");
            string contentType = null;
            ContentTypesByExtension.TryGetValue(extension, out contentType);

            // If we found a content type for this file, add it to the HTTP response, otherwise omit it.
            if (contentType != null)
                context.Response.ContentType = contentType;
            
            // Copy the file contents to the response stream
            File.OpenRead(filePath).CopyTo(context.ResponseStream);
        }
    }
}
