// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace AqarX.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        // الامتدادات والحجم المسموح بيهم لصورة البروفايل
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxAvatarBytes = 2 * 1024 * 1024; // 2 MB

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _environment = environment;
        }

        public string Username { get; set; }

        // بيتعرض في الـ view (الصورة الحالية + الحروف الأولى كـ fallback)
        public string ProfilePictureUrl { get; set; }
        public string Initials { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First name")]
            [StringLength(50, ErrorMessage = "The {0} must be at most {1} characters long.")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last name")]
            [StringLength(50, ErrorMessage = "The {0} must be at most {1} characters long.")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Address")]
            [StringLength(200, ErrorMessage = "The {0} must be at most {1} characters long.")]
            public string Address { get; set; }

            [Display(Name = "Profile picture")]
            public IFormFile ProfilePicture { get; set; }
        }

        // بيجيب صف الـ User المرتبط بحساب الـ Identity الحالي
        private async Task<User> GetDomainUserAsync(ApplicationUser user)
        {
            if (user.AppUserId == null)
            {
                return null;
            }

            return await _context.Users.FindAsync(user.AppUserId.Value);
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var domainUser = await GetDomainUserAsync(user);

            Username = userName;
            ProfilePictureUrl = domainUser?.ProfilePictureUrl;

            var first = domainUser?.Fname ?? string.Empty;
            var last = domainUser?.Lname ?? string.Empty;
            Initials = $"{(first.Length > 0 ? first[0] : ' ')}{(last.Length > 0 ? last[0] : ' ')}".Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(Initials))
            {
                Initials = "U";
            }

            Input = new InputModel
            {
                FirstName = domainUser?.Fname,
                LastName = domainUser?.Lname,
                PhoneNumber = phoneNumber,
                Address = domainUser?.Address
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            // تحديث رقم التليفون عن طريق Identity
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Error: Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // تحديث بيانات الـ User الأساسية (الاسم/العنوان/الصورة)
            var domainUser = await GetDomainUserAsync(user);
            if (domainUser != null)
            {
                domainUser.Fname = Input.FirstName;
                domainUser.Lname = Input.LastName;
                domainUser.Address = Input.Address;

                if (Input.ProfilePicture != null && Input.ProfilePicture.Length > 0)
                {
                    var uploadResult = await SaveProfilePictureAsync(domainUser);
                    if (uploadResult != null)
                    {
                        // فيه خطأ في الرفع
                        ModelState.AddModelError("Input.ProfilePicture", uploadResult);
                        await LoadAsync(user);
                        return Page();
                    }
                }

                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated.";
            return RedirectToPage();
        }

        // بترفع صورة البروفايل وبترجع رسالة خطأ لو فيه مشكلة، أو null لو نجحت
        private async Task<string> SaveProfilePictureAsync(User domainUser)
        {
            var file = Input.ProfilePicture;
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                return "Only JPG, PNG or WEBP images are allowed.";
            }

            if (file.Length > MaxAvatarBytes)
            {
                return "The image must be 2 MB or smaller.";
            }

            var webRoot = _environment.WebRootPath
                ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsDir = Path.Combine(webRoot, "uploads", "avatars");
            Directory.CreateDirectory(uploadsDir);

            // اسم فريد لكل يوزر عشان نتجنب تضارب الأسماء
            var fileName = $"user_{domainUser.UserId}_{Guid.NewGuid():N}{extension}";
            var absolutePath = Path.Combine(uploadsDir, fileName);

            // نمسح الصورة القديمة لو موجودة
            if (!string.IsNullOrEmpty(domainUser.ProfilePictureUrl))
            {
                var oldPath = Path.Combine(webRoot, domainUser.ProfilePictureUrl.TrimStart('/', '\\')
                    .Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(oldPath))
                {
                    try { System.IO.File.Delete(oldPath); } catch { /* نتجاهل لو المسح فشل */ }
                }
            }

            using (var stream = new FileStream(absolutePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            domainUser.ProfilePictureUrl = $"/uploads/avatars/{fileName}";
            return null;
        }
    }
}
