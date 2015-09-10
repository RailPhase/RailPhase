﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Web2Sharp.Templates;

namespace Web2Sharp
{
    /// <summary>
    /// A view handles a request and returns a response.
    /// </summary>
    /// <remarks>
    /// Views are the main 
    /// </remarks>
    /// <param name="request">The HTTP request to handle.</param>
    /// <returns>Returns a RawHttpResponse object (usually HttpResponse).</returns>
    public delegate RawHttpResponse View(HttpRequest request);

    /// <summary>
    /// The main class for Web2Sharp web applications.
    /// </summary>
    /// <importance>10</importance>
    public class App
    {
        List<UrlPattern> urlPatterns = new List<UrlPattern>();
        List<Module> activeModules = new List<Module>();

        /// <summary>
        /// Adds a new URL pattern.
        /// </summary>
        /// <remarks>
        /// When the App receives a request with a URL that matches the given pattern, the specified view will be called.
        /// Please note that there are overloads of this method with more convenient signatures, like <see cref="AddUrlPattern(string, View)"/>.
        /// </remarks>
        /// <param name="pattern">The URL pattern to add.</param>
        /// <seealso cref="Web2Sharp.UrlPattern"/>
        /// <seealso cref="Web2Sharp.View"/>
        public void AddUrlPattern(UrlPattern pattern) { urlPatterns.Add(pattern); }

        /// <summary>
        /// Adds a new URL pattern.
        /// </summary>
        /// <remarks>
        /// When the App receives a request with a URL that matches the given pattern, the specified view will be called.
        /// </remarks>
        /// <param name="pattern">A string in .NET Regex Syntax, specifying the URL pattern.</param>
        /// <param name="view">The View that should be called when </param>
        /// <seealso cref="Web2Sharp.UrlPattern"/>
        /// <seealso cref="Web2Sharp.View"/>
        public void AddUrlPattern(string pattern, View view) { urlPatterns.Add(new UrlPattern(pattern, view)); }

        /// <summary>
        /// Adds a new URL pattern that responds to requests with a template.
        /// </summary>
        /// <param name="pattern">A string in .NET Regex Syntax, specifying the URL pattern.</param>
        /// <param name="template">The TemplateRenderer that is used to render the response.</param>
        /// <param name="contentType">The optional HTTP content-type. Default is "text/html".</param>
        public void AddUrlPattern(string pattern, TemplateRenderer template, string contentType = "text/html")
        {
            AddUrlPattern(pattern, (request) =>
            {
                return new HttpResponse(template(null), contentType: contentType);
            });
        }

        /// <summary>
        /// Adds a new URL pattern that responds to requests with a template.
        /// </summary>
        /// <param name="pattern">A string in .NET Regex Syntax, specifying the URL pattern.</param>
        /// <param name="template">The path to the template file that is used to render the response.</param>
        /// <param name="contentType">The optional HTTP content-type. Default is "text/html".</param>
        public void AddUrlPattern(string pattern, string templateFile, string contentType = "text/html")
        {
            TemplateRenderer template = Template.FromFile(templateFile);
            AddUrlPattern(pattern, template, contentType);
        }

        public void AddUrl(string url, View view)
        {
            AddUrlPattern("^" + Regex.Escape(url) + "$", view);
        }

        public void AddUrl(string url, TemplateRenderer template, string contentType = "text/html")
        {
            AddUrl(url, (request) =>
            {
                return new HttpResponse(template(null), contentType: contentType);
            });
        }

        public void AddUrl(string url, string templateFile, string contentType = "text/html")
        {
            TemplateRenderer template = Template.FromFile(templateFile);
            AddUrl(url, template, contentType);
        }

        public View NotFoundView = (request) =>
        {
            return new HttpResponse("<h1>404 Not Found</h1>", contentType: "text/html", status: "404 NOT FOUND");
        };

        /// <summary>
        /// Registers an URL pattern with a view that serves the local static files in the given directory.
        /// </summary>
        /// <param name="url">The root url prefix to serve the files on. Must end with a slash.</param>
        /// <param name="rootDirectory">The root directory where the served files are located. Must end with a slash.</param>
        public void AddStaticDirectory(string rootDirectory, string url = "/static/")
        {
            View view = (HttpRequest request) =>
            {
                return ServeStatic.ServeStaticFiles(request, url, rootDirectory);
            };

            var urlPattern = "^" + Regex.Escape(url) + ".*$";

            AddUrlPattern(urlPattern, view);
		}
		
        public void AddModule(Module module)
        {
            foreach(var urlPattern in module.UrlPatterns)
            {
                AddUrlPattern(urlPattern);
            }

            activeModules.Add(module);
        }

        /// <summary>
        /// Handles an incoming HTTP request. You usually do not need to call this.
        /// </summary>
        /// <remarks>
        /// When called, this method will go through the registered URL patterns and pass the request to the view of the pattern that matches first.
        /// If not URL pattern matches the URL of the request, a 404 page is returned.
        /// </remarks>
        /// <param name="request">The HTTPRequest to handle</param>
        /// <returns>Returns a HTTPResponse, generated by one of the registered URL patterns, or a 404 response if no pattern matches.</returns>
        public RawHttpResponse HandleRequest(HttpRequest request)
        {
            foreach(var urlPattern in urlPatterns)
            {
                string path = request.GetParameterASCII("DOCUMENT_URI");
                if (urlPattern.Pattern.IsMatch(path))
                {
                    request.PatternMatch = new UrlPatternMatch
                    {
                        Pattern = urlPattern,
                        Match = urlPattern.Pattern.Match(path)
                    };

                    // Todo: Catch errors
                    return urlPattern.View(request);
                }
            }

            return NotFoundView(request);
        }

        /// <summary>
        /// Handles an incoming FastCGI request. You usually do not need to call this.
        /// </summary>
        public void ReceiveFcgiRequest(object sender, FastCGI.Request fcgiRequest)
        {
            var httpRequest = new HttpRequest(fcgiRequest);
            var response = HandleRequest(httpRequest);
            if(response.RawBody != null)
                fcgiRequest.WriteResponse(response.RawBody);
            fcgiRequest.Close();
        }

        /// <summary>
        /// Starts listening as a FastCGI client. This method never returns! 
        /// </summary>
        /// <remarks>This method starts the FastCGI client and will respond to any requests that are received over FastCGI. Any URL patterns have to be registered before calling this, because this method never returns.</remarks>
        /// <param name="port">The port for the FastCGI client.</param>
        public void Run(int port=19000)
        {
            var fcgiApp = new FastCGI.FCGIApplication();
            fcgiApp.OnRequestReceived += ReceiveFcgiRequest;
            fcgiApp.Run(port);
        }

    }
}
