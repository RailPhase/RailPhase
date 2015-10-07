using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailPhase
{
    public class Paginate<T>
    {
        public IEnumerable<T> AllObjects;

        public int ElementsPerPage = 25;

        public int PageCount { get { return 1 + (AllObjects.Count() / ElementsPerPage); } }

        public int Page = 1;

        public IEnumerable<T> PageObjects { get { return GetPage(Page); } }

        public IEnumerable<T> GetPage(int page)
        {
            int skipCount = (page - 1) * ElementsPerPage;
            return AllObjects.Skip(skipCount).Take(ElementsPerPage);
        }
    }
}
