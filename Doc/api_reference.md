<a id="Web2Sharp.App"></a>
## class Web2Sharp.App

The main class for Web2Sharp web applications.

**Constructors**

<a id="Web2Sharp.App..ctor"></a>

* **App** *()*  



**Methods**

<a id="Web2Sharp.App.AddUrlPattern(Web2Sharp.UrlPattern)"></a>

* *void* **AddUrlPattern** *(Web2Sharp.UrlPattern pattern)*  
  Adds a new URL pattern.  
  When the App receives a request with a URL that matches the given pattern, the specified view will be called.
Please note that there are overloads of this method with more convenient signatures, like [AddUrlPattern](#Web2Sharp.App.AddUrlPattern(System.String,Web2Sharp.View)) .

<a id="Web2Sharp.App.AddUrlPattern(System.String,Web2Sharp.View)"></a>

* *void* **AddUrlPattern** *(string pattern, Web2Sharp.View view)*  
  Adds a new URL pattern.  
  When the App receives a request with a URL that matches the given pattern, the specified view will be called.

<a id="Web2Sharp.App.AddUrlPattern(System.String,Web2Sharp.Templates.TemplateRenderer,System.String)"></a>

* *void* **AddUrlPattern** *(string pattern, Web2Sharp.Templates.TemplateRenderer template, [string contentType])*  
  Adds a new URL pattern that responds to requests with a template.  

<a id="Web2Sharp.App.AddUrlPattern(System.String,System.String,System.String)"></a>

* *void* **AddUrlPattern** *(string pattern, string templateFile, [string contentType])*  
  Adds a new URL pattern that responds to requests with a template.  

<a id="Web2Sharp.App.HandleRequest(Web2Sharp.HttpRequest)"></a>

* *Web2Sharp.RawHttpResponse* **HandleRequest** *(Web2Sharp.HttpRequest request)*  
  Handles an incoming HTTP request. You usually do not need to call this.  
  When called, this method will go through the registered URL patterns and pass the request to the view of the pattern that matches first.
If not URL pattern matches the URL of the request, a 404 page is returned.

<a id="Web2Sharp.App.ReceiveFcgiRequest(System.Object,FastCGI.Request)"></a>

* *void* **ReceiveFcgiRequest** *(Object sender, FastCGI.Request fcgiRequest)*  
  Handles an incoming FastCGI request. You usually do not need to call this.  

<a id="Web2Sharp.App.Run(System.Int32)"></a>

* *void* **Run** *([int port])*  
  Starts listening as a FastCGI client. This method never returns!  
  This method starts the FastCGI client and will respond to any requests that are received over FastCGI. Any URL patterns have to be registered before calling this, because this method never returns.




---

<a id="Web2Sharp.HttpRequest"></a>
## class Web2Sharp.HttpRequest

Represents an incoming HTTP request.

**Constructors**

<a id="Web2Sharp.HttpRequest..ctor"></a>

* **HttpRequest** *(FastCGI.Request fcgiRequest)*  
  Creates a new request object.  



**Methods**

<a id="Web2Sharp.HttpRequest.GetParameterASCII(System.String)"></a>

* *string* **GetParameterASCII** *(string name)*  

<a id="Web2Sharp.HttpRequest.GetParameterUTF8(System.String)"></a>

* *string* **GetParameterUTF8** *(string name)*  


**Properties and Fields**

<a id="Web2Sharp.HttpRequest.ServerParameters"></a>

* *IDictionary&lt;string, Byte[]&gt;* **ServerParameters**  
  A dictionary of all HTTP parameters included in the request  


<a id="Web2Sharp.HttpRequest.GET"></a>

* *Dictionary&lt;string, string&gt;* **GET**  
  A dictionary of all GET parameters included in the request.  


<a id="Web2Sharp.HttpRequest.Uri"></a>

* *string* **Uri**  
  The URI of this request  


<a id="Web2Sharp.HttpRequest.Body"></a>

* *string* **Body**  
  The HTTP body of the request.  


<a id="Web2Sharp.HttpRequest.Method"></a>

* *Web2Sharp.HttpMethod* **Method**  
  The HTTP method of the request.  



<a id="Web2Sharp.HttpRequest.FcgiRequest"></a>

* *FastCGI.Request* **FcgiRequest**  
  The underlying FastCGI request. Contains some more detailed information.  





---

<a id="Web2Sharp.HttpMethod"></a>
## enum Web2Sharp.HttpMethod

Specifies a Http Method.

**Enum Values**

* **GET**
* **POST**
* **PUT**
* **DELETE**
* **UNKNOWN**


---

<a id="Web2Sharp.RawHttpResponse"></a>
## class Web2Sharp.RawHttpResponse

Base class for HTTP responses. Use [HttpResponse](#Web2Sharp.HttpResponse) if you want to create a simple HTTP response.

**Constructors**

<a id="Web2Sharp.RawHttpResponse..ctor"></a>

* **RawHttpResponse** *([string body])*  
  Creates a new raw http response, without any headers pre-set.  



**Properties and Fields**


<a id="Web2Sharp.RawHttpResponse.Body"></a>

* *string* **Body**  
  The raw body of the HTTP response, including all headers.  





---

<a id="Web2Sharp.HttpResponse"></a>
## class Web2Sharp.HttpResponse
*Extends Web2Sharp.RawHttpResponse*

Represents a HTTP response.

If you need full control over the raw response content, use [RawHttpResponse](#Web2Sharp.RawHttpResponse) instead.

**Constructors**

<a id="Web2Sharp.HttpResponse..ctor"></a>

* **HttpResponse** *(string body, [string status], [string contentType], [string additionalHeaders])*  
  Creates a HTTP response, with the most important headers already set.  
  If you need full control over the raw response content, use [RawHttpResponse](#Web2Sharp.RawHttpResponse) instead.





---

<a id="Web2Sharp.UrlPattern"></a>
## class Web2Sharp.UrlPattern

Represents a URL pattern, used by the [App](#Web2Sharp.App) class to handle incoming requests.

**Constructors**

<a id="Web2Sharp.UrlPattern..ctor"></a>

* **UrlPattern** *(System.Text.RegularExpressions.Regex pattern, Web2Sharp.View view)*  
  Creates a new URL pattern.  


<a id="Web2Sharp.UrlPattern..ctor"></a>

* **UrlPattern** *(string pattern, Web2Sharp.View view)*  
  Creates a new URL pattern.  



**Properties and Fields**


<a id="Web2Sharp.UrlPattern.Pattern"></a>

* *System.Text.RegularExpressions.Regex* **Pattern**  
  The regular expression for the URL pattern.  


<a id="Web2Sharp.UrlPattern.View"></a>

* *Web2Sharp.View* **View**  
  The view that should be called for requests that match the pattern.  





---

<a id="Web2Sharp.Templates.TemplateParserException"></a>
## class Web2Sharp.Templates.TemplateParserException
*Extends System.Exception*

Represents a syntax error in a template.

**Constructors**

<a id="Web2Sharp.Templates.TemplateParserException..ctor"></a>

* **TemplateParserException** *(string message)*  





---

<a id="Web2Sharp.Templates.Template"></a>
## class Web2Sharp.Templates.Template

Provides functions to work with templates.

**Static Methods**

<a id="Web2Sharp.Templates.Template.FromFile(System.String)"></a>

* *Web2Sharp.Templates.TemplateRenderer* **FromFile** *(string filename)*  
  Loads a [TemplateRenderer](#Web2Sharp.Templates.TemplateRenderer) from a file.  

<a id="Web2Sharp.Templates.Template.FromString(System.String)"></a>

* *Web2Sharp.Templates.TemplateRenderer* **FromString** *(string text)*  
  Loads a [TemplateRenderer](#Web2Sharp.Templates.TemplateRenderer) from a string.  




---

