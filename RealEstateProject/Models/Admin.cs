namespace RealEstateProject.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        public User? User { get; set; }

        public ICollection<PropertyApproval> PropertyApprovals { get; set; } = new List<PropertyApproval>();

    }
}
