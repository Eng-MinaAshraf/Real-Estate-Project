namespace RealEstateProject.Models
{
    public class Owner
    {
        public int OwnerId { get; set; }

        public User? User { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<Property> Properties { get; set; } = new List<Property>();
    
}
}
