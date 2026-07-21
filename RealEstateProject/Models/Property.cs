using System.ComponentModel.DataAnnotations;

namespace RealEstateProject.Models
{
    public class Property
    {
        [Key]
        public int PropId { get; set; }

        public int? OwnerId { get; set; }
        public Owner? Owner { get; set; }

        public string PropType { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Conditions { get; set; } = string.Empty;
        public string PublishStatus { get; set; } = string.Empty;
        public string ListingStatus { get; set; } = string.Empty;

        public ICollection<Media> MediaFiles { get; set; } = new List<Media>();
        public ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<PropertyApproval> PropertyApprovals { get; set; } = new List<PropertyApproval>();

    }
}
