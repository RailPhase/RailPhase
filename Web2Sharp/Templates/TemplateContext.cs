using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web2Sharp.Templates
{
    public class TemplateContext
    {
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public readonly HttpRequest Request;

        public object this[string key]
        {
            get
            {
                return Parameters[key];
            }
        }

        public TemplateContext(HttpRequest request, IDictionary<string, object> parameters)
        {
            Request = request;
            foreach(var param in parameters)
            {
                Parameters.Add(param.Key, param.Value);
            }
        }

        public TemplateContext(HttpRequest request)
        {
            Request = request;
        }
    }
}
