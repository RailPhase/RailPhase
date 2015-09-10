using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Web2Sharp
{
    class ServeStatic
    {
        public static Dictionary<string, string> ContentTypesByExtension = new Dictionary<string, string>()
        {
            ["txt"]  = "text/plain",
            ["htm"]  = "text/html",
            ["html"] = "text/html",
            ["css"]  = "text/css",
            ["js"]   = "application/javascript",
            ["json"] = "application/json",
            ["jpg"]  = "image/jpeg",
            ["jpeg"] = "image/jpeg",
            ["png"]  = "image/png",
            ["gif"]  = "image/gif",
            ["tif"]  = "image/tiff",
            ["tiff"] = "image/tiff",
            ["bmp"]  = "image/bmp",
            ["svg"]  = "image/svg+xml",
            ["swf"]  = "application/x-shockwave-flash",
            ["flv"]  = "video/x-flv",
            ["pdf"]  = "application/pdf",
        }; 

        public static RawHttpResponse ServeStaticFiles(HttpRequest request, string urlPrefix, string rootDirectory)
        {
            string root = Path.GetFullPath(rootDirectory);
            string relativeFilePath = Utils.MakeRelativePath(request.GetParameterASCII("DOCUMENT_URI"), urlPrefix);
            string filePath = Path.GetFullPath(Path.Combine(rootDirectory, relativeFilePath));
            
            // No ../ allowed! Make sure only files inside the rootDirectory are server.
            if(!filePath.StartsWith(root) || !File.Exists(filePath))
            {
                return new HttpResponse("<h1>404 Not Found</h1>", status: "404 Not Found");
            }

            // Try to find the content type for this file extension
            var extension = Path.GetExtension(filePath).ToLower();
            extension = extension.Replace(".", "");
            string contentType = null;
            ContentTypesByExtension.TryGetValue(extension, out contentType);

            var rawData = File.ReadAllBytes(filePath);

            var head = "";

            // If we found a content type for this file, add it to the HTTP response, otherwise omit it.
            if (contentType != null)
                head = "Status: 200 OK\nContent-Type: " + contentType + "\n\n";
            else
                head = "Status: 200 OK\n\n";

            byte[] rawHead = Encoding.ASCII.GetBytes(head);

            // Concatenate rawHead and rawData into body
            byte[] body = new byte[rawHead.Length + rawData.Length];
            Buffer.BlockCopy(rawHead, 0, body, 0, rawHead.Length);
            Buffer.BlockCopy(rawData, 0, body, rawHead.Length, rawData.Length);

            return new RawHttpResponse(body);
        }
    }
}
