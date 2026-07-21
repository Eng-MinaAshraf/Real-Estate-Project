using System.ComponentModel.DataAnnotations;

namespace RealEstateProject.Models
{
    public class Promotion
    {
        [Key]
        public int PromoId { get; set; }
        public int? PropId { get; set; }
        public Property? Property { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? PaidAmount { get; set; }
    }
}
