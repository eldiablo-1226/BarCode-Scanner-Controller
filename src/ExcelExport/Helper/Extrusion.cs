using System;
using System.Collections.Generic;
using System.Linq;
using DataBase.Model;

namespace ExcelExport.Helper
{
    public static class Extrusion
    {
        public static Dictionary<DateTime, IEnumerable<WorkTimeLog>> TakeByGroup(
            this IEnumerable<IGrouping<DateTime, WorkTimeLog>> values, int count)
        {
            var temp = new Dictionary<DateTime, IEnumerable<WorkTimeLog>>();

            foreach (var value in values)
            {
                var group = value.GroupBy(x => x.WorkerBy);
                var bufer = group
                    .Select(x => x.Take(3))
                    .ToList();

                //temp.Add(value.Key, bufer);
            }

            return temp;
        }
    }
}
