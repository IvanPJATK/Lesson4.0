using LegacyRenewalApp.Enums;

namespace LegacyRenewalApp.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public SegmentEnum Segment { get; set; }
        public CountryEnum Country { get; set; }
        public int YearsWithCompany { get; set; }
        public int LoyaltyPoints { get; set; }
        public bool IsActive { get; set; }
    }
}
