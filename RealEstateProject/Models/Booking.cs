namespace RealEstateProject.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public int? PropId { get; set; }
        public Property? Property { get; set; }

        public DateTime? BookingDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;
    }
}
