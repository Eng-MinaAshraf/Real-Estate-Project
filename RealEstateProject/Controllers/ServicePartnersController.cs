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
    public class ServicePartnersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicePartnersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServicePartners
        public async Task<IActionResult> Index()
        {
            return View(await _context.ServicePartners.ToListAsync());
        }

        // GET: ServicePartners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePartner = await _context.ServicePartners
                .FirstOrDefaultAsync(m => m.PartnerId == id);
            if (servicePartner == null)
            {
                return NotFound();
            }

            return View(servicePartner);
        }

        // GET: ServicePartners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServicePartners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PartnerId,CompanyName,ServiceCategory,CommissionRate")] ServicePartner servicePartner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicePartner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(servicePartner);
        }

        // GET: ServicePartners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePartner = await _context.ServicePartners.FindAsync(id);
            if (servicePartner == null)
            {
                return NotFound();
            }
            return View(servicePartner);
        }

        // POST: ServicePartners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartnerId,CompanyName,ServiceCategory,CommissionRate")] ServicePartner servicePartner)
        {
            if (id != servicePartner.PartnerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicePartner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicePartnerExists(servicePartner.PartnerId))
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
            return View(servicePartner);
        }

        // GET: ServicePartners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePartner = await _context.ServicePartners
                .FirstOrDefaultAsync(m => m.PartnerId == id);
            if (servicePartner == null)
            {
                return NotFound();
            }

            return View(servicePartner);
        }

        // POST: ServicePartners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicePartner = await _context.ServicePartners.FindAsync(id);
            if (servicePartner != null)
            {
                _context.ServicePartners.Remove(servicePartner);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServicePartnerExists(int id)
        {
            return _context.ServicePartners.Any(e => e.PartnerId == id);
        }
    }
}
