using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace RealEstateProject.Models
{
    public class PropertyApproval
    {
        [Key]
        public int ApprovalId { get; set; }

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        public int? PropId { get; set; }
        public Property? Property { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string ApprovalStatus { get; set; } = string.Empty;
    }
}
