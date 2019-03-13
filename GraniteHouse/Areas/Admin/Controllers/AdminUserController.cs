using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AdminUserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminUserController(ApplicationDbContext db)
        {
            _db = db;
        }

        // Gibt es einen Grund, wieso diese Funktion nicht als asynchron deklariert sein könnte?
        // Es gibt offensichtlich einen Datenbank-Zugriff, ergo sollte ein await verwendet werden!
        public IActionResult Index()
        {
            return View(_db.ApplicationUser.ToList());
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var user = await _db.ApplicationUser.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userFromDb = await _db.ApplicationUser.Where(u => u.Id == id).FirstOrDefaultAsync();
                userFromDb.Name = user.Name;
                userFromDb.PhoneNumber = user.PhoneNumber;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var user = await _db.ApplicationUser.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(string id)
        {
            var userFromDb = await _db.ApplicationUser.Where(u => u.Id == id).FirstOrDefaultAsync();
            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}