using System.Collections.Generic;
using System.Linq;

namespace ExcelExport.Helper
{
    public static class Extrusion
    {
        public static Dictionary<T, IEnumerable<TS>> TakeByGroup<T,TS>(
            this IEnumerable<IGrouping<T, TS>> values, int count)
        {
            var temp = new Dictionary<T, IEnumerable<TS>>();

            foreach (var value in values)
            {
                temp.Add(value.Key, value.Take(count));
            }

            return temp;
        }
    }
}
