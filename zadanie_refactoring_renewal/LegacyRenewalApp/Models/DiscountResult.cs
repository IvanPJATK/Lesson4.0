using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyRenewalApp.Models
{
    public class DiscountResult
    {
        public decimal BaseAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal SubtotalAfterDiscount { get; set; }
        public string Notes { get; set; }
    }
}
