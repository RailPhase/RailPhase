using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RailPhase.Templates;

namespace RailPhase.Demo
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

            // Start accepting requests from the web server on port 19000.
            // This method never returns!
            app.Run(19000);
        }

        /// <summary>
        /// A view that generates a simple request info page
        /// </summary>
        static HttpResponse InfoView(RailPhase.HttpRequest request)
        {
            // Get the template for the info page.
            var render = Templates.Template.FromFile("InfoTemplate.html");

            // Pass the HttpRequest as the template context, because we want
            // to display information about the request. Normally, we would
            // pass some custom object here, containing the information we
            // want to display.
            var body = render(request);

            // Return a simple Http response.
            // We could also return non-HTML content or error codes here
            // by setting the parameters in the HttpResponse constructor.
            return new HttpResponse(body);
        }
    }

    public class DemoContext
    {
        public string Heading;
        public string Username;
    }
}
