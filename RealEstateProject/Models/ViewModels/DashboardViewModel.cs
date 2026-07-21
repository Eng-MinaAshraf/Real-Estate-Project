using System.Collections.Generic;

namespace RealEstateProject.Models.ViewModels
{
    /// <summary>
    /// بيانات لوحة التحكم الحقيقية (الإحصائيات + آخر العقارات).
    /// </summary>
    public class DashboardViewModel
    {
        // Summary stats
        public int TotalProperties { get; set; }
        public int ActiveSubscriptions { get; set; }
        public double? AverageRating { get; set; }

        // آخر العقارات (حسب دور المستخدم)
        public List<Property> RecentProperties { get; set; } = new List<Property>();
    }
}
