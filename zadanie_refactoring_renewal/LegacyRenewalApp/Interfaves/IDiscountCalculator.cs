using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyRenewalApp.Helper;
using LegacyRenewalApp.Models;
using LegacyRenewalApp.Repositories;

namespace LegacyRenewalApp.Interfaves
{
    public interface IDiscountCalculator
    {
        public DiscountResult CalculateDiscount(Customer customer, SubscriptionPlan plan, int customerId, string normalizedPlanCode, string normalizedPaymentMethod, int seatCount, bool useLoyaltyPoints);
    }
}
