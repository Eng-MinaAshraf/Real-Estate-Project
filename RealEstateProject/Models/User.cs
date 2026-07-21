
using System.Collections.Generic;

namespace RealEstateProject.Models
{
    public class User
    {
      
            public int UserId { get; set; }
            public string Fname { get; set; }
            public string Lname { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string Phone { get; set; }
            public string Role { get; set; }

            // بيانات البروفايل الإضافية
            public string? Address { get; set; }
            public string? ProfilePictureUrl { get; set; }

            // One-to-One
            public Tenant Tenant { get; set; }
            public Owner Owner { get; set; }
            public Admin Admin { get; set; }

            // Ratings
            public ICollection<Rating> RatingsGiven { get; set; } = new List<Rating>();
            public ICollection<Rating> RatingsReceived { get; set; } = new List<Rating>();

            // Notifications
            public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        }
    }



