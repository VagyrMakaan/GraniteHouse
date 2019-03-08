using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductViewModel ProductVM { get; set; }

        public ProductController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;

            ProductVM = new ProductViewModel()
            {
                ProductTypes = _db.ProductType.ToList(),
                SpecialTags = _db.SpecialTag.ToList(),
                Product = new Models.Product()
            };
        }

        public async Task<IActionResult> Index()
        {
            var products = _db.Product.Include(m => m.ProductType).Include(m => m.SpecialTag);
            return View(await products.ToListAsync());
        }

        public IActionResult Create()
        {
            return View(ProductVM);
        }

        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductVM);
            }

            _db.Product.Add(ProductVM.Product);
            await _db.SaveChangesAsync();
            
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productsFromDb = _db.Product.Find(ProductVM.Product.Id);

            if (files.Count != 0)
            {
                // Image has been uploaded
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, ProductVM.Product.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductVM.Product.Id + extension;
            }
            else
            {
                // When user does not upload/provide an image, then use the default_product.jpg
                var uploads = Path.Combine(webRootPath, SD.ImageFolder + @"\" + SD.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageFolder + @"\" + ProductVM.Product.Id + ".jpg");
                productsFromDb.Image = @"\" + SD.ImageFolder + @"\" + ProductVM.Product.Id + ".jpg";
               
            }
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Product = await _db.Product.Include(m => m.SpecialTag).Include(m => m.ProductType).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int id)
        {
            if (!ModelState.IsValid)
            {
                return View(ProductVM);
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var productFromDb = _db.Product.Where(m => m.Id == ProductVM.Product.Id).FirstOrDefault();

            if (files.Count > 0 && files[0] != null)
            {
                // if user uploads a new image
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);

                var extension_old = Path.GetExtension(productFromDb.Image);
                if (System.IO.File.Exists(Path.Combine(uploads, ProductVM.Product.Id + extension_old)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, ProductVM.Product.Id + extension_old));
                }

                var extension_new = Path.GetExtension(files[0].FileName);
                using (var filestream = new FileStream(Path.Combine(uploads, ProductVM.Product.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                ProductVM.Product.Image = @"\" + SD.ImageFolder + @"\" + ProductVM.Product.Id + extension_new;
            }

            if (ProductVM.Product.Image != null)
            {
                productFromDb.Image = ProductVM.Product.Image;
            }

            productFromDb.Name = ProductVM.Product.Name;
            productFromDb.Price = ProductVM.Product.Price;
            productFromDb.Available = ProductVM.Product.Available;
            productFromDb.ProductTypeId = ProductVM.Product.ProductTypeId;
            productFromDb.SpecialTagId = ProductVM.Product.SpecialTagId;
            productFromDb.ShadeColor = ProductVM.Product.ShadeColor;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Product = await _db.Product.Include(m => m.SpecialTag).Include(m => m.ProductType).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Product = await _db.Product.Include(m => m.SpecialTag).Include(m => m.ProductType).SingleOrDefaultAsync(m => m.Id == id);

            if (ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePOST(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Product product = await _db.Product.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            else
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolder);
                var extension = Path.GetExtension(product.Image);

                if (System.IO.File.Exists(Path.Combine(uploads, product.Id + extension)))
                {
                    System.IO.File.Delete(Path.Combine(uploads, product.Id + extension));
                }

                _db.Product.Remove(product);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
        }
    }
}