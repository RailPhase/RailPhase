using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RailPhase
{
    /// <summary>
    /// A template renderer takes a context object and outputs a string.
    /// </summary>
    /// <param name="context">The context object for the template renderer.</param>
    /// <returns>Returns the template, rendered into a string.</returns>
    /// <seealso cref="Template.FromFile(string)"/>
    /// <seealso cref="Template.FromString(string)"/>
    public delegate string TemplateRenderer(object context);

    /// <summary>
    /// A block renderer is used internally to render single blocks inside of templates.
    /// </summary>
    /// <param name="context">The context object for the template renderer.</param>
    /// <param name="blockRenderers"></param>
    /// <returns>Returns the block, rendered into a string.</returns>
    public delegate string BlockRenderer(object context, Dictionary<string,BlockRenderer> blockRenderers);
}
