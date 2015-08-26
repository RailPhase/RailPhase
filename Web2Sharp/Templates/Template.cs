using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Web2Sharp.Templates
{
    public delegate string TemplateRenderer(object context);
    public delegate string BlockRenderer(object context, Dictionary<string,BlockRenderer> blockRenderers);
}
