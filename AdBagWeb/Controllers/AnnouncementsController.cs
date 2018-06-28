using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdBagWeb.Models;

namespace AdBagWeb.Controllers
{
    public class AnnouncementsController : Controller
    {
        private readonly AdBagWebDBContext _context;

        public AnnouncementsController(AdBagWebDBContext context)
        {
            _context = context;
        }

        // GET: Announcements
        public async Task<IActionResult> Index()
        {
            var adBagWebDBContext = _context.Announcement.Include(a => a.IdCategoryNavigation).Include(a => a.IdUserNavigation);
            return View(await adBagWebDBContext.ToListAsync());
        }

        // GET: Announcements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcement
                .Include(a => a.IdCategoryNavigation)
                .Include(a => a.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdAnnouncement == id);
            if (announcement == null)
            {
                return NotFound();
            }

            return View(announcement);
        }

        // GET: Announcements/Create
        public IActionResult Create()
        {
            ViewData["IdCategory"] = new SelectList(_context.Category, "IdCategory", "Name");
            ViewData["IdUser"] = new SelectList(_context.User, "IdUser", "Email");
            return View();
        }

        // POST: Announcements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAnnouncement,Title,Description,UploadDate,ExpirationDate,IdUser,IdCategory")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(announcement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategory"] = new SelectList(_context.Category, "IdCategory", "Name", announcement.IdCategory);
            ViewData["IdUser"] = new SelectList(_context.User, "IdUser", "Email", announcement.IdUser);
            return View(announcement);
        }

        // GET: Announcements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcement.FindAsync(id);
            if (announcement == null)
            {
                return NotFound();
            }
            ViewData["IdCategory"] = new SelectList(_context.Category, "IdCategory", "Name", announcement.IdCategory);
            ViewData["IdUser"] = new SelectList(_context.User, "IdUser", "Email", announcement.IdUser);
            return View(announcement);
        }

        // POST: Announcements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAnnouncement,Title,Description,UploadDate,ExpirationDate,IdUser,IdCategory")] Announcement announcement)
        {
            if (id != announcement.IdAnnouncement)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(announcement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnnouncementExists(announcement.IdAnnouncement))
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
            ViewData["IdCategory"] = new SelectList(_context.Category, "IdCategory", "Name", announcement.IdCategory);
            ViewData["IdUser"] = new SelectList(_context.User, "IdUser", "Email", announcement.IdUser);
            return View(announcement);
        }

        // GET: Announcements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var announcement = await _context.Announcement
                .Include(a => a.IdCategoryNavigation)
                .Include(a => a.IdUserNavigation)
                .FirstOrDefaultAsync(m => m.IdAnnouncement == id);
            if (announcement == null)
            {
                return NotFound();
            }

            return View(announcement);
        }

        // POST: Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var announcement = await _context.Announcement.FindAsync(id);
            _context.Announcement.Remove(announcement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnnouncementExists(int id)
        {
            return _context.Announcement.Any(e => e.IdAnnouncement == id);
        }
    }
}
