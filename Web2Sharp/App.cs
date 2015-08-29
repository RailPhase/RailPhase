using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Web2Sharp.Templates;

namespace Web2Sharp
{

    public delegate RawHttpResponse View(HttpRequest request);

    public class App
    {
        List<UrlPattern> urlPatterns = new List<UrlPattern>();

        public void AddUrlPattern(UrlPattern pattern) { urlPatterns.Add(pattern); }
        public void AddUrlPattern(string pattern, View view) { urlPatterns.Add(new UrlPattern(pattern, view)); }
        public void AddUrlPattern(string pattern, TemplateRenderer template, string contentType = "text/html")
        {
            AddUrlPattern(pattern, (request) =>
            {
                return new HttpResponse(template(null), contentType: contentType);
            });
        }

        public void AddUrlPattern(string pattern, string templateFile, string contentType = "text/html")
        {
            TemplateRenderer template = Template.FromFile(templateFile);
            AddUrlPattern(pattern, template, contentType);
        }

        public RawHttpResponse HandleRequest(HttpRequest request)
        {
            foreach(var urlPattern in urlPatterns)
            {
                string path = request.ServerParameters["DOCUMENT_URI"];
                if (urlPattern.Pattern.IsMatch(path))
                    // Todo: Catch errors
                    return urlPattern.View(request);
            }

            // Todo: Return error
            return new HttpResponse("<h1>Page not found</h1>", status:"404 NOT FOUND");
        }

        public void ReceiveFcgiRequest(object sender, FastCGI.Request fcgiRequest)
        {
            var httpRequest = new HttpRequest(fcgiRequest);
            var response = HandleRequest(httpRequest);
            fcgiRequest.WriteResponseUtf8(response.Body);
            fcgiRequest.Close();
        }

        public void Run(int port=19000)
        {
            var fcgiApp = new FastCGI.FCGIApplication();
            fcgiApp.OnRequestReceived += ReceiveFcgiRequest;
            fcgiApp.Run(port);
        }
    }
}
