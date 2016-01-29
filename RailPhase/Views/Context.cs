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
    public class Context
    {
        protected HttpListenerContext HttpContext;

        public UrlPatternMatch PatternMatch { get; protected set; }

        public HttpListenerRequest Request { get { return HttpContext.Request; } }

        public HttpListenerResponse Response { get { return HttpContext.Response; } }

        public Stream ResponseStream { get { return Response.OutputStream; } }
        
        public void WriteResponse(string response)
        {
            using (var writer = new StreamWriter(ResponseStream, Response.ContentEncoding))
            {
                writer.Write(response);
            }   
        }

        public Context(HttpListenerContext httpContext, UrlPatternMatch patternMatch = null)
        {
            HttpContext = httpContext;
            PatternMatch = patternMatch;
        }
    }
}
