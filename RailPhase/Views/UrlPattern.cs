using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace RailPhase
{
    /// <summary>
    /// Represents a URL pattern, used by the <see cref="App"/> class to handle incoming requests.
    /// </summary>
    /// <seealso cref="App.AddUrlPattern(UrlPattern)"/>
    public class UrlPattern
    {
        /// <summary>
        /// Creates a new URL pattern.
        /// </summary>
        /// <param name="pattern">The regular expression for the URL pattern.</param>
        /// <param name="view">The view that should be called for requests that match the pattern.</param>
        public UrlPattern(Regex pattern, View view)
        {
            Pattern = pattern;
            View = view;
        }

        /// <summary>
        /// Creates a new URL pattern.
        /// </summary>
        /// <param name="pattern">The regular expression (in .NET Regex syntax) for the URL pattern.</param>
        /// <param name="view">The view that should be called for requests that match the pattern.</param>
        public UrlPattern(string pattern, View view)
            : this(new Regex(pattern), view)
        { }

        /// <summary>
        /// The regular expression for the URL pattern.
        /// </summary>
        public readonly Regex Pattern;

        /// <summary>
        /// The view that should be called for requests that match the pattern.
        /// </summary>
        public readonly View View;
    }

    /// <summary>
    /// Contains information about how a request was matched to a URL pattern.
    /// </summary>
    /// <remarks>
    /// If the URL pattern contained named regex groups, you can use this like a dictionary to access the matched URL substrings.
    /// </remarks>
    /// <example>
    /// app.AddUrlPattern(@"^/article/(?<articleSlug>[\w\-]+)$", ArticleView);
    /// 
    /// [...]
    /// 
    /// public HttpResponse ArticleView
    /// {
    ///     // Get the article slug from the pattern match
    ///     var articleSlug = request.PatternMatch["articleSlug"];
    /// }
    /// </example>
    public class UrlPatternMatch
    {
        /// <summary>
        /// The <see cref="UrlPattern"/> that matched the request.
        /// </summary>
        public UrlPattern Pattern;

        /// <summary>
        /// The Regex Match object, containing information about how the request URI matched the URL pattern.
        /// </summary>
        public Match Match;

        /// <summary>
        /// Convenience access to the named regex groups.
        /// </summary>
        public string this[string groupName]
        {
            get { return Match.Groups[groupName].Value; }
        }
    }
}
