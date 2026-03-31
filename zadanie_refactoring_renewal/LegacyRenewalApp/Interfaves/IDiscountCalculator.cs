using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyRenewalApp.Interfaves
{
    public interface IDiscountCalculator
    {
        public decimal CalculateDiscount(int customerId, string planCode, int seatCount);
    }
}
