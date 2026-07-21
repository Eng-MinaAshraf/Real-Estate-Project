using System;
using Microsoft.AspNetCore.Identity;

namespace RealEstateProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? AppUserId { get; set; }
        public User? AppUser { get; set; }

        // بيتسجّل فيه آخر مرة اليوزر غيّر الباسورد (Identity مابيتابعوش افتراضيًا)
        public DateTime? LastPasswordChangeDate { get; set; }
    }
}