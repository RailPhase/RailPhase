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
    public class HttpResponse : RawHttpResponse
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
        public HttpResponse(string body, string status = "200 OK", string contentType = "text/html", string additionalHeaders = "", IEnumerable<Cookie> cookies = null)
        {
            var responseText = new StringBuilder();
            responseText.AppendLine("HTTP/1.1 " + status);
            responseText.AppendLine("Content-Type: " + contentType + "; charset=utf-8");

            // Write the cookie headers
            if (cookies != null)
            {
                foreach (var cookie in cookies)
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
            if (!String.IsNullOrEmpty(additionalHeaders))
                responseText.AppendLine(additionalHeaders);

            // Empty line marks the start of the body
            responseText.AppendLine("");
            responseText.Append(body);

            RawBody = Encoding.UTF8.GetBytes(responseText.ToString());
        }

        /// <summary>
        /// Implicit conversion from string to HttpResponse
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator HttpResponse(string s)
        {
            return new HttpResponse(s);
        }
    }

    public class RedirectResponse : HttpResponse
    {
        public RedirectResponse(string location, bool permanentRedirect = false) :
            base(
                "",
                status: (permanentRedirect ? "301 Moved Permanently" : "307 Temporary Redirect"),
                additionalHeaders: "Location:" + location
            )
        { }
    }
}
