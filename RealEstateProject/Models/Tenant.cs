using System.Collections.Generic;


namespace RealEstateProject.Models
{
    public class Tenant
    {
        public int TenantId { get; set; }

        public string SmokingStatus { get; set; } = string.Empty;
        public string Personality { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;

        // Navigation Property
        public User? User { get; set; }

        // Navigation Collections
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

    }
}
