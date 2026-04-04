using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyRenewalApp.Models;

namespace LegacyRenewalApp.Interfaves
{
    public interface IFeeCalculator
    {
        public FeeCalculatorResult CalculateFees(string normalizedPlanCode, string normalizedPaymentMethod, decimal subtotalAfterDiscount);
    }
}
