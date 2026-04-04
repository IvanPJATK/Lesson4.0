using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyRenewalApp.Enums;
using LegacyRenewalApp.Interfaves;
using LegacyRenewalApp.Models;

namespace LegacyRenewalApp.Helper
{
    public class TaxCalculator : ITaxCalculator
    {
        public TaxCalculatorResult calculateTax(CountryEnum country, decimal subtotalAfterDiscount, decimal supportFee, decimal paymentFee)
        {
            string notes = string.Empty;
            decimal taxRate = country switch
            {
                CountryEnum.Poland => 0.23m,
                CountryEnum.Germany => 0.19m,
                CountryEnum.CzechRepublic => 0.21m,
                CountryEnum.Norway => 0.25m,
                _ => 0.20m
            };
            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;
            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }
            return new TaxCalculatorResult { FinalAmount = finalAmount, TaxAmount = taxAmount, TaxBase = taxBase, taxRate = taxRate , Notes = notes};
        }
    }
}
