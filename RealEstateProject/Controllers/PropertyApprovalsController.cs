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
    [Authorize(Roles = "Admin")]
    public class PropertyApprovalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RealEstateProject.Services.AlertMatchingService _alertMatching;

        public PropertyApprovalsController(ApplicationDbContext context, RealEstateProject.Services.AlertMatchingService alertMatching)
        {
            _context = context;
            _alertMatching = alertMatching;
        }

        // لما الموافقة تبقى Approved: ننشر العقار ونطابقه مع تنبيهات المستأجرين
        private async Task PublishAndNotifyAsync(int? propId, string? approvalStatus)
        {
            if (propId == null || approvalStatus != "Approved")
            {
                return;
            }

            var property = await _context.Properties.FirstOrDefaultAsync(p => p.PropId == propId);
            if (property == null)
            {
                return;
            }

            bool wasPublished = property.PublishStatus == "Published";
            if (!wasPublished)
            {
                property.PublishStatus = "Published";
                await _context.SaveChangesAsync();
            }

            // نطابق التنبيهات بس أول ما العقار يتنشر
            if (!wasPublished)
            {
                await _alertMatching.NotifyMatchingAlertsAsync(property);
            }
        }

        // GET: PropertyApprovals
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.PropertyApprovals.Include(p => p.Admin).Include(p => p.Property);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: PropertyApprovals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyApproval = await _context.PropertyApprovals
                .Include(p => p.Admin)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(m => m.ApprovalId == id);
            if (propertyApproval == null)
            {
                return NotFound();
            }

            return View(propertyApproval);
        }

        // GET: PropertyApprovals/Create
        public IActionResult Create()
        {
            ViewData["AdminId"] = new SelectList(_context.Admins, "AdminId", "AdminId");
            ViewData["PropId"] = new SelectList(_context.Properties, "PropId", "PropId");
            return View();
        }

        // POST: PropertyApprovals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ApprovalId,AdminId,PropId,ApprovalDate,ApprovalStatus")] PropertyApproval propertyApproval)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyApproval);
                await _context.SaveChangesAsync();
                await PublishAndNotifyAsync(propertyApproval.PropId, propertyApproval.ApprovalStatus);
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "AdminId", "AdminId", propertyApproval.AdminId);
            ViewData["PropId"] = new SelectList(_context.Properties, "PropId", "PropId", propertyApproval.PropId);
            return View(propertyApproval);
        }

        // GET: PropertyApprovals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyApproval = await _context.PropertyApprovals.FindAsync(id);
            if (propertyApproval == null)
            {
                return NotFound();
            }
            ViewData["AdminId"] = new SelectList(_context.Admins, "AdminId", "AdminId", propertyApproval.AdminId);
            ViewData["PropId"] = new SelectList(_context.Properties, "PropId", "PropId", propertyApproval.PropId);
            return View(propertyApproval);
        }

        // POST: PropertyApprovals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApprovalId,AdminId,PropId,ApprovalDate,ApprovalStatus")] PropertyApproval propertyApproval)
        {
            if (id != propertyApproval.ApprovalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyApproval);
                    await _context.SaveChangesAsync();
                    await PublishAndNotifyAsync(propertyApproval.PropId, propertyApproval.ApprovalStatus);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyApprovalExists(propertyApproval.ApprovalId))
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
            ViewData["AdminId"] = new SelectList(_context.Admins, "AdminId", "AdminId", propertyApproval.AdminId);
            ViewData["PropId"] = new SelectList(_context.Properties, "PropId", "PropId", propertyApproval.PropId);
            return View(propertyApproval);
        }

        // GET: PropertyApprovals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyApproval = await _context.PropertyApprovals
                .Include(p => p.Admin)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(m => m.ApprovalId == id);
            if (propertyApproval == null)
            {
                return NotFound();
            }

            return View(propertyApproval);
        }

        // POST: PropertyApprovals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyApproval = await _context.PropertyApprovals.FindAsync(id);
            if (propertyApproval != null)
            {
                _context.PropertyApprovals.Remove(propertyApproval);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyApprovalExists(int id)
        {
            return _context.PropertyApprovals.Any(e => e.ApprovalId == id);
        }
    }
}
