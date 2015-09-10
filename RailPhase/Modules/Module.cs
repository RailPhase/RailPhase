using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailPhase
{
    public abstract class Module
    {
        public IEnumerable<UrlPattern> UrlPatterns { get; protected set; }

        public string Name { get; protected set; }
        public string Author { get; protected set; }
        public string Website { get; protected set; }
    }
}
