using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyRenewalApp.Models;

namespace LegacyRenewalApp.Helper
{
    public class FeeCalculator
    {
        public FeeCalculatorResult CalculateFees(bool includePremiumSupport, string normalizedPlanCode, string normalizedPaymentMethod, decimal subtotalAfterDiscount)
        {
            decimal supportFee = 0m;
            string notes = String.Empty; 
            if (includePremiumSupport)
            {
                if (normalizedPlanCode == "START")
                {
                    supportFee = 250m;
                }
                else if (normalizedPlanCode == "PRO")
                {
                    supportFee = 400m;
                }
                else if (normalizedPlanCode == "ENTERPRISE")
                {
                    supportFee = 700m;
                }
                notes += "premium support included; ";
            }
            decimal paymentFee = 0m;
            if (normalizedPaymentMethod == "CARD")
            {
                paymentFee = (subtotalAfterDiscount + supportFee) * 0.02m;
                notes += "card payment fee; ";
            }
            else if (normalizedPaymentMethod == "BANK_TRANSFER")
            {
                paymentFee = (subtotalAfterDiscount + supportFee) * 0.01m;
                notes += "bank transfer fee; ";
            }
            else if (normalizedPaymentMethod == "PAYPAL")
            {
                paymentFee = (subtotalAfterDiscount + supportFee) * 0.035m;
                notes += "paypal fee; ";
            }
            else if (normalizedPaymentMethod == "INVOICE")
            {
                paymentFee = 0m;
                notes += "invoice payment; ";
            }
            else
            {
                throw new ArgumentException("Unsupported payment method");
            }
            FeeCalculatorResult result = new FeeCalculatorResult();
            result.PaymentFee = paymentFee;
            result.SupportFee = supportFee;
            result.Notes = notes;
            return result;
        }

    }
}
