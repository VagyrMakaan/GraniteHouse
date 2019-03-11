using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _db.Product.Include(m => m.ProductType).Include(m => m.SpecialTag).ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Product.Include(m => m.ProductType).Include(m => m.SpecialTag).Where(m => m.Id == id).FirstOrDefaultAsync();

            return View(product);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id)
        {
            List<int> shoppingCartContents = HttpContext.Session.Get<List<int>>("sesShoppingCart");

            if (shoppingCartContents == null)
            {
                shoppingCartContents = new List<int>();
            }

            shoppingCartContents.Add(id);
            HttpContext.Session.Set("sesShoppingCart", shoppingCartContents);

            return RedirectToAction(nameof(Index), "Home", new { area = "Customer" });
        }

        public IActionResult Remove(int id)
        {
            List<int> shoppingCartContents = HttpContext.Session.Get<List<int>>("sesShoppingCart");

            if (shoppingCartContents != null && shoppingCartContents.Count > 0)
            {
                if (shoppingCartContents.Contains(id))
                {
                    shoppingCartContents.Remove(id);
                }
            }

            HttpContext.Session.Set("sesShoppingCart", shoppingCartContents);

            return RedirectToAction(nameof(Index), "Home", new { area = "Customer" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
