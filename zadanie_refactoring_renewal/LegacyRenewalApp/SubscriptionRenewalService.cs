using LegacyRenewalApp.Enums;
using LegacyRenewalApp.Helper;
using LegacyRenewalApp.Interfaves;
using LegacyRenewalApp.Models;
using LegacyRenewalApp.Repositories;
using System;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IRenewalServiceValidator _renewalServiceValidator;
        private readonly IBillingGateway _billingGatewayAdapter;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly IFeeCalculator _feeCalculator;
        private readonly ITaxCalculator _taxCalculator;

        public SubscriptionRenewalService() : this(new CustomerRepository(), new SubscriptionPlanRepository(), new RenewalServiceValidator(), new BillingGatewayAdapter(), new DiscountCalculator(), new FeeCalculator(), new TaxCalculator())
        { }
        public SubscriptionRenewalService(ICustomerRepository customerRepository, ISubscriptionPlanRepository planRepository, IRenewalServiceValidator renewalServiceValidator, IBillingGateway billingGatewayAdapter, IDiscountCalculator discountCalculator, IFeeCalculator feeCalculator, ITaxCalculator taxCalculator)
        {
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _renewalServiceValidator = renewalServiceValidator;
            _billingGatewayAdapter = billingGatewayAdapter;
            _discountCalculator = discountCalculator;
            _feeCalculator = feeCalculator;
            _taxCalculator = taxCalculator;
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

            DiscountResult discountResult = _discountCalculator.CalculateDiscount(customer, plan, customerId, normalizedPlanCode, normalizedPaymentMethod, seatCount, useLoyaltyPoints);
            decimal baseAmount = discountResult.BaseAmount;
            decimal discountAmount = discountResult.DiscountAmount;
            string notes = discountResult.Notes;
            decimal subtotalAfterDiscount = discountResult.SubtotalAfterDiscount;

            FeeCalculatorResult feeResult = _feeCalculator.CalculateFees(includePremiumSupport, normalizedPlanCode, normalizedPaymentMethod, subtotalAfterDiscount);
            decimal supportFee = feeResult.SupportFee;
            decimal paymentFee = feeResult.PaymentFee;
            notes += feeResult.Notes;

            TaxCalculatorResult taxCalculatorResult = _taxCalculator.calculateTax(customer.Country, subtotalAfterDiscount, supportFee, paymentFee); 
            decimal taxBase = taxCalculatorResult.TaxBase;
            decimal taxAmount = taxCalculatorResult.TaxAmount;
            decimal finalAmount = taxCalculatorResult.FinalAmount;
            notes += taxCalculatorResult.Notes;

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
