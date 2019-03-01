using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecialTagController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialTagController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.SpecialTag.ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _db.SpecialTag.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag specialTag)
        {
            if (!ModelState.IsValid)
            {
                return View(specialTag);
            }

            _db.Add(specialTag);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _db.SpecialTag.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTag specialTag)
        {
            if (id != specialTag.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(specialTag);
            }

            _db.Update(specialTag);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _db.SpecialTag.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var specialTag = await _db.SpecialTag.FindAsync(id);

            if (specialTag == null)
            {
                return NotFound();
            }

            _db.SpecialTag.Remove(specialTag);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}