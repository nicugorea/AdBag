using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AdBagWeb.Models;
using AdBagWeb.Classes;

namespace AdBagWeb.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AdBagWebDBContext _context;

        public CategoriesController(AdBagWebDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!Authentication.Instance.IsAdmin()) return Redirect("~/Home");
            var categoriesList = await _context.Category.ToListAsync();
            categoriesList.Sort((a, b) => string.Compare(a.Name, b.Name));
            return View(categoriesList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!Authentication.Instance.IsAdmin()) return Redirect("~/Home");
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.IdCategory == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult Create()
        {
            if (!Authentication.Instance.IsAdmin()) return Redirect("~/Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategory,Name")] Category category)
        {
            if (!Authentication.Instance.IsAdmin()) return Redirect("~/Home");
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!Authentication.Instance.IsAdmin()) return Redirect("~/Home");
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name")] Category category)
        {
            if (!Authentication.Instance.IsAdmin()) return Redirect("~/Home");
            if (id != category.IdCategory)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.IdCategory))
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
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!Authentication.Instance.IsAdmin()) return Redirect("~/Home");
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .FirstOrDefaultAsync(m => m.IdCategory == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!Authentication.Instance.IsAdmin()) return Redirect("~/Home");
            var category = await _context.Category.FindAsync(id);
            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.IdCategory == id);
        }
    }
}
