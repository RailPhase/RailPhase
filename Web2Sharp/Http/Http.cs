using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web2Sharp
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
    public class HttpRequest
    {
        /// <summary>
        /// Creates a new request object.
        /// </summary>
        /// <param name="fcgiRequest"></param>
        public HttpRequest(FastCGI.Request fcgiRequest)
        {
            FcgiRequest = fcgiRequest;
            string httpMethod = fcgiRequest.Parameters["REQUEST_METHOD"];
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
            var getParams = ServerParameters["QUERY_STRING"].Split('&');
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
                        GET[key] = value;
                    }
                }
            }
        }

        /// <summary>
        /// The underlying FastCGI request. Contains some more detailed information.
        /// </summary>
        public readonly FastCGI.Request FcgiRequest;

        /// <summary>
        /// A dictionary of all HTTP parameters included in the request
        /// </summary>
        public IDictionary<string,string> ServerParameters { get { return FcgiRequest.Parameters; } }

        /// <summary>
        /// A dictionary of all GET parameters included in the request.
        /// </summary>
        public Dictionary<string,string> GET { get; private set; }

        /// <summary>
        /// The URI of this request
        /// </summary>
        public string Uri
        {
            get
            {
                return FcgiRequest.Parameters["REQUEST_URI"];
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
        /// <param name="body">(Optional) The raw HTTP body, including any headers. You can omit this parameter and set the <see cref="Body"/> later.</param>
        public RawHttpResponse(string body="")
        {
            Body = body;
        }

        /// <summary>
        /// The raw body of the HTTP response, including all headers.
        /// </summary>
        public string Body;
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
        /// Creates a HTTP response, with the most important headers already set.
        /// </summary>
        /// <remarks>
        /// If you need full control over the raw response content, use <see cref="RawHttpResponse"/> instead.
        /// </remarks>
        /// <param name="body">The content of the response, not including any headers.</param>
        /// <param name="status">(Optional) The HTTP status code. Default is "200 OK".</param>
        /// <param name="contentType">(Optional) The HTTP content-type. Default is "text/html".</param>
        /// <param name="additionalHeaders">(Optional) Any additional headers, in raw HTTP format.</param>
        public HttpResponse(string body, string status="200 OK", string contentType="text/html", string additionalHeaders="")
        {
            Body = "Status: "+status+"\n";
            Body += "Content-Type: "+contentType+"\n";
            Body += "\n";
            Body += body;
        }
    }

}
