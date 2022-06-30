using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VGSShop.Models
{
    public partial class Sp_GetReportProductByMonth
    {
        public int Month { get; set; }
        public string ProductName { get; set; }
        public int total { get; set; }
    }
}
