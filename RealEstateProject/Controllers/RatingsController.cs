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
    [Authorize]
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ratings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ratings.Include(r => r.Giver).Include(r => r.Receiver);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Ratings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Giver)
                .Include(r => r.Receiver)
                .FirstOrDefaultAsync(m => m.RatingId == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // GET: Ratings/Create
        public IActionResult Create()
        {
            ViewData["GiverId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RatingId,GiverId,ReceiverId,Score,Comment")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rating);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GiverId"] = new SelectList(_context.Users, "UserId", "UserId", rating.GiverId);
            ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "UserId", rating.ReceiverId);
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }
            ViewData["GiverId"] = new SelectList(_context.Users, "UserId", "UserId", rating.GiverId);
            ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "UserId", rating.ReceiverId);
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RatingId,GiverId,ReceiverId,Score,Comment")] Rating rating)
        {
            if (id != rating.RatingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rating);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RatingExists(rating.RatingId))
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
            ViewData["GiverId"] = new SelectList(_context.Users, "UserId", "UserId", rating.GiverId);
            ViewData["ReceiverId"] = new SelectList(_context.Users, "UserId", "UserId", rating.ReceiverId);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .Include(r => r.Giver)
                .Include(r => r.Receiver)
                .FirstOrDefaultAsync(m => m.RatingId == id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating != null)
            {
                _context.Ratings.Remove(rating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RatingExists(int id)
        {
            return _context.Ratings.Any(e => e.RatingId == id);
        }
    }
}
