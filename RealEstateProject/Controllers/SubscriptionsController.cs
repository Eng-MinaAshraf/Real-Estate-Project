using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealEstateProject.Data;
using RealEstateProject.Models;
using Microsoft.AspNetCore.Authorization;


namespace AqarX.Controllers
{
    [Authorize(Roles = "Admin,Owner")]
    public class SubscriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // بيتأكد إذا كان اليوزر الحالي Admin
        private bool IsAdmin() => User.IsInRole("Admin");

        private async Task<int?> GetCurrentOwnerIdAsync()
        {
            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email)) return null;

            var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (appUser == null) return null;

            var owner = await _context.Owners.FirstOrDefaultAsync(o => o.OwnerId == appUser.UserId);
            return owner?.OwnerId;
        }

        // GET: Subscriptions (My Subscriptions / All for Admin)
        public async Task<IActionResult> Index()
        {
            var query = _context.Subscriptions
                .Include(s => s.Owner)
                .Include(s => s.SubscriptionPlan)
                .AsQueryable();

            // الأدمن بيشوف كل الاشتراكات، والمالك بيشوف اشتراكاته بس
            if (!IsAdmin())
            {
                var currentOwnerId = await GetCurrentOwnerIdAsync();
                if (currentOwnerId == null) return Forbid();
                query = query.Where(s => s.OwnerId == currentOwnerId);
            }

            query = query.OrderByDescending(s => s.SubscriptionId);

            return View(await query.ToListAsync());
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Owner)
                .Include(s => s.SubscriptionPlan)
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);
            if (subscription == null)
            {
                return NotFound();
            }

            if (!IsAdmin())
            {
                var currentOwnerId = await GetCurrentOwnerIdAsync();
                if (subscription.OwnerId != currentOwnerId)
                {
                    return Forbid();
                }
            }

            return View(subscription);
        }

        // GET: Subscriptions/Create?planId=2
        public async Task<IActionResult> Create(int? planId)
        {
            if (planId == null)
            {
                return RedirectToAction("Index", "SubscriptionPlans");
            }

            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null)
            {
                return NotFound();
            }

            ViewBag.Plan = plan;
            return View();
        }

        // POST: Subscriptions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int planId, string billingCycle, string cardNumber, string cardName, string expiry, string cvv)
        {
            var currentOwnerId = await GetCurrentOwnerIdAsync();
            if (currentOwnerId == null) return Forbid();

            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null) return NotFound();

            if (billingCycle != "Monthly" && billingCycle != "Annual")
            {
                ModelState.AddModelError(string.Empty, "Please select a valid billing cycle.");
                ViewBag.Plan = plan;
                return View();
            }

            // بيانات الدفع شكلية فقط للعرض (Demo) - مبنخزنهاش
            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(cardName)
                || string.IsNullOrWhiteSpace(expiry) || string.IsNullOrWhiteSpace(cvv))
            {
                ModelState.AddModelError(string.Empty, "Please fill in all payment details.");
                ViewBag.Plan = plan;
                return View();
            }

            var startDate = DateTime.Now;
            var endDate = billingCycle == "Annual" ? startDate.AddYears(1) : startDate.AddMonths(1);
            var discount = billingCycle == "Annual" ? 20 : 0;

            var subscription = new Subscription
            {
                OwnerId = currentOwnerId,
                PlanId = planId,
                StartDate = startDate,
                EndDate = endDate,
                BillingCycle = billingCycle,
                Discount = discount,
                PaymentStatus = "Paid"
            };

            _context.Add(subscription);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Subscribed to {plan.PlanName} successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            if (!IsAdmin())
            {
                var currentOwnerId = await GetCurrentOwnerIdAsync();
                if (subscription.OwnerId != currentOwnerId)
                {
                    return Forbid();
                }
            }

            ViewData["PlanId"] = new SelectList(_context.SubscriptionPlans, "PlanId", "PlanName", subscription.PlanId);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubscriptionId,OwnerId,PlanId,StartDate,EndDate,BillingCycle,Discount,PaymentStatus")] Subscription subscription)
        {
            if (id != subscription.SubscriptionId)
            {
                return NotFound();
            }

            var existing = await _context.Subscriptions.AsNoTracking().FirstOrDefaultAsync(s => s.SubscriptionId == id);
            if (existing == null) return NotFound();

            if (IsAdmin())
            {
                // الأدمن بيعدل اشتراك أي مالك، فبنحافظ على المالك الأصلي
                subscription.OwnerId = existing.OwnerId;
            }
            else
            {
                var currentOwnerId = await GetCurrentOwnerIdAsync();
                if (existing.OwnerId != currentOwnerId)
                {
                    return Forbid();
                }
                subscription.OwnerId = currentOwnerId;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Subscription updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.SubscriptionId))
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
            ViewData["PlanId"] = new SelectList(_context.SubscriptionPlans, "PlanId", "PlanName", subscription.PlanId);
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Owner)
                .Include(s => s.SubscriptionPlan)
                .FirstOrDefaultAsync(m => m.SubscriptionId == id);
            if (subscription == null)
            {
                return NotFound();
            }

            if (!IsAdmin())
            {
                var currentOwnerId = await GetCurrentOwnerIdAsync();
                if (subscription.OwnerId != currentOwnerId)
                {
                    return Forbid();
                }
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                if (!IsAdmin())
                {
                    var currentOwnerId = await GetCurrentOwnerIdAsync();
                    if (subscription.OwnerId != currentOwnerId)
                    {
                        return Forbid();
                    }
                }
                _context.Subscriptions.Remove(subscription);
                TempData["SuccessMessage"] = "Subscription cancelled.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(int id)
        {
            return _context.Subscriptions.Any(e => e.SubscriptionId == id);
        }
    }
}