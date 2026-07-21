namespace RealEstateProject.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }

        public int? OwnerId { get; set; }
        public Owner Owner { get; set; }

        public int? PlanId { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string BillingCycle { get; set; }
        public decimal? Discount { get; set; }
        public string PaymentStatus { get; set; }
    }
}
