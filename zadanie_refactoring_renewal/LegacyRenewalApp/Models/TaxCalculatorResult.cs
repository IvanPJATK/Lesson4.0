using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyRenewalApp.Enums;

namespace LegacyRenewalApp.Models
{
    public class TaxCalculatorResult
    {
        public decimal taxRate { get; set; }
        public decimal TaxBase { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string Notes { get; set; } = String.Empty;
    }
}
