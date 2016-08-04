using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RailPhase
{
    /// <summary>
    /// Represents a single HTTP request. Provides information about the request and methods to respond to it.
    /// </summary>
    public class Context
    {
        /// <summary>
        /// The underlying HttpListenerContext.
        /// </summary>
        protected HttpListenerContext HttpContext;

        /// <summary>
        /// The <see cref="UrlPatternMatch"/> that matched this request. May be null if the request was not (yet) matched to a pattern.
        /// </summary>
        public UrlPatternMatch PatternMatch { get; protected set; }

        /// <summary>
        /// Contains additional objects that can be attached to this context.
        /// </summary>
        /// <remarks>
        /// Can be used for various purposes. For example, when <see cref="App.CatchViewExceptions"/> is true and an exception occurs, the exception is attached to this context and passed to <see cref="App.InternalErrorView"/>.
        /// </remarks>
        protected Dictionary<Type, object> Tags = new Dictionary<Type, object>();

        /// <summary>
        /// Attach a tag object to this context.
        /// </summary>
        /// <remarks>
        /// Can be used for various purposes. For example, when <see cref="App.CatchViewExceptions"/> is true and an exception occurs, the exception is attached to this context and passed to <see cref="App.InternalErrorView"/>.
        /// </remarks>
        /// <seealso cref="AddTag"/>
        public void AddTag<T>(T tag) where T : class
        {
            Tags[typeof(T)] = tag;
        }

        /// <summary>
        /// Get an object that was attached to this context with <see cref="AddTag"/>
        /// </summary>
        /// <seealso cref="GetTag"/>
        public T GetTag<T>() where T : class
        {
            object tagObject;
            Tags.TryGetValue(typeof(T), out tagObject);
            T tag = tagObject as T;
            return tag;
        }

        /// <summary>
        /// Contains information about the request.
        /// </summary>
        public HttpListenerRequest Request { get { return HttpContext.Request; } }

        /// <summary>
        /// Contains information about the response to be sent.
        /// </summary>
        public HttpListenerResponse Response { get { return HttpContext.Response; } }

        /// <summary>
        /// The stream of the response body.
        /// </summary>
        /// <remarks>
        /// If you write to this stream, you can no longer change HTTP headers of the response.
        /// </remarks>
        /// <seealso cref="WriteResponse"/>
        public Stream ResponseStream { get { return Response.OutputStream; } }

        /// <summary>
        /// Write something to the response body.
        /// </summary>
        /// <remarks>
        /// If you want to write binary data, directly write to the <see cref="ResponseStream"/>. If you write to this stream, you can no longer change HTTP headers of the response.
        /// </remarks>
        /// <seealso cref="ResponseStream"/>
        public void WriteResponse(string response)
        {
            using (var writer = new StreamWriter(ResponseStream, Response.ContentEncoding))
            {
                writer.Write(response);
            }
        }

        /// <summary>
        /// Create a new context. You usually do not need to do this.
        /// </summary>
        public Context(HttpListenerContext httpContext, UrlPatternMatch patternMatch = null)
        {
            HttpContext = httpContext;
            PatternMatch = patternMatch;
        }
    }
}
