using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Common.Extensions
{
    public class DayTimeExtensions
    {
        public static bool IsWithinMonth(DateTime? date)
        {
            if (date == null)
            {
                return false;
            }

            var birthday = new DateTime(DateTime.Now.Year, date.Value.Month, date.Value.Day);
            var difference = birthday - DateTime.Now;
            if (difference.TotalDays <= 30 && difference.TotalDays >= 0)
            {
                return true;
            }

            return false;
        }
    }
}
