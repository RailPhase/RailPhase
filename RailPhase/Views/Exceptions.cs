using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailPhase
{
    public class RequestExceptionInfo
    {
        public Exception Exception { get; set; }
        public Context Context { get; set; }
    }
    public class RequestException: Exception
    {
        const string DebugExceptionTemplate = @"
                    {% context RailPhase.RequestExceptionInfo, RailPhase %}
      <!DOCTYPE html>
      <html lang=""en"">
      <head>
          <meta charset = ""utf-8"" >
          <title>{{Exception.GetType().FullName}} - {{Exception.Message}}</title>
          <style>
              html {
                font-family: ""Arial"", sans-serif;
                font-size: 16px;
                background: #eee;
              }
              body {
                max-width: 960px;
                min-height: 100vh;
                padding: 20px;
                margin: 0 auto;
                background: #fff;
              }

              h1 {
                padding: 30px;
                background: #1693de;
                margin: -20px -20px 20px -20px;
              }

              pre {
                overflow: auto;
                border: 1px solid #ccc;
              }

              table {
                text-align: left;
                border: 1px solid #ccc;
                width: 100%;
              }

              table td {
                border-top: 1px solid #eee;
              }
          </style>
      </head>
      <body>
          <h1>Internal Server Error: {{Exception.GetType().Name}}</h1>
          <p>While processing this request, an unhandled exception of type {{Exception.GetType().FullName}} was thrown.</p>
          {% if !string.IsNullOrEmpty(Exception.Message) %}
            <p><b>The exception message is</b>: {{Exception.Message}}</p>
          {% endif %}
          <hr>
          <h2>Stack</h2>
          <code><pre>{{Exception.StackTrace}}</pre></code>
          <hr>
          <h2>Request: {{Context.Request.RawUrl}}</h2>
          <table>
              <tr><th>Header Name</th><th>Value</th></tr>
              {% for key in Context.Request.Headers.AllKeys %}
                  <tr><td>{{key}}</td><td>{{Context.Request.Headers[key]}}</td></tr>
              {% endfor %}
          </table>
          <hr>
          <p><i>This error message was generated on {{DateTime.Now}} by <a href=""https://railphase.github.io"">RailPhase</a> {{typeof(RailPhase.App).Assembly.GetName().Version}}.</i></p>
      </body>
      </html>
        ";
        public static void ViewDebugException(Context context)
        {
            var exception = context.GetTag<Exception>();
            var info = new RequestExceptionInfo
            {
                Exception = exception,
                Context = context
            };

            var template = Template.FromString(DebugExceptionTemplate);
            context.Response.StatusCode = 500;
            context.WriteResponse(template(info));
        }
    }

    public class NotFoundException: RequestException
    {
    }

    public class NotAllowedException: RequestException
    {

    }
}
