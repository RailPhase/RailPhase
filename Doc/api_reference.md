<a id="RailPhase.App"></a>
## class RailPhase.App

The main class for RailPhase web applications.

**Constructors**

<a id="RailPhase.App..ctor"></a>

* **App** *()*  



**Methods**

<a id="RailPhase.App.AddUrlPattern(RailPhase.UrlPattern)"></a>

* *void* **AddUrlPattern** *(RailPhase.UrlPattern pattern)*  
  Adds a new URL pattern.  
  When the App receives a request with a URL that matches the given pattern, the specified view will be called.
Please note that there are overloads of this method with more convenient signatures, like [AddUrlPattern](#RailPhase.App.AddUrlPattern(System.String,RailPhase.View)) .

<a id="RailPhase.App.AddUrlPattern(System.String,RailPhase.View)"></a>

* *void* **AddUrlPattern** *(string pattern, RailPhase.View view)*  
  Adds a new URL pattern.  
  When the App receives a request with a URL that matches the given pattern, the specified view will be called.

<a id="RailPhase.App.AddUrlPattern(System.String,RailPhase.Templates.TemplateRenderer,System.String)"></a>

* *void* **AddUrlPattern** *(string pattern, RailPhase.Templates.TemplateRenderer template, [string contentType])*  
  Adds a new URL pattern that responds to requests with a template.  

<a id="RailPhase.App.AddUrlPattern(System.String,System.String,System.String)"></a>

* *void* **AddUrlPattern** *(string pattern, string templateFile, [string contentType])*  
  Adds a new URL pattern that responds to requests with a template.  

<a id="RailPhase.App.HandleRequest(RailPhase.HttpRequest)"></a>

* *RailPhase.RawHttpResponse* **HandleRequest** *(RailPhase.HttpRequest request)*  
  Handles an incoming HTTP request. You usually do not need to call this.  
  When called, this method will go through the registered URL patterns and pass the request to the view of the pattern that matches first.
If not URL pattern matches the URL of the request, a 404 page is returned.

<a id="RailPhase.App.ReceiveFcgiRequest(System.Object,FastCGI.Request)"></a>

* *void* **ReceiveFcgiRequest** *(Object sender, FastCGI.Request fcgiRequest)*  
  Handles an incoming FastCGI request. You usually do not need to call this.  

<a id="RailPhase.App.Run(System.Int32)"></a>

* *void* **Run** *([int port])*  
  Starts listening as a FastCGI client. This method never returns!  
  This method starts the FastCGI client and will respond to any requests that are received over FastCGI. Any URL patterns have to be registered before calling this, because this method never returns.




---

<a id="RailPhase.HttpRequest"></a>
## class RailPhase.HttpRequest

Represents an incoming HTTP request.

**Constructors**

<a id="RailPhase.HttpRequest..ctor"></a>

* **HttpRequest** *(FastCGI.Request fcgiRequest)*  
  Creates a new request object.  



**Methods**

<a id="RailPhase.HttpRequest.GetParameterASCII(System.String)"></a>

* *string* **GetParameterASCII** *(string name)*  

<a id="RailPhase.HttpRequest.GetParameterUTF8(System.String)"></a>

* *string* **GetParameterUTF8** *(string name)*  


**Properties and Fields**

<a id="RailPhase.HttpRequest.ServerParameters"></a>

* *IDictionary&lt;string, Byte[]&gt;* **ServerParameters**  
  A dictionary of all HTTP parameters included in the request  


<a id="RailPhase.HttpRequest.GET"></a>

* *Dictionary&lt;string, string&gt;* **GET**  
  A dictionary of all GET parameters included in the request.  


<a id="RailPhase.HttpRequest.Uri"></a>

* *string* **Uri**  
  The URI of this request  


<a id="RailPhase.HttpRequest.Body"></a>

* *string* **Body**  
  The HTTP body of the request.  


<a id="RailPhase.HttpRequest.Method"></a>

* *RailPhase.HttpMethod* **Method**  
  The HTTP method of the request.  



<a id="RailPhase.HttpRequest.FcgiRequest"></a>

* *FastCGI.Request* **FcgiRequest**  
  The underlying FastCGI request. Contains some more detailed information.  





---

<a id="RailPhase.HttpMethod"></a>
## enum RailPhase.HttpMethod

Specifies a Http Method.

**Enum Values**

* **GET**
* **POST**
* **PUT**
* **DELETE**
* **UNKNOWN**


---

<a id="RailPhase.RawHttpResponse"></a>
## class RailPhase.RawHttpResponse

Base class for HTTP responses. Use [HttpResponse](#RailPhase.HttpResponse) if you want to create a simple HTTP response.

**Constructors**

<a id="RailPhase.RawHttpResponse..ctor"></a>

* **RawHttpResponse** *([string body])*  
  Creates a new raw http response, without any headers pre-set.  



**Properties and Fields**


<a id="RailPhase.RawHttpResponse.Body"></a>

* *string* **Body**  
  The raw body of the HTTP response, including all headers.  





---

<a id="RailPhase.HttpResponse"></a>
## class RailPhase.HttpResponse
*Extends RailPhase.RawHttpResponse*

Represents a HTTP response.

If you need full control over the raw response content, use [RawHttpResponse](#RailPhase.RawHttpResponse) instead.

**Constructors**

<a id="RailPhase.HttpResponse..ctor"></a>

* **HttpResponse** *(string body, [string status], [string contentType], [string additionalHeaders])*  
  Creates a HTTP response, with the most important headers already set.  
  If you need full control over the raw response content, use [RawHttpResponse](#RailPhase.RawHttpResponse) instead.





---

<a id="RailPhase.UrlPattern"></a>
## class RailPhase.UrlPattern

Represents a URL pattern, used by the [App](#RailPhase.App) class to handle incoming requests.

**Constructors**

<a id="RailPhase.UrlPattern..ctor"></a>

* **UrlPattern** *(System.Text.RegularExpressions.Regex pattern, RailPhase.View view)*  
  Creates a new URL pattern.  


<a id="RailPhase.UrlPattern..ctor"></a>

* **UrlPattern** *(string pattern, RailPhase.View view)*  
  Creates a new URL pattern.  



**Properties and Fields**


<a id="RailPhase.UrlPattern.Pattern"></a>

* *System.Text.RegularExpressions.Regex* **Pattern**  
  The regular expression for the URL pattern.  


<a id="RailPhase.UrlPattern.View"></a>

* *RailPhase.View* **View**  
  The view that should be called for requests that match the pattern.  





---

<a id="RailPhase.Templates.TemplateParserException"></a>
## class RailPhase.Templates.TemplateParserException
*Extends System.Exception*

Represents a syntax error in a template.

**Constructors**

<a id="RailPhase.Templates.TemplateParserException..ctor"></a>

* **TemplateParserException** *(string message)*  





---

<a id="RailPhase.Templates.Template"></a>
## class RailPhase.Templates.Template

Provides functions to work with templates.

**Static Methods**

<a id="RailPhase.Templates.Template.FromFile(System.String)"></a>

* *RailPhase.Templates.TemplateRenderer* **FromFile** *(string filename)*  
  Loads a [TemplateRenderer](#RailPhase.Templates.TemplateRenderer) from a file.  

<a id="RailPhase.Templates.Template.FromString(System.String)"></a>

* *RailPhase.Templates.TemplateRenderer* **FromString** *(string text)*  
  Loads a [TemplateRenderer](#RailPhase.Templates.TemplateRenderer) from a string.  




---

