using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailPhase
{
    public abstract class ObjectView<T>
        where T : ObjectView<T>
    {
        public abstract RawHttpResponse View(HttpRequest request);

        protected static Func<string, string> UriToId;
        protected static Dictionary<string, ObjectView<T>> ObjectsById = new Dictionary<string, ObjectView<T>>();

        static ObjectView()
        {
            var type = typeof(T);
            var idMember = (from property in type.GetProperties()
                            where 
                            select 
        }

        public static RawHttpResponse DispatchView(HttpRequest request)
        {
            var id = UriToId(request.Uri);

            if (!ObjectsById.ContainsKey(id))
                return new HttpResponse("404 Not Found", "404 Not Found");
            else return ObjectsById[id].View(request);
        }
    }
}
