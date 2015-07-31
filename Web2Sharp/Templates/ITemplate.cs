using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web2Sharp.Templates
{
    public interface ITemplate
    {
        string Render(IDictionary<string, object> context);
    }
}
