using LegacyRenewalApp.Interfaves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyRenewalApp.Helper
{
    public class DiscountCalculator : IDiscountCalculator
    {
        public decimal CalculateDiscount(int customerId, string planCode, int seatCount)
        {
            // Placeholder for discount calculation logic
            return 0m; // No discount by default
        }
    }
}
