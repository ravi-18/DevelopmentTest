using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevelopmentTest.Models;

namespace DevelopmentTest.Controllers
{
    public class SportEventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SportEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SportEvents
        public async Task<IActionResult> Index()
        {
              return _context.SportEvents != null ? 
                          View(await _context.SportEvents.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.SportEvents'  is null.");
        }

        // GET: SportEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SportEvents == null)
            {
                return NotFound();
            }

            var sportEvent = await _context.SportEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportEvent == null)
            {
                return NotFound();
            }

            return View(sportEvent);
        }

        // GET: SportEvents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SportEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventDate,EventName,EventType")] SportEvent sportEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sportEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sportEvent);
        }

        // GET: SportEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SportEvents == null)
            {
                return NotFound();
            }

            var sportEvent = await _context.SportEvents.FindAsync(id);
            if (sportEvent == null)
            {
                return NotFound();
            }
            return View(sportEvent);
        }

        // POST: SportEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventDate,EventName,EventType")] SportEvent sportEvent)
        {
            if (id != sportEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sportEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SportEventExists(sportEvent.Id))
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
            return View(sportEvent);
        }

        // GET: SportEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SportEvents == null)
            {
                return NotFound();
            }

            var sportEvent = await _context.SportEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sportEvent == null)
            {
                return NotFound();
            }

            return View(sportEvent);
        }

        // POST: SportEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SportEvents == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SportEvents'  is null.");
            }
            var sportEvent = await _context.SportEvents.FindAsync(id);
            if (sportEvent != null)
            {
                _context.SportEvents.Remove(sportEvent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SportEventExists(int id)
        {
          return (_context.SportEvents?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
