# Template guide

The template system of RailPhase helps you to render HTML pages and other content.
Templates are also a nice way to separate your application logic from the design and content.

From the application's point of view, a template is a just function that returns a string.

Templates are usually loaded from files (but you can load strings, too). Like this:

````language:csharp
var renderTemplate = Template.FromFile("MyTemplate.html");
var htmlContent = renderTemplate(context: null);
````

While `MyTemplate.html` could be something like this:

````language:html
<h1>Hello World!</h1>
The current time is {{DateTime.Now}}.
````

The `DateTime.Now` tag is evaluated and replaced on server side, so the `renderTemplate` call returns
pure HTML code with all template tags replaced:

````language:html
<h1>Hello World!</h1>
The current time is 1/2/2016 1:02:15 PM.
````

# Expressions

Expression tags are C# code placed in ``{{}}`` brackets. You already saw one in the example above.

Any valid C# expression is allowed. When the template is rendered, the expressions are evaluated and
the tags are replaced with the respective values. If the type of the expression is not
`System.String`, the `ToString()` method is
used to convert it into a string.

# Context

Context objects can be used to pass data from your code to the template. When passing a context object
to a template, all member fields and properties will be available in the template expressions.

If you think of a website as a formatted version of some information, then the context contains that
information and the template describes how to format it.

When working with context objects, you need to specify the type of the context object in your
template. Use the `context` tag to do this:

````language:html
{% context MyApp.MyClass %}
````

## Example

Let's say you are creating a blog. Then your website is basically a list of articles, which could be
represented like this:

````language:csharp
namespace MyBlog
{
    public class Article
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public string Content { get; set; }
    }
}
````

Then, the template could look like this:

````language:html
{% context MyBlog.Article %}

<h1>{{Title}}</h1>
<h2>{{Subtitle}}</h1>

<div class="article-meta">Posted by {{Author}} on {{PublishDate}}</div>

<div class="article-content">
{{Content}}
</div>
````

And in your view, you simply pass the article to the template:

````language:csharp
public HttpResponse ViewArticle(HttpRequest request)
{
  var article = GetArticle(request);
  var renderArticle = Template.FromFile("Article.html");
  var formattedArticle = renderArticle(article);
  return new HttpResponse(formattedArticle);
}
````
