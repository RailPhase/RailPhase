using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;

namespace RailPhase
{
    /// <summary>
    /// Specifies a Http Method.
    /// </summary>
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE,
        UNKNOWN
    }

    /// <summary>
    /// Represents an incoming HTTP request.
    /// </summary>
    /// <importance>5</importance>
    public class HttpRequest
    {
        /// <summary>
        /// Creates a new request object.
        /// </summary>
        /// <param name="fcgiRequest"></param>
        public HttpRequest(FastCGI.Request fcgiRequest)
        {
            FcgiRequest = fcgiRequest;
            string httpMethod = fcgiRequest.GetParameterASCII("REQUEST_METHOD");
            switch (httpMethod)
            {
                case "GET":
                    Method = HttpMethod.GET;
                    break;
                case "POST":
                    Method = HttpMethod.POST;
                    break;
                case "PUT":
                    Method = HttpMethod.PUT;
                    break;
                case "DELETE":
                    Method = HttpMethod.DELETE;
                    break;
                default:
                    Method = HttpMethod.UNKNOWN;
                    break;
            }

            GET = new Dictionary<string, string>();
            // Todo: perform correct parsing and decoding according to HTTP standard
            var getParams = Encoding.ASCII.GetString(ServerParameters["QUERY_STRING"]).Split('&');
            foreach (var param in getParams)
            {
                if (param.Length > 0)
                {
                    var keyValue = param.Split(new char[] { '=' }, 2);
                    if (keyValue.Length >= 1)
                    {
                        var key = keyValue[0];
                        var value = "";
                        if (keyValue.Length >= 2)
                            value = keyValue[1];
                        GET[key] = System.Uri.UnescapeDataString(value.Replace('+', ' '));
                    }
                }
            }

            POST = new Dictionary<string, string>();
            // Todo: perform correct parsing and decoding according to HTTP standard
            var postParams = Body.Split('&');
            foreach (var param in postParams)
            {
                if (param.Length > 0)
                {
                    var keyValue = param.Split(new char[] { '=' }, 2);
                    if (keyValue.Length >= 1)
                    {
                        var key = keyValue[0];
                        var value = "";
                        if (keyValue.Length >= 2)
                            value = keyValue[1];
                        POST[key] = System.Uri.UnescapeDataString(value.Replace('+', ' '));
                    }
                }
            }

            if (ServerParameters.ContainsKey("HTTP_COOKIE"))
            {
                var cookieHeader = GetParameterASCII("HTTP_COOKIE");
                var cookies = cookieHeader.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var cookie in cookies)
                {
                    var cookieNameValue = cookie.Split('=');
                    Cookies[cookieNameValue[0].Trim()] = System.Uri.UnescapeDataString(cookieNameValue[1].Trim().Replace('+', ' '));
                }
            }

        }

        /// <summary>
        /// The underlying FastCGI request. Contains some more detailed information.
        /// </summary>
        public readonly FastCGI.Request FcgiRequest;

        /// <summary>
        /// Contains information about the URL pattern that was matched to this request.
        /// </summary>
        /// <remarks>
        /// If the URL pattern contained regex groups, you can use this like a dictionary to access the matched URL substrings.
        /// See <see cref="UrlPatternMatch"/> for details.
        /// </remarks>
        public UrlPatternMatch PatternMatch;

        /// <summary>
        /// A dictionary of all HTTP parameters included in the request
        /// </summary>
        public IDictionary<string, byte[]> ServerParameters { get { return FcgiRequest.Parameters; } }

        public string GetParameterASCII(string name)
        {
            return Encoding.ASCII.GetString(ServerParameters[name]);
        }

        public string GetParameterUTF8(string name)
        {
            return Encoding.UTF8.GetString(ServerParameters[name]);
        }

        /// <summary>
        /// A dictionary of all GET parameters included in the request.
        /// </summary>
        public Dictionary<string, string> GET { get; private set; }
        public Dictionary<string, string> POST { get; private set; }

        public Dictionary<string, string> Cookies = new Dictionary<string, string>();

        /// <summary>
        /// The URI of this request
        /// </summary>
        public string Uri
        {
            get
            {
                return GetParameterASCII("REQUEST_URI");
            }
        }

        /// <summary>
        /// The HTTP body of the request.
        /// </summary>
        public string Body
        {
            get
            {
                return FcgiRequest.Body;
            }
        }

        /// <summary>
        /// The HTTP method of the request.
        /// </summary>
        public HttpMethod Method { get; private set; }

        static int FakeRequestId = 128000;
        /// <summary>
        /// Creates an HttpRequest from the given System.Net.HttpListenerContext.
        /// </summary>
        /// <remarks>
        /// Used internally by <see cref="App.RunTestServer(string)"/>.
        /// </remarks>
        public static HttpRequest FromHttpListenerContext(HttpListenerContext context)
        {
            var fakeRequest = new FastCGI.Request(FakeRequestId++, new MemoryStream());

            fakeRequest.Parameters.Add("REQUEST_METHOD", Encoding.ASCII.GetBytes(context.Request.HttpMethod));
            fakeRequest.Parameters.Add("QUERY_STRING", Encoding.ASCII.GetBytes(context.Request.RawUrl));
            fakeRequest.Parameters.Add("DOCUMENT_URI", Encoding.ASCII.GetBytes(context.Request.RawUrl));
            fakeRequest.Parameters.Add("REQUEST_URI", Encoding.ASCII.GetBytes(context.Request.RawUrl));

            return new HttpRequest(fakeRequest);
        }
    }

}
