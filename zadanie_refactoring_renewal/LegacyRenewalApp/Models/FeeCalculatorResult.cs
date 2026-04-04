using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyRenewalApp.Models
{
    public class FeeCalculatorResult
    {
        public decimal SupportFee { get; set; }
        public decimal PaymentFee { get; set; }

        public string Notes { get; set; }
    }
}
