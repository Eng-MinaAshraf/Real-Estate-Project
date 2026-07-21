using System;

namespace RealEstateProject.Models
{
    public class Alert
    {
        public int AlertId { get; set; }

        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        // معايير البحث المحفوظة
        public string PropertyType { get; set; } = string.Empty;
        public string Purpose { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        // السعر: MinPrice اختياري، MaxPrice مطلوب للمطابقة
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
