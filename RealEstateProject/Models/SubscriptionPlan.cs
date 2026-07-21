using System.ComponentModel.DataAnnotations;

namespace RealEstateProject.Models
{
    public class SubscriptionPlan
    {
        [Key]
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public int MaxUnits { get; set; }
        public string AllowedUnitTypes { get; set; }
        public int MaxMediaCount { get; set; }
        public bool? HasFeaturedSearch { get; set; }
        public bool? HasMonthlyReports { get; set; }
        public bool? HasAccountManager { get; set; }

        public decimal? MonthlyPrice { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    }
}
