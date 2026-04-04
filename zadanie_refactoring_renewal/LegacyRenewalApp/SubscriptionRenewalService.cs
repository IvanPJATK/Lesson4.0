using LegacyRenewalApp.Helper;
using LegacyRenewalApp.Interfaves;
using LegacyRenewalApp.Models;
using LegacyRenewalApp.Repositories;
using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly CustomerRepository _customerRepository;
        private readonly SubscriptionPlanRepository _planRepository;
        private readonly RenewalServiceValidator _renewalServiceValidator;
        private readonly BillingGatewayAdapter _billingGatewayAdapter;
        private readonly DiscountCalculator _discountCalculator;
        private readonly FeeCalculator _feeCalculator;

        public SubscriptionRenewalService() : this(new CustomerRepository(), new SubscriptionPlanRepository(), new RenewalServiceValidator(), new BillingGatewayAdapter(), new DiscountCalculator(), new FeeCalculator())
        { }
        public SubscriptionRenewalService(CustomerRepository customerRepository, SubscriptionPlanRepository planRepository, RenewalServiceValidator renewalServiceValidator, BillingGatewayAdapter billingGatewayAdapter, DiscountCalculator discountCalculator, FeeCalculator feeCalculator)
        {
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _renewalServiceValidator = renewalServiceValidator;
            _billingGatewayAdapter = billingGatewayAdapter;
            _discountCalculator = discountCalculator;
            _feeCalculator = feeCalculator;
        }
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            _renewalServiceValidator.Validate(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(normalizedPlanCode);

            //here should be discount calculator
            DiscountResult discountResult = _discountCalculator.CalculateDiscount(customer, plan, customerId, normalizedPlanCode, normalizedPaymentMethod, seatCount, useLoyaltyPoints);
            decimal baseAmount = discountResult.BaseAmount;
            decimal discountAmount = discountResult.DiscountAmount;
            string notes = discountResult.Notes;
            decimal subtotalAfterDiscount = discountResult.SubtotalAfterDiscount;

            FeeCalculatorResult feeCalculatorResult = _feeCalculator.CalculateFees(includePremiumSupport, normalizedPlanCode, normalizedPaymentMethod, subtotalAfterDiscount);

            decimal supportFee = feeCalculatorResult.SupportFee;
            decimal paymentFee = feeCalculatorResult.PaymentFee;
            notes += feeCalculatorResult.Notes;

            decimal taxRate = 0.20m;
            if (customer.Country == "Poland")
            {
                taxRate = 0.23m;
            }
            else if (customer.Country == "Germany")
            {
                taxRate = 0.19m;
            }
            else if (customer.Country == "Czech Republic")
            {
                taxRate = 0.21m;
            }
            else if (customer.Country == "Norway")
            {
                taxRate = 0.25m;
            }

            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            _billingGatewayAdapter.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                LegacyBillingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}
