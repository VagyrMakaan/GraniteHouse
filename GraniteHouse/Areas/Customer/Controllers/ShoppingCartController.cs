using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel()
            {
                Products = new List<Models.Product>()
            };
        }

        public async Task<IActionResult> Index()
        {
            List<int> shoppingCartContents = HttpContext.Session.Get<List<int>>("sesShoppingCart");

            if (shoppingCartContents != null && shoppingCartContents.Count > 0)
            {
                foreach (int item in shoppingCartContents)
                {
                    Product product = await _db.Product.Include(p => p.SpecialTag).Include(p => p.ProductType).Where(p => p.Id == item).FirstOrDefaultAsync();
                    ShoppingCartVM.Products.Add(product);
                }
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            List<int> shoppingCartContents = HttpContext.Session.Get<List<int>>("sesShoppingCart");
            
            if (!ModelState.IsValid 
                || shoppingCartContents.Count < 1) {
                
                // Why is a reimplementation of the ctor-functionality necessary? 
                // Why or where is the Product-List set to null???
                if (ShoppingCartVM.Products == null)
                {
                    ShoppingCartVM.Products = new List<Product>();
                }

                // Why do I need to reimplement the GET-Index function? 
                if (shoppingCartContents.Count > 0)
                {
                    foreach (int item in shoppingCartContents)
                    {
                        Product product = await _db.Product.Include(p => p.SpecialTag).Include(p => p.ProductType).Where(p => p.Id == item).FirstOrDefaultAsync();
                        ShoppingCartVM.Products.Add(product);
                    }
                }

                return View(ShoppingCartVM);
            }

            //  Solution as proposed in the course, does not seem to work properly 
            //  - Appointment.AppointmentTime & Appointment.AppointmentDate are either null or set to '0001/01/01 00:00:00'
            //  - No idea why the form-inputs do not 'bind' to the properties => no idea where to start debugging/what kind of magic is used for that

            /*
             *  ShoppingCartVM.Appointment.AppointmentDate = ShoppingCartVM.Appointment.AppointmentDate
             *      .AddHours(ShoppingCartVM.Appointment.AppointmentTime.Hour)
             *      .AddMinutes(ShoppingCartVM.Appointment.AppointmentTime.Minute);
            */

            // Work-Around: Due to above code not working, date parts are read from POST manually and converted to usable DateTime objects

            var _request = HttpContext.Request;
            var dateAsString = _request.Form["Appointment.AppointmentDate"];
            var timeAsString = _request.Form["Appointment.AppointmentTime"];

            DateTime date;
            DateTime.TryParseExact(dateAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

            DateTime time;
            DateTime.TryParseExact(timeAsString, "hh:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out time);

            ShoppingCartVM.Appointment.AppointmentDate = date.AddHours(time.Hour).AddMinutes(time.Minute);

            // Continuation of udemy-course code

            Appointment appointment = ShoppingCartVM.Appointment;

            _db.Appointment.Add(appointment);
            await _db.SaveChangesAsync();

            int appointmentId = appointment.Id;

            foreach (int productId in shoppingCartContents)
            {
                ProductSelectedForAppointment productSelectedForAppointment = new ProductSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };

                _db.ProductSelectedForAppointment.Add(productSelectedForAppointment);
            }

            await _db.SaveChangesAsync();

            shoppingCartContents = new List<int>();
            HttpContext.Session.Set("sesShoppingCart", shoppingCartContents);

            return RedirectToAction("AppointmentConfirmation", "ShoppingCart", new { Id = appointmentId });
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            List<int> shoppingCartContents = HttpContext.Session.Get<List<int>>("sesShoppingCart");

            if (shoppingCartContents.Count > 0)
            {
                if (shoppingCartContents.Contains(id))
                {
                    shoppingCartContents.Remove(id);
                    HttpContext.Session.Set("sesShoppingCart", shoppingCartContents);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AppointmentConfirmation(int id)
        {
            ShoppingCartVM.Appointment = await _db.Appointment.Where(a => a.Id == id).FirstOrDefaultAsync();
            List<ProductSelectedForAppointment> productsSelectedForAppointment = await _db.ProductSelectedForAppointment.Where(p => p.AppointmentId == id).ToListAsync();

            foreach (ProductSelectedForAppointment productSelected in productsSelectedForAppointment)
            {
                var product = await _db.Product.Include(p => p.ProductType).Include(p => p.SpecialTag).Where(p => p.Id == productSelected.ProductId).FirstOrDefaultAsync();
                ShoppingCartVM.Products.Add(product);
            }

            return View(ShoppingCartVM);
        }
    }
}