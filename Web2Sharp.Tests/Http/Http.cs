using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Web2Sharp;

namespace Web2Sharp.Tests.Http
{
    [TestFixture]
    class Http
    {
        App app;
        Random rand;

        public string GetResponseFromUrl(string url)
        {
            var fcgiParams = new Dictionary<string, string>
            {
                ["QUERY_STRING"] = "",
                ["REQUEST_METHOD"] = "GET",
                ["CONTENT_TYPE"] = "",
                ["CONTENT_LENGTH"] = "",
                ["SCRIPT_NAME"] = url,
                ["REQUEST_URI"] = url,
                ["DOCUMENT_URI"] = url,
                ["SERVER_PROTOCOL"] = "HTTP/1.1",
                ["REQUEST_SCHEME"] = "http",
                ["GATEWAY_INTERFACE"] = "CGI/1.1",
                ["SERVER_SOFTWARE"] = "nginx/1.9.2",
                ["REMOTE_ADDR"] = "127.0.0.1",
                ["REMOTE_PORT"] = "11192",
                ["SERVER_ADDR"] = "127.0.0.1",
                ["SERVER_PORT"] = "12121",
                ["SERVER_NAME"] = "localhost",
                ["HTTP_HOST"] = "localhost:12121",
                ["HTTP_CONNECTION"] = "keep-alive",
                ["HTTP_CACHE_CONTROL"] = "max-age=0",
                ["HTTP_ACCEPT"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                ["HTTP_UPGRADE_INSECURE_REQUESTS"] = "1",
                ["HTTP_USER_AGENT"] = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.78 Safari/537.36",
                ["HTTP_ACCEPT_ENCODING"] = "gzip, deflate, sdch",
                ["HTTP_ACCEPT_LANGUAGE"] = "de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4",
            };

            var fcgiRequest = new FastCGI.Request(0, null);
            fcgiRequest.Parameters = fcgiParams;
            var request = new HttpRequest(fcgiRequest);
            var response = app.HandleRequest(request);

            return response.Body;
        }

        [SetUp]
        public void Init()
        {
            app = new App();
            rand = new Random();
        }

        [Test]
        public void Http_Basic_Url_Patterns()
        {
            var urlsByPatterns = new Dictionary<string, string>
            {
                [@"^$"] = "",
                [@"^/a$"] = "/a",
                [@"^/b/\w+$"] = "/b/abc",
                [@"^/c/\w+/world$"] = "/c/hello/world",
                [@"^/c/\w+/world/$"] = "/c/hello/world/",
            };

            foreach(var kvp in urlsByPatterns)
            {
                var pattern = kvp.Key;
                var url = kvp.Value;

                var expectedResponse = rand.Next().ToString();
                app.AddUrlPattern(pattern, request => new RawHttpResponse(expectedResponse));

                var response = GetResponseFromUrl(url);

                Assert.AreEqual(expectedResponse, response);
            }
        }

        [Test]
        public void Http_HttpResponse()
        {
            var responseText = "This is some content.\nIt can contain header-like text:\nServer:Apache/2";
            var contentType = "text/text";
            var statusCode = "404 Not Found";
            var additionalHeaders = "Date: Sat, 29 Aug 2015 16:35:44 GMT";

            app.AddUrlPattern(
                "^/test-httpresponse$",
                request => new HttpResponse(
                    responseText,
                    status: statusCode,
                    contentType: contentType,
                    additionalHeaders: additionalHeaders
                    )
                );

            string response = GetResponseFromUrl("/test-httpresponse");

            Assert.True(response.Contains(additionalHeaders));
            Assert.True(response.EndsWith(responseText));
            Assert.True(response.Contains("Status: " + statusCode));
            Assert.True(response.Contains("Content-Type: " + contentType));
        }
    }
}
