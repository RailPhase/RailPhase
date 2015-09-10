# RailPhase

[![Build Status](https://travis-ci.org/LukasBoersma/RailPhase.svg?branch=master)](https://travis-ci.org/LukasBoersma/RailPhase)

This library allows you to write web applications in .NET via FastCGI. The API is inspired by [Django](https://www.djangoproject.com/). Right now, this library has far less features, though.

See below for a simple usage example.

RailPhase uses [FastCGI for .NET](https://github.com/LukasBoersma/FastCGI) for the FastCGI protocol and adds a lightweight layer of convenience functions. RailPhase is based on .NET 4.5.

## License and contributing

This software is distributed under the terms of the MIT license. You can use it for your own projects for free under the conditions specified in LICENSE.txt.

If you have questions, feel free to contact me. Visit [lukas-boersma.com](https://lukas-boersma.com) for my contact details.

If you think you found a bug, you can open an Issue on Github. If you make changes to this library, I would be happy about a pull request.

## Documentation

The documentation is hosted here: [Documentation](http://RailPhase.readthedocs.org/en/latest/)

Direct link to the API reference: [API Reference](http://RailPhase.readthedocs.org/en/latest/api_reference)

## Basic usage

Use this library together with a web server like Apache and nginx. The web server will serve static content and forward HTTP requests for dynamic content to your application.

RailPhase starts listening for requests and forwards them to your *views*. A view accepts a HttpRequest and returns a HttpResponse. Regular expressions are used to determine which URL pattern should be mapped to which view.

Before using, make sure that you understand the concept of [FastCGI](https://en.wikipedia.org/wiki/FastCGI). Your RailPhase app will *not* accept HTTP connections. You need a web server to access your content.

This example sets up a simple web application:

```csharp
    // Create a simple web app and serve content on to URLs.
    var app = new App();

    // Define the URL patterns
    app.AddUrlPattern("^/$", (request) => new HttpResponse("<h1>Hello World</h1>"));
    app.AddUrlPattern("^/info$", InfoView);

    // Start accepting requests from the web server on localhost. Default port is 19000.
    // This method never returns!
    app.Run();
```

Where *InfoView* could be something like:

```csharp
    static HttpResponse InfoView(RailPhase.HttpRequest request)
    {
        // This is of course not the recommended way to generate HTML content,
        // but it is the simplest way for this example.

        string body = "<html><head>\n";
        body += @"<style>
                    table {
                        border-collapse: collapse;
                    }

                    table, th, td {
                        border: 1px solid black;
                    }
                 </style>";

        body += "</head><body>\n";
        body += "<h1>Request Info Page</h1>\n";

        // Print all server parameters into a table
        body += "<h2>Server parameters</h2>\n";
        body += "<table><tr><th>Name</th><th>Value</th>\n";
        foreach (var param in request.ServerParameters)
        {
            body += string.Format(
                            "<tr><td>{0}</td><td>{1}</td></tr>\n",
                            param.Key, param.Value
                        );
        }
        body += "</table>\n";
        body += "</body></html>\n";

        // Return a simple Http response.
        // We could also return non-HTML content or error codes here
        // by setting the parameters in the HttpResponse constructor.
        return new HttpResponse(body);
    }
```

If you run this program and set up your web server properly (see below), you should be able to connect your browser to the root URL and "/info".

## Web server configuration

RailPhase relies on getting certain FastCGI parameters from the web server.

Refer to your web server documentation for configuration details:

 * [nginx documentation](http://nginx.org/en/docs/http/ngx_http_fastcgi_module.html)
 * [Apache documentation](http://httpd.apache.org/mod_fcgid/mod/mod_fcgid.html)

For nginx, add this to pass all requests to your FastCGI application:

    location / {
        include fastcgi_params;
        fastcgi_pass   127.0.0.1:19000;
    }

Where fastcgi_params is a file in your nginx config folder, containing at least these parameters (you can add more if you need to):

    fastcgi_param   QUERY_STRING            $query_string;
    fastcgi_param   REQUEST_METHOD          $request_method;
    fastcgi_param   CONTENT_TYPE            $content_type;
    fastcgi_param   CONTENT_LENGTH          $content_length;

    fastcgi_param   SCRIPT_FILENAME         $document_root$fastcgi_script_name;
    fastcgi_param   SCRIPT_NAME             $fastcgi_script_name;
    fastcgi_param   PATH_INFO               $fastcgi_path_info;
    fastcgi_param 	PATH_TRANSLATED         $document_root$fastcgi_path_info;
    fastcgi_param   REQUEST_URI             $request_uri;
    fastcgi_param   DOCUMENT_URI            $document_uri;
    fastcgi_param   DOCUMENT_ROOT           $document_root;
    fastcgi_param   SERVER_PROTOCOL         $server_protocol;

    fastcgi_param   GATEWAY_INTERFACE       CGI/1.1;
    fastcgi_param   SERVER_SOFTWARE         nginx/$nginx_version;

    fastcgi_param   REMOTE_ADDR             $remote_addr;
    fastcgi_param   REMOTE_PORT             $remote_port;
    fastcgi_param   SERVER_ADDR             $server_addr;
    fastcgi_param   SERVER_PORT             $server_port;
    fastcgi_param   SERVER_NAME             $server_name;

    fastcgi_param   HTTPS                   $https;
