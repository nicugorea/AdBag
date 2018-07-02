using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdBagWeb.Models;
using AdBagWeb.Classes;
using AdBagWeb.ViewModels;
using System.IO;

namespace AdBagWeb.Controllers
{
    public class AnnouncementsController : Controller
    {
        private readonly AdBagWebDBContext _context;

        public AnnouncementsController(AdBagWebDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

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
            var commentsList = _context.Comment.Include(c => c.IdAnnouncementNavigation).Include(u => u.IdUserNavigation).ToList();

            ViewBag.Comments = commentsList;
            var model = new AdComment { Ad = announcement, Comment = "" };
            return View(model);
        }

        [HttpPost]
        public IActionResult Details(int id, string comment)
        {
            Models.Comment dbComment = new Comment();
            dbComment.IdUser = Authentication.Instance.GetId().Value;
            dbComment.IdAnnouncement = id;
            dbComment.PostTime = DateTime.Now;
            dbComment.Text = comment;

            _context.Add(dbComment);
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = id });
        }

        public IActionResult Create()
        {
            ViewData["IdCategory"] = new SelectList(_context.Category, "IdCategory", "Name");
            ViewData["IdUser"] = new SelectList(_context.User, "IdUser", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementViewModel announcement)
        {
            announcement.Ad.UploadDate = DateTime.Now;
            announcement.Ad.ExpirationDate = announcement.Ad.UploadDate;
            announcement.Ad.ExpirationDate.AddDays(7);
            announcement.Ad.IdUser = Authentication.Instance.GetId().Value;

            if (ModelState.IsValid)
            {

                using (var memoryStream = new MemoryStream())
                {
                    await announcement.Image.CopyToAsync(memoryStream);
                    var image = new ImageFile();
                    image.BinaryData = memoryStream.ToArray();
                    image.Extension = "png";
                    image.Name = announcement.Ad.IdAnnouncement.ToString() + announcement.Ad.UploadDate.ToString();
                    _context.Add(image);
                    _context.SaveChanges();
                    var img = _context.ImageFile.First(i => i.Name == (announcement.Ad.IdAnnouncement.ToString() + announcement.Ad.UploadDate.ToString()));
                    announcement.Ad.IdImage = img.IdImage;
                    _context.Announcement.Add(announcement.Ad);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategory"] = new SelectList(_context.Category, "IdCategory", "Name", announcement.Ad.IdCategory);
            ViewData["IdUser"] = new SelectList(_context.User, "IdUser", "Email", announcement.Ad.IdUser);

            return View(announcement);

        }

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


        public IActionResult List(string sortBy)
        {

            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortBy) ? "title_desc" : "";
            ViewBag.DateSortParm = sortBy == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.CategorySortParm = sortBy == "category_asc" ? "category_desc" : "category_asc";
            ViewBag.UserSortParm = sortBy == "user_asc" ? "user_desc" : "user_asc";


            var adList = _context.Announcement.Include(a => a.IdCategoryNavigation).Include(a => a.IdUserNavigation).Include(a=>a.IdImageNavigation).ToList();




            switch (sortBy)
            {
                case "title_desc":
                    adList = adList.OrderByDescending(a => a.Title).ToList();
                    break;
                case "date_asc":
                    adList = adList.OrderBy(a => a.UploadDate).ToList();
                    break;
                case "date_desc":
                    adList = adList.OrderByDescending(a => a.UploadDate).ToList();
                    break;
                case "category_asc":
                    adList = adList.OrderBy(a => a.IdCategoryNavigation.Name).ToList();
                    break;
                case "category_desc":
                    adList = adList.OrderByDescending(a => a.IdCategoryNavigation.Name).ToList();
                    break;
                case "user_asc":
                    adList = adList.OrderBy(a => a.IdUserNavigation.Username).ToList();
                    break;
                case "user_desc":
                    adList = adList.OrderByDescending(a => a.IdUserNavigation.Username).ToList();
                    break;
                default:  // Name ascending 
                    adList = adList.OrderBy(a => a.Title).ToList();
                    break;
            }


            return View(adList);
        }


    }
}
