using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            switch(httpMethod)
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
            foreach(var param in getParams)
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
                foreach(var cookie in cookies)
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
        public IDictionary<string,byte[]> ServerParameters { get { return FcgiRequest.Parameters; } }

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

    /// <summary>
    /// Base class for HTTP responses. Use <see cref="HttpResponse"/> if you want to create a simple HTTP response.
    /// </summary>
    /// <seealso cref="HttpResponse"/>
    public class RawHttpResponse
    {
        /// <summary>
        /// Creates a new raw http response, without any headers pre-set.
        /// </summary>
        /// <param name="body">(Optional) The raw HTTP body, including any headers. You can omit this parameter and set the <see cref="RawBody"/> later.</param>
        public RawHttpResponse(byte[] body = null)
        {
            RawBody = body;
        }

        /// <summary>
        /// The raw body of the HTTP response, including all headers. Can be null.
        /// </summary>
        public byte[] RawBody;

        /// <summary>
        /// Writes this response to the output stream of the given System.Net.HttpListenerContext
        /// </summary>
        /// <remarks>
        /// Used internally by <see cref="App.RunTestServer(string)"/>.
        /// </remarks>
        public void WriteToHttpListenerContext(HttpListenerContext context)
        {
            var responseStream = new MemoryStream(RawBody);
            
            // Read the HTTP headers from the raw body
            bool bodyReached = false;
            bool firstLine = true;
            while (!bodyReached)
            {
                var line = new StringBuilder();
                var lineEndReached = false;
                while (!lineEndReached)
                {
                    var b = responseStream.ReadByte();
                    if (b < 0)
                    {
                        bodyReached = true;
                        break;
                    }
                    else if ((char)b == '\n')
                        lineEndReached = true;
                    else if ((char)b == '\r')
                        continue; // Ignore \r characters
                    else
                        line.Append((char)b);
                }

                // Stop if we reached an empty line; this marks the start of the body
                if (String.IsNullOrWhiteSpace(line.ToString()))
                    bodyReached = true;
                else
                {
                    // First line contains HTTP response status line
                    if (firstLine)
                    {
                        var statusElements = line.ToString().Split(new char[] { ' ' }, 3);

                        if (statusElements.Length != 3 || statusElements[0] != "HTTP/1.1")
                            throw new InvalidDataException("Invalid HTTP Response: Incorrect status line found");

                        var statusCode = statusElements[1];
                        var statusDescription = statusElements[2];

                        context.Response.StatusCode = Int32.Parse(statusCode);
                        context.Response.StatusDescription = statusDescription;

                        firstLine = false;
                    }
                    else
                    {
                        // Add each header to the listenercontext response
                        var headerElements = line.ToString().Split(new char[] { ':' }, 2);
                        context.Response.AddHeader(headerElements[0].Trim(), headerElements[1].Trim());
                    }
                }
            }

            // And finally, write the body to the output stream.
            context.Response.OutputStream.Write(RawBody, (int)responseStream.Position, RawBody.Length - (int)responseStream.Position);
            context.Response.OutputStream.Close();
        }
    }

    /// <summary>
    /// Represents a HTTP response.
    /// </summary>
    /// <remarks>
    /// If you need full control over the raw response content, use <see cref="RawHttpResponse"/> instead.
    /// </remarks>
    public class HttpResponse: RawHttpResponse
    {
        /// <summary>
        /// Creates a HTTP response, encoded with UTF-8, with the most important headers already set.
        /// </summary>
        /// <remarks>
        /// If you need full control over the raw response content, use <see cref="RawHttpResponse"/> instead.
        /// </remarks>
        /// <param name="body">The content of the response, not including any headers.</param>
        /// <param name="status">(Optional) The HTTP status code. Default is "200 OK".</param>
        /// <param name="contentType">(Optional) The HTTP content-type. Default is "text/html". Additionally, "charset=utf-8" will be appended to the HTTP content-type header.</param>
        /// <param name="additionalHeaders">(Optional) Any additional headers, in raw HTTP format. These must NOT end with a newline.</param>
        public HttpResponse(string body, string status="200 OK", string contentType="text/html", string additionalHeaders="", IEnumerable<Cookie> cookies = null)
        {
            var responseText = new StringBuilder();
            responseText.AppendLine("HTTP/1.1 " + status);
            responseText.AppendLine("Content-Type: " + contentType + "; charset=utf-8");

            // Write the cookie headers
            if(cookies != null)
            {
                foreach(var cookie in cookies)
                {
                    responseText.Append("Set-Cookie:" + cookie.Name + "=" + cookie.Value);
                    if (cookie.Expires != null)
                    {
                        responseText.Append(";Expires=" + cookie.Expires.ToRFC822String());
                    }
                    if (cookie.Domain != null)
                    {
                        responseText.Append(";Domain=" + cookie.Domain);
                    }
                    if (cookie.Path != null)
                    {
                        responseText.Append(";Path=" + cookie.Path);
                    }
                    responseText.AppendLine();
                }
            }

            // Write additional headers
            if(!String.IsNullOrEmpty(additionalHeaders))
                responseText.AppendLine(additionalHeaders);

            // Empty line marks the start of the body
            responseText.AppendLine("");
            responseText.Append(body);

            RawBody = Encoding.UTF8.GetBytes(responseText.ToString());
        }
    }

    public class RedirectResponse: HttpResponse
    {
        public RedirectResponse(string location, bool permanentRedirect = false):
            base(
                "",
                status: (permanentRedirect ? "301 Moved Permanently" : "307 Temporary Redirect"),
                additionalHeaders: "Location:"+location
            )
        {}
    }

    public class Cookie
    {
        public string Name;
        public string Value;
        public DateTime Expires;
        public string Domain;
        public string Path;
    }

}
