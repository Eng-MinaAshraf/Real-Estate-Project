using System.ComponentModel.DataAnnotations;

namespace RealEstateProject.Models
{
    public class ServicePartner
    {
        [Key]
        public int PartnerId { get; set; }

        public string CompanyName { get; set; } = string.Empty;
        public string ServiceCategory { get; set; } = string.Empty;
        public decimal? CommissionRate { get; set; }

        public ICollection<ServiceRequest> Service_Requests { get; set; } = new List<ServiceRequest>();

    }
}
