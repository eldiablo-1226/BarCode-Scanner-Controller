using System;
using System.Collections.Generic;
using System.Linq;
using DataBase.Model;

namespace BarCodeScanner.Helps
{
    public static class Extrusion
    {
        public static Dictionary<DateTime, IEnumerable<WorkTimeLog>> TakeByGroup(
            this IList<WorkTimeLog> values)
        {
            var temp = new Dictionary<DateTime, IEnumerable<WorkTimeLog>>();
            var sorted = values.GroupBy(x => x.ScanTime.Date);
            foreach (var value in sorted)
            {
                List<WorkTimeLog> bufer = new List<WorkTimeLog>();
                var sortbyworker = value.GroupBy(x => x.WorkerBy.Id);
                foreach (var i in sortbyworker)
                {
                    //List<WorkTimeLog> tempbuffer = new List<WorkTimeLog>(2);
                    int index = 0;
                    foreach (var j in i)
                    {
                        if (index >= 2)
                            break;
                        bufer.Add(j);
                        index++;
                    }
                }

                temp.Add(value.Key, bufer);
            }

            return temp;
        }
    }
}
