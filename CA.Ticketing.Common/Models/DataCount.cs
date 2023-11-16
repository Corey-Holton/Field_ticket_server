using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Common.Models
{
    public class DataCount<T>
    {
        public int TotalCount { get; set; }
        public List<T> ItemsList { get; set; }



    }
}
