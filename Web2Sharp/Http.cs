using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web2Sharp
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE,
        UNKNOWN
    }

    public class HttpRequest
    {
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

        public readonly FastCGI.Request FcgiRequest;

        public IDictionary<string,string> ServerParameters { get { return FcgiRequest.Parameters; } }

        public Dictionary<string,string> GET { get; private set; }

        public string Uri
        {
            get
            {
                return FcgiRequest.Parameters["REQUEST_URI"];
            }
        }

        public string Body
        {
            get
            {
                return FcgiRequest.Body;
            }
        }

        public HttpMethod Method { get; private set; }
    }

    public class RawHttpResponse
    {
        public RawHttpResponse(string body)
        {
            Body = body;
        }
        public string Body;
    }

    public class HttpResponse: RawHttpResponse
    {
        public HttpResponse(string body, string status="200 OK", string contentType="text/html", string additionalHeaders=""):
            base("")
        {
            Body = "Status: "+status+"\n";
            Body += "Content-Type: "+contentType+"\n";
            Body += "\n";
            Body += body;
        }
    }

}
