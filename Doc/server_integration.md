# Web server integration

The recommended way to use RailPhase is to use a dedicated web server like Apache or Nginx and connect it to RailPhase via FastCGI.

## FastCGI configuration

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
