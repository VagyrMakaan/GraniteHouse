using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class ProductTypeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ProductType.ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _db.ProductType.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductType productType)
        {
            if (!ModelState.IsValid)
            {
                return View(productType);
            }

            _db.Add(productType);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _db.ProductType.FindAsync(id); 
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductType productType)
        {
            if (id != productType.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(productType);
            }

            _db.Update(productType);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productType = await _db.ProductType.FindAsync(id);
            if (productType == null)
            {
                return NotFound();
            }

            return View(productType);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productType = await _db.ProductType.FindAsync(id);

            if (productType == null)
            {
                return NotFound();
            }

            _db.ProductType.Remove(productType);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}