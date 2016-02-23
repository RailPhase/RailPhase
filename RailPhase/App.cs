using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web;

namespace RailPhase
{
    /// <summary>
    /// Handles a HTTP request and returns a response.
    /// </summary>
    /// <param name="context">A <see cref="Context"/> object that contains information about the incoming request.</param>
    /// <returns>Returns the response as a string. If the string is not null, it is written to the responsestream. Otherwise, the return value is ignored.</returns>
    public delegate string StringView(Context context);

    /// <summary>
    /// Handles a HTTP request.
    /// </summary>
    public delegate void View(Context context);

    /// <summary>
    /// The main class for RailPhase web applications.
    /// </summary>
    /// <importance>10</importance>
    public class App
    {
        List<UrlPattern> urlPatterns = new List<UrlPattern>();
        List<Module> activeModules = new List<Module>();

        public bool EnableAsyncProcessing { get; set; } = false;

        public int MaxParallelRequests { get; set; } = Environment.ProcessorCount;

        public Encoding DefaultResponseEncoding { get; set; } = Encoding.UTF8;
        public string DefaultContentType { get; set; } = "text/html";

        public bool IsRunning { get; set; } = false;

        public void Stop() { IsRunning = false; }

        Queue<Task> OpenRequests = new Queue<Task>();

        public void ConnectDatabase(string connection = "Database")
        {
            Database.InitializeDatabase(connection);
        }

        public static View StringToVoidView(StringView view)
        {
            return (Context context) =>
            {
                var response = view(context);
                if (response != null)
                    context.WriteResponse(response);
            };
        }

        /// <summary>
        /// Adds a new URL pattern.
        /// </summary>
        /// <remarks>
        /// When the App receives a request with a URL that matches the given pattern, the specified view will be called.
        /// Please note that there are overloads of this method with more convenient signatures, like <see cref="AddUrlPattern(string, View)"/>.
        /// </remarks>
        /// <param name="pattern">The URL pattern to add.</param>
        /// <seealso cref="RailPhase.UrlPattern"/>
        /// <seealso cref="RailPhase.View"/>
        public void AddUrlPattern(UrlPattern pattern) { urlPatterns.Add(pattern); }

        /// <summary>
        /// Adds a new URL pattern.
        /// </summary>
        /// <remarks>
        /// When the App receives a request with a URL that matches the given regex pattern, the specified view will be called.
        /// </remarks>
        /// <param name="pattern">A string in .NET Regex Syntax, specifying the URL pattern.</param>
        /// <param name="view">The View that should be called when </param>
        /// <seealso cref="RailPhase.UrlPattern"/>
        /// <seealso cref="RailPhase.View"/>
        public void AddUrlPattern(string pattern, View view) { urlPatterns.Add(new UrlPattern(pattern, view)); }

        public void AddUrlPattern(string pattern, StringView view) { urlPatterns.Add(new UrlPattern(pattern, StringToVoidView(view))); }
        
        /// <summary>
        /// Adds a new URL pattern with a regex pattern that responds to requests with a template.
        /// </summary>
        /// <param name="pattern">A string in .NET Regex Syntax, specifying the URL pattern.</param>
        /// <param name="template">The path to the template file that is used to render the response.</param>
        /// <param name="contentType">The optional HTTP content-type. Default is "text/html".</param>
        public void AddUrlPattern(string pattern, string templateFile, string contentType = "text/html")
        {
            TemplateRenderer template = Template.FromFile(templateFile);
            AddUrlPattern(pattern, StringToVoidView((context) =>
            {
                context.Response.ContentType = contentType;
                return template(null);
            }));
        }

        /// <summary>
        /// Adds a new URL pattern with a static URL.
        /// </summary>
        public void AddUrl(string url, View view)
        {
            if (!url.StartsWith("/"))
                url = "/" + url;

            AddUrlPattern("^" + Regex.Escape(url) + "$", view);
        }

        /// <summary>
        /// Adds a new URL pattern with a static URL.
        /// </summary>
        public void AddUrl(string url, StringView view)
        {
            if (!url.StartsWith("/"))
                url = "/" + url;

            AddUrlPattern("^" + Regex.Escape(url) + "$", StringToVoidView(view));
        }

        /// <summary>
        /// Adds a new URL pattern with a static URL. The URL pattern responds to request with a the given template.
        /// </summary>
        public void AddUrl(string url, string templateFile, string contentType = "text/html")
        {
            if (!url.StartsWith("/"))
                url = "/" + url;

            AddUrlPattern("^" + url + "$", templateFile, contentType);
        }

        /// <summary>
        /// The view that should be called when a request does not match any of the URL patterns.
        /// </summary>
        public View NotFoundView = StringToVoidView((context) =>
        {
            context.Response.StatusCode = 404;
            return "<h1>404 Not Found</h1>";
        });

        /// <summary>
        /// Registers an URL pattern with a view that serves the local static files in the given directory.
        /// </summary>
        /// <param name="url">The root url prefix to serve the files on. Must end with a slash.</param>
        /// <param name="rootDirectory">The root directory where the served files are located. Must end with a slash.</param>
        public void AddStaticDirectory(string rootDirectory, string url = "/static/")
        {
            if (!url.StartsWith("/"))
                url = "/" + url;
            if (!url.EndsWith("/"))
                url = url + "/";

            AddUrlPattern("^" + url + ".*$", (Context context) =>
            {
                ServeStatic.ServeStaticFiles(context, url, rootDirectory);
            });
        }

        /// <summary>
        /// Registers the URL patterns of the given Module in this app.
        /// </summary>
        public void AddModule(Module module)
        {
            foreach (var urlPattern in module.UrlPatterns)
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
        public void HandleRequest(HttpListenerContext httpContext)
        {
            var serveTimer = new Stopwatch();
            serveTimer.Start();

            bool foundPatternMatch = false;

            Context context = null;

            string path = httpContext.Request.Url.AbsolutePath;

            foreach (var urlPattern in urlPatterns)
            {
                if (urlPattern.Pattern.IsMatch(path))
                {
                    foundPatternMatch = true;

                    var patternMatch = new UrlPatternMatch
                    {
                        Pattern = urlPattern,
                        Match = urlPattern.Pattern.Match(path)
                    };

                    context = new Context(httpContext, patternMatch);

                    // Todo: Catch errors
                    urlPattern.View(context);

                    break;
                }
            }

            if (!foundPatternMatch)
            {
                //Todo: Log 404
                context = new Context(httpContext, null);
                NotFoundView(context);
            }

            // Save database changes after each request
            Database.SaveAllModelChanges();

            serveTimer.Stop();
        }

        protected void ApplyDefaultSettings(HttpListenerContext httpContext)
        {
            httpContext.Response.ContentEncoding = DefaultResponseEncoding;
            httpContext.Response.ContentType = DefaultContentType;
        }
        
        /// <summary>
        /// Starts a HTTP server that serves incoming requests. This method blocks until <see cref="Stop"/> is called.
        /// </summary>
        /// <remarks>
        /// The web server will accept HTTP requests from the given prefix (default is "http://localhost:8080").
        /// </remarks>
        public void RunHttpServer(string prefix = "http://localhost:8080/")
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(prefix);
                listener.Start();

                IsRunning = true;

                Task<HttpListenerContext> listenerTask = null;

                while (IsRunning)
                {
                    if (listenerTask == null)
                    {
                        listenerTask = listener.GetContextAsync();
                    }

                    if (listenerTask.IsCompleted)
                    {
                        var httpContext = listenerTask.Result;
                        listenerTask = null;

                        ApplyDefaultSettings(httpContext);

                        var requestTask = new Task(() =>
                        {
                            HandleRequest(httpContext);
                            httpContext.Response.OutputStream.Close();
                        });

                        requestTask.Start();

                        if (EnableAsyncProcessing)
                        {
                            OpenRequests.Enqueue(requestTask);
                            while (OpenRequests.Count > MaxParallelRequests && OpenRequests.Count > 0)
                            {
                                var oldestTask = OpenRequests.Dequeue();
                                oldestTask.Wait();
                            }
                        }
                        else
                        {
                            requestTask.Wait();
                        }
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                
                listener.Stop();
            }
        }

    }
}
