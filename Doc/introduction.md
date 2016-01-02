# Introduction

RailPhase is a web framework. It helps you to develop websites, web applications, or basically any software that responds to HTTP requests.

This is an overview of the main concepts of RailPhase. After reading this, you will be able to

## Basic concepts

RailPhase focuses on providing a simple set of convenient functions and tools that allow you to write robust web applications. These are the most basic concepts:

* **App**: An *app* is a single web application (or website). The App class of RailPhase is the main class you use to configure the behaviour of your web application.
* **View**: RailPhase receives HTTP requests and forwards them to your *views*. A view accepts a request and returns a response. Add *URL patterns* to your app to let it know which URLs should be handled by which views.
* **Template**: A *template* is a convenient way to render HTML (or other) content. You can think of it as the design of your website, separated from the code. Usually, templates contain placeholder tags where dynamic content will be filled in. Another way to think of a template is a description of how to format the information you want to display.
* **Context**: If you render a template, you can provide it with a *context object*. This context object holds the information that you want to display.

More detailed explanations will follow in the next sections.

## Getting started

Here is a checklist to get you ready for your first web application made with RailPhase:

1. You should be familiar with the basic concepts of .NET and know how to develop .NET applications.
2. Create a new "console application" .NET project. The examples in this documentation will be written in C#, but you can use any language that [compiles to CLI](https://en.wikipedia.org/wiki/List_of_CLI_languages).
3. Use [nuget](https://www.nuget.org) to add RailPhase to your project. Alternatively, you can also download the latest release on [Github](https://github.com/LukasBoersma/RailPhase/releases/latest).

## Your first application

This example sets up a simple web application:

```csharp
var app = new App();

// Add a simple view for the root URL
app.AddUrl("/", (request) => new HttpResponse("<h1>Hello World</h1>"));

// Start accepting requests from localhost. Default port is 8080.
// This method never returns!
app.RunHttpServer();
```
