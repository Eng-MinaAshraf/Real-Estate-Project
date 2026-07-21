using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;
using RealEstateProject.Models;
using Microsoft.AspNetCore.Authorization;


namespace AqarX.Controllers
{
    [Authorize(Roles = "Tenant")]
    public class AlertsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // بيرجع TenantId بتاع اليوزر الحالي (في السكيمة دي TenantId == UserId)
        private async Task<int?> GetCurrentTenantIdAsync()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return null;

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (appUser == null) return null;

            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.TenantId == appUser.UserId);
            return tenant?.TenantId;
        }

        // GET: Alerts — تنبيهات المستأجر الحالي بس
        public async Task<IActionResult> Index()
        {
            var tenantId = await GetCurrentTenantIdAsync();
            if (tenantId == null)
            {
                return Forbid();
            }

            var alerts = await _context.Alerts
                .Where(a => a.TenantId == tenantId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return View(alerts);
        }

        // GET: Alerts/Create — ممكن يتعبّى مسبقًا من فلاتر البحث
        public IActionResult Create(string? propertyType, string? purpose, string? location, decimal? minPrice, decimal? maxPrice)
        {
            var alert = new Alert
            {
                PropertyType = propertyType ?? string.Empty,
                Purpose = purpose ?? string.Empty,
                Location = location ?? string.Empty,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                IsActive = true
            };

            return View(alert);
        }

        // POST: Alerts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyType,Purpose,Location,MinPrice,MaxPrice")] Alert alert)
        {
            var tenantId = await GetCurrentTenantIdAsync();
            if (tenantId == null)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // القيم الحساسة تتحدد سيرفر-سايد ومتتاخدش من الفورم
                alert.TenantId = tenantId;
                alert.CreatedAt = DateTime.UtcNow;
                alert.IsActive = true;

                _context.Add(alert);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Property alert created. We'll notify you when a match is published.";
                return RedirectToAction(nameof(Index));
            }

            return View(alert);
        }

        // GET: Alerts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantId = await GetCurrentTenantIdAsync();
            if (tenantId == null)
            {
                return Forbid();
            }

            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            // التأكد من الملكية سيرفر-سايد
            if (alert.TenantId != tenantId)
            {
                return Forbid();
            }

            return View(alert);
        }

        // POST: Alerts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlertId,PropertyType,Purpose,Location,MinPrice,MaxPrice,IsActive")] Alert alert)
        {
            if (id != alert.AlertId)
            {
                return NotFound();
            }

            var tenantId = await GetCurrentTenantIdAsync();
            if (tenantId == null)
            {
                return Forbid();
            }

            // نجيب النسخة الأصلية عشان نتأكد من الملكية ونحافظ على الحقول اللي مش في الفورم
            var existing = await _context.Alerts.AsNoTracking().FirstOrDefaultAsync(a => a.AlertId == id);
            if (existing == null)
            {
                return NotFound();
            }

            if (existing.TenantId != tenantId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                // نمنع أي تلاعب في الملكية أو تاريخ الإنشاء
                alert.TenantId = existing.TenantId;
                alert.CreatedAt = existing.CreatedAt;

                try
                {
                    _context.Update(alert);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Alert updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlertExists(alert.AlertId))
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

            return View(alert);
        }

        // POST: Alerts/ToggleActive/5 — تفعيل/إيقاف التنبيه
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var tenantId = await GetCurrentTenantIdAsync();
            if (tenantId == null)
            {
                return Forbid();
            }

            var alert = await _context.Alerts.FindAsync(id);
            if (alert == null)
            {
                return NotFound();
            }

            if (alert.TenantId != tenantId)
            {
                return Forbid();
            }

            alert.IsActive = !alert.IsActive;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = alert.IsActive ? "Alert activated." : "Alert deactivated.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Alerts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tenantId = await GetCurrentTenantIdAsync();
            if (tenantId == null)
            {
                return Forbid();
            }

            var alert = await _context.Alerts.FirstOrDefaultAsync(m => m.AlertId == id);
            if (alert == null)
            {
                return NotFound();
            }

            if (alert.TenantId != tenantId)
            {
                return Forbid();
            }

            return View(alert);
        }

        // POST: Alerts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tenantId = await GetCurrentTenantIdAsync();
            if (tenantId == null)
            {
                return Forbid();
            }

            var alert = await _context.Alerts.FindAsync(id);
            if (alert != null)
            {
                // التأكد من الملكية قبل الحذف
                if (alert.TenantId != tenantId)
                {
                    return Forbid();
                }

                _context.Alerts.Remove(alert);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Alert deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AlertExists(int id)
        {
            return _context.Alerts.Any(e => e.AlertId == id);
        }
    }
}
