using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SummerBack.NablaUtils
{
    [Serializable]
    public class PagedList<T> : List<T>
    {
        public int NumberOfRecords { get; set; }

        public PagedList()
        {
        }

        public PagedList(IEnumerable<T> source) : base(source)
        {
        }

        public new PagedList<T> GetRange(int index, int count)
        {
            var result = new PagedList<T>();
            result.NumberOfRecords = NumberOfRecords;
            if (index >= 0 && count >= 0)
            {
                result.AddRange(base.GetRange(index, count));   
            }
            return result;
        }
    }
}
