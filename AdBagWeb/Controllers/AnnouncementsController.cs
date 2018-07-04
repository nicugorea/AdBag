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
using Microsoft.AspNetCore.Hosting;

namespace AdBagWeb.Controllers
{
    public class AnnouncementsController : Controller
    {
        #region Properties
        private readonly AdBagWebDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        #endregion

        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }

        #region Create
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



                var image = new ImageFile();
                image.Extension = Path.GetExtension(announcement.Image.FileName);
                image.Name = announcement.Ad.IdAnnouncement.ToString() + "_" + announcement.Ad.UploadDate.ToString("yyyy-mm-dd-hh-mm-ss");
                image.Path = "/ImgUploads/" + image.Name + image.Extension;
                using (var stream = new FileStream("Uploads/img/" + image.Name + image.Extension, FileMode.Create))
                {

                    await announcement.Image.CopyToAsync(stream);
                }
                _context.Add(image);
                _context.SaveChanges();
                var img = _context.ImageFile.First(i => i.Name == image.Name);
                announcement.Ad.IdImage = img.IdImage;
                _context.Announcement.Add(announcement.Ad);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCategory"] = new SelectList(_context.Category, "IdCategory", "Name", announcement.Ad.IdCategory);
            ViewData["IdUser"] = new SelectList(_context.User, "IdUser", "Email", announcement.Ad.IdUser);

            return View(announcement);

        }
        #endregion

        #region Constructor
        public AnnouncementsController(AdBagWebDBContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion


        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var announcement = await _context.Announcement
                .Include(a => a.IdCategoryNavigation)
                .Include(a => a.IdUserNavigation)
                .Include(a => a.IdImageNavigation)
                .FirstOrDefaultAsync(m => m.IdAnnouncement == id);
            if (announcement == null)
            {
                return NotFound();
            }
            var commentsList = _context.Comment.
                Include(c => c.IdAnnouncementNavigation).
                Include(u => u.IdUserNavigation).
                Where(c=>c.IdAnnouncement==id).
                ToList();

            ViewBag.Comments = commentsList;
            return View(announcement);
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
        #endregion


        #region Edit
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
        #endregion

        #region Delete
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

            var comments = _context.Comment.Where(c => c.IdAnnouncement == id).ToList();

            foreach (var item in comments)
            {
                _context.Remove(item);
            }
            await _context.SaveChangesAsync();

            _context.Announcement.Remove(announcement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region List
        public IActionResult List(string sortBy, string searchBy, string searchValue, string category)
        {
            ViewBag.Categories = _context.Category.ToList();


            ViewBag.SortBy = sortBy;
            ViewBag.SearchBy = searchBy;
            ViewBag.SearchValue = searchValue;
            ViewBag.Category = category;

            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortBy) ? "title_desc" : "title_asc";
            ViewBag.DateSortParm = sortBy == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.CategorySortParm = sortBy == "category_asc" ? "category_desc" : "category_asc";
            ViewBag.UserSortParm = sortBy == "user_asc" ? "user_desc" : "user_asc";




            var adList = new List<Announcement>();

            switch (searchBy)
            {
                case "user":
                    adList = _context.Announcement.
                    Where(a => searchValue == null || a.IdUserNavigation.Username.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).
                    Include(a => a.IdCategoryNavigation).
                    Include(a => a.IdUserNavigation).
                    Include(a => a.IdImageNavigation).
                    ToList();
                    break;
                default:
                    adList = _context.Announcement.
                    Where(a => searchValue == null || a.Title.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).
                    Include(a => a.IdCategoryNavigation).
                    Include(a => a.IdUserNavigation).
                    Include(a => a.IdImageNavigation).
                    ToList();
                    break;
            }

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
                default:
                    adList = adList.OrderBy(a => a.Title).ToList();
                    break;
            }

            if (!String.IsNullOrEmpty(category) && category != "All")
            {
                adList = adList.Where(a => a.IdCategoryNavigation.Name == category).ToList();
                var toDelete = (ViewBag.Categories as List<Category>).First(c => c.Name == ViewBag.Category as string);
                if (toDelete != null)
                    (ViewBag.Categories as List<Category>).Remove(toDelete);

            }

            return View(adList);
        }
        #endregion

        #region PrivateMethods
        private bool AnnouncementExists(int id)
        {
            return _context.Announcement.Any(e => e.IdAnnouncement == id);
        }
        #endregion


    }
}
