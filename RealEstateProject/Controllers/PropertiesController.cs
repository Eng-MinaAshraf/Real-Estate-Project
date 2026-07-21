using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;
using RealEstateProject.Models;

namespace AqarX.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly RealEstateProject.Services.AlertMatchingService _alertMatching;

        public PropertiesController(ApplicationDbContext context, IWebHostEnvironment env, RealEstateProject.Services.AlertMatchingService alertMatching)
        {
            _context = context;
            _env = env;
            _alertMatching = alertMatching;
        }

        // بيرجع OwnerId بتاع اليوزر الحالي لو هو Owner، وإلا null
        private async Task<int?> GetCurrentOwnerIdAsync()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return null;

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (appUser == null) return null;

            var owner = await _context.Owners.FirstOrDefaultAsync(o => o.OwnerId == appUser.UserId);
            return owner?.OwnerId;
        }

        private bool IsAdmin() => User.IsInRole("Admin");

        // GET: Properties — مع فلاتر بحث اختيارية
        public async Task<IActionResult> Index(string? propType, string? purpose, string? location, decimal? minPrice, decimal? maxPrice)
        {
            // بنمرر الـ OwnerId الحالي للـ View عشان يظهر أزرار Edit/Delete للمالك بس
            ViewBag.CurrentOwnerId = await GetCurrentOwnerIdAsync();

            var query = _context.Properties
                .Include(p => p.Owner).ThenInclude(o => o.User)
                .Include(p => p.MediaFiles)
                .AsQueryable();

            // نطبّق الفلاتر لو المستخدم أدخل قيم
            if (!string.IsNullOrWhiteSpace(propType))
            {
                query = query.Where(p => p.PropType == propType);
            }
            if (!string.IsNullOrWhiteSpace(purpose))
            {
                query = query.Where(p => p.Purpose == purpose);
            }
            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(p => p.Location.Contains(location));
            }
            if (minPrice != null)
            {
                query = query.Where(p => p.Price >= minPrice);
            }
            if (maxPrice != null)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            // نمرر قيم الفلاتر للـ View عشان تفضل ظاهرة في الفورم وتتمرر لزر التنبيه
            ViewBag.FilterPropType = propType;
            ViewBag.FilterPurpose = purpose;
            ViewBag.FilterLocation = location;
            ViewBag.FilterMinPrice = minPrice;
            ViewBag.FilterMaxPrice = maxPrice;
            ViewBag.HasSearch = !string.IsNullOrWhiteSpace(propType)
                                || !string.IsNullOrWhiteSpace(purpose)
                                || !string.IsNullOrWhiteSpace(location)
                                || minPrice != null
                                || maxPrice != null;

            return View(await query.ToListAsync());
        }

        // GET: Properties/MyProperties
        // بيعرض عقارات الـ Owner الحالي بس (My Properties في لوحة تحكم المالك)
        public async Task<IActionResult> MyProperties()
        {
            var currentOwnerId = await GetCurrentOwnerIdAsync();
            if (currentOwnerId == null)
            {
                // مش Owner (زائر/Tenant/Admin من غير سجل Owner) => ممنوع
                return Forbid();
            }

            ViewBag.CurrentOwnerId = currentOwnerId;

            var myProperties = await _context.Properties
                .Include(p => p.Owner).ThenInclude(o => o.User)
                .Include(p => p.MediaFiles)
                .Where(p => p.OwnerId == currentOwnerId) // الفلترة دايمًا على UserId بتاع المالك الحالي
                .ToListAsync();

            return View(myProperties);
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Owner).ThenInclude(o => o.User)
                .Include(p => p.MediaFiles)
                .FirstOrDefaultAsync(m => m.PropId == id);
            if (property == null)
            {
                return NotFound();
            }

            // بنمرر الـ OwnerId الحالي للـ View عشان يظهر أزرار Edit/Delete للمالك بس
            ViewBag.CurrentOwnerId = await GetCurrentOwnerIdAsync();

            return View(property);
        }

        // GET: Properties/Create
        public async Task<IActionResult> Create()
        {
            var currentOwnerId = await GetCurrentOwnerIdAsync();

            if (IsAdmin())
            {
                // الأدمن يقدر يختار أي Owner
                ViewData["OwnerId"] = new SelectList(_context.Owners.Include(o => o.User), "OwnerId", "User.Email");
            }
            else if (currentOwnerId != null)
            {
                // الـ Owner يشوف نفسه بس كخيار وحيد
                ViewData["OwnerId"] = new SelectList(
                    _context.Owners.Where(o => o.OwnerId == currentOwnerId).Include(o => o.User),
                    "OwnerId", "User.Email", currentOwnerId);
            }
            else
            {
                return Forbid();
            }

            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PropId,OwnerId,PropType,Purpose,Price,Location,Conditions,PublishStatus,ListingStatus")] Property property,
            List<IFormFile> Photos)
        {
            var currentOwnerId = await GetCurrentOwnerIdAsync();

            // لو مش Admin، لازم العقار يبقى منسوب للـ Owner بتاع اليوزر الحالي بس (منع أي تلاعب في القيمة المرسلة)
            if (!IsAdmin())
            {
                if (currentOwnerId == null) return Forbid();
                property.OwnerId = currentOwnerId;
            }

            if (ModelState.IsValid)
            {
                _context.Add(property);
                await _context.SaveChangesAsync();

                if (Photos != null && Photos.Count > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "properties");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var photo in Photos)
                    {
                        if (photo.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(photo.FileName);
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await photo.CopyToAsync(stream);
                            }

                            _context.Media.Add(new Media
                            {
                                PropId = property.PropId,
                                MediaType = "Image",
                                MediaUrl = "/uploads/properties/" + uniqueFileName
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                // لو العقار اتنشر مباشرةً، نطابقه مع تنبيهات المستأجرين
                if (property.PublishStatus == "Published")
                {
                    await _alertMatching.NotifyMatchingAlertsAsync(property);
                }

                TempData["SuccessMessage"] = "Property listed successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "OwnerId", property.OwnerId);
            return View(property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return NotFound();
            }

            // منع أي Owner من تعديل عقار مش بتاعه
            if (!IsAdmin())
            {
                var currentOwnerId = await GetCurrentOwnerIdAsync();
                if (currentOwnerId == null || property.OwnerId != currentOwnerId)
                {
                    return Forbid();
                }
            }

            ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "OwnerId", property.OwnerId);
            return View(property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropId,OwnerId,PropType,Purpose,Price,Location,Conditions,PublishStatus,ListingStatus")] Property property)
        {
            if (id != property.PropId)
            {
                return NotFound();
            }

            // نجيب النسخة الأصلية من الداتابيز عشان نتأكد مين مالكها الحقيقي (منع تلاعب في الفورم)
            var existingProperty = await _context.Properties.AsNoTracking().FirstOrDefaultAsync(p => p.PropId == id);
            if (existingProperty == null)
            {
                return NotFound();
            }

            if (!IsAdmin())
            {
                var currentOwnerId = await GetCurrentOwnerIdAsync();
                if (currentOwnerId == null || existingProperty.OwnerId != currentOwnerId)
                {
                    return Forbid();
                }
                // منع تغيير الـ OwnerId حتى لو حد لعب في الفورم يدوي
                property.OwnerId = existingProperty.OwnerId;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(property);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Property updated successfully.";

                    // نطابق التنبيهات بس لما العقار ينتقل لحالة Published (مش كان منشور قبل كده)
                    if (property.PublishStatus == "Published" && existingProperty.PublishStatus != "Published")
                    {
                        await _alertMatching.NotifyMatchingAlertsAsync(property);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(property.PropId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Owners, "OwnerId", "OwnerId", property.OwnerId);
            return View(property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var property = await _context.Properties
                .Include(p => p.Owner).ThenInclude(o => o.User)
                .FirstOrDefaultAsync(m => m.PropId == id);
            if (property == null)
            {
                return NotFound();
            }

            if (!IsAdmin())
            {
                var currentOwnerId = await GetCurrentOwnerIdAsync();
                if (currentOwnerId == null || property.OwnerId != currentOwnerId)
                {
                    return Forbid();
                }
            }

            return View(property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property != null)
            {
                if (!IsAdmin())
                {
                    var currentOwnerId = await GetCurrentOwnerIdAsync();
                    if (currentOwnerId == null || property.OwnerId != currentOwnerId)
                    {
                        return Forbid();
                    }
                }
                _context.Properties.Remove(property);
                TempData["SuccessMessage"] = "Property deleted.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.PropId == id);
        }
    }
}