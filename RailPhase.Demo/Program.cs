using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailPhase.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a simple web app and serve content on to URLs.
            var app = new App();

            // Define the URL patterns to respond to incoming requests.
            // Any request that does not match one of these patterns will
            // be served by app.NotFoundView, which defaults to a simple
            // 404 message.

            // Easiest way to respond to a request: return a string
            app.AddStringView("^/$", (request) => "Hello World");
            // More complex response, see below
            app.AddStringView("^/info$", InfoView);

            // Start listening for HTTP requests. Default port is 8080.
            // This method does never return!
            app.RunHttpServer("http://localhost:18080/");

            // Now you should be able to visit me in your browser on http://localhost:18080/
        }

        /// <summary>
        /// A view that generates a simple request info page
        /// </summary>
        static string InfoView(RailPhase.Context context)
        {
            // Get the template for the info page.
            var render = Template.FromFile("InfoTemplate.html");

            // Pass the HttpRequest as the template context, because we want
            // to display information about the request. Normally, we would
            // pass some custom object here, containing the information we
            // want to display.
			return render(null, context);
        }
    }

    public class DemoData
    {
        public string Heading;
        public string Username;
    }
}
