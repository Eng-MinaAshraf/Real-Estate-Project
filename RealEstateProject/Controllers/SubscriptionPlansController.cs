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
    public class SubscriptionPlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubscriptionPlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubscriptionPlans - صفحة عامة متاحة للجميع (Pricing)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.SubscriptionPlans.ToListAsync());
        }

        // GET: SubscriptionPlans/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionPlan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(m => m.PlanId == id);
            if (subscriptionPlan == null)
            {
                return NotFound();
            }

            return View(subscriptionPlan);
        }

        // GET: SubscriptionPlans/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: SubscriptionPlans/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanId,PlanName,MaxUnits,AllowedUnitTypes,MaxMediaCount,HasFeaturedSearch,HasMonthlyReports,HasAccountManager,MonthlyPrice")] SubscriptionPlan subscriptionPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscriptionPlan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Subscription plan created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(subscriptionPlan);
        }

        // GET: SubscriptionPlans/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionPlan = await _context.SubscriptionPlans.FindAsync(id);
            if (subscriptionPlan == null)
            {
                return NotFound();
            }
            return View(subscriptionPlan);
        }

        // POST: SubscriptionPlans/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlanId,PlanName,MaxUnits,AllowedUnitTypes,MaxMediaCount,HasFeaturedSearch,HasMonthlyReports,HasAccountManager,MonthlyPrice")] SubscriptionPlan subscriptionPlan)
        {
            if (id != subscriptionPlan.PlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscriptionPlan);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Subscription plan updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionPlanExists(subscriptionPlan.PlanId))
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
            return View(subscriptionPlan);
        }

        // GET: SubscriptionPlans/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subscriptionPlan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(m => m.PlanId == id);
            if (subscriptionPlan == null)
            {
                return NotFound();
            }

            return View(subscriptionPlan);
        }

        // POST: SubscriptionPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subscriptionPlan = await _context.SubscriptionPlans.FindAsync(id);
            if (subscriptionPlan != null)
            {
                _context.SubscriptionPlans.Remove(subscriptionPlan);
                TempData["SuccessMessage"] = "Subscription plan deleted.";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionPlanExists(int id)
        {
            return _context.SubscriptionPlans.Any(e => e.PlanId == id);
        }
    }
}