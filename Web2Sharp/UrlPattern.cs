using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Web2Sharp
{
    public class UrlPattern
    {
        public UrlPattern(Regex pattern, View view)
        {
            Pattern = pattern;
            View = view;
        }

        public UrlPattern(string pattern, View view)
            : this(new Regex(pattern), view)
        { }

        public readonly Regex Pattern;
        public readonly View View;
    }
}
