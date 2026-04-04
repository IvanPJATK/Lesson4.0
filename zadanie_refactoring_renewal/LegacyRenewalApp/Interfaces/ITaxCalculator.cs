using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyRenewalApp.Enums;
using LegacyRenewalApp.Models;

namespace LegacyRenewalApp.Interfaves
{
    public interface ITaxCalculator
    {
        public TaxCalculatorResult calculateTax(CountryEnum country, decimal subtotalAfterDiscount, decimal supportFee, decimal paymentFee);
    }
}
