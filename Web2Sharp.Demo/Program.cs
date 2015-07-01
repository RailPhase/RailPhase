using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web2Sharp.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a simple web app and serve content on to URLs.
            var app = new App();

            // Define the URL patterns
            app.AddUrlPattern("^/$", (request) => new HttpResponse("<h1>Hello World</h1>"));
            app.AddUrlPattern("^/info$", InfoView);

            // Start accepting requests from the web server.
            // This method never returns!
            app.Run();
        }

        /// <summary>
        /// A view that generates a simple request info page
        /// </summary>
        static HttpResponse InfoView(Web2Sharp.HttpRequest request)
        {
            // This is of course not the recommended way to generate HTML content,
            // but it is the simplest way for this example.

            string body = "<html><head>\n";
            body += @"<style>
                        table {
                            border-collapse: collapse;
                        }

                        table, th, td {
                            border: 1px solid black;
                        }
                     </style>";

            body += "</head><body>\n";
            body += "<h1>Request Info Page</h1>\n";

            // Print all server parameters into a table
            body += "<h2>Server parameters</h2>\n";
            body += "<table><tr><th>Name</th><th>Value</th>\n";
            foreach (var param in request.ServerParameters)
            {
                body += string.Format("<tr><td>{0}</td><td>{1}</td></tr>\n", param.Key, param.Value);
            }
            body += "</table>\n";
            body += "</body></html>\n";

            // Return a simple Http response.
            // We could also return non-HTML content or error codes here
            // by setting the parameters in the HttpResponse constructor.
            return new HttpResponse(body);
        }
    }
}
