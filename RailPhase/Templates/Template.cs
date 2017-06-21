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
    /// <param name="data">The data that is rendered by this template.</param>
    /// <param name="context">The <see cref="Context"/> of the current request.</param>
    /// <returns>Returns the template, rendered into a string.</returns>
    /// <seealso cref="Template.FromFile(string)"/>
    /// <seealso cref="Template.FromString(string)"/>
    public delegate string TemplateRenderer(object data, Context context);

    /// <summary>
    /// A block renderer is used internally to render single blocks inside of templates.
    /// </summary>
    /// <param name="data">The data that is rendered by this template.</param>
    /// <param name="context">The <see cref="Context"/> of the current request.</param>
    /// <param name="blockRenderers"></param>
    /// <returns>Returns the block, rendered into a string.</returns>
    public delegate string BlockRenderer(object data, Context context, Dictionary<string,BlockRenderer> blockRenderers);
}
