using System.ComponentModel.DataAnnotations;

namespace RealEstateProject.Models
{
    public class ServiceRequest
    {
        [Key]
        public int RequestId { get; set; }

        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public int? PartnerId { get; set; }
        public ServicePartner? ServicePartner { get; set; }

        public DateTime? RequestDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
