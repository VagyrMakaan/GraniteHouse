﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModel;
using GraniteHouse.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    [Area("Admin")]
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AppointmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            string searchName = null, 
            string searchEmail = null, 
            string searchPhone = null, 
            string searchDate = null)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                Appointments = new List<Appointment>(),
            };

            // Why does the course use non-synchronous code?
            appointmentVM.Appointments = _db.Appointment.Include(a => a.SalesPerson).ToList();
            // appointmentVM.Appointments = await _db.Appointment.Include(a => a.SalesPerson).ToListAsync();
            if (User.IsInRole(SD.AdminEndUser))
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
            }
            
            if (searchName != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }

            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }

            if (searchDate != null)
            {
                try
                {
                    DateTime date = Convert.ToDateTime(searchDate);
                    appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.AppointmentDate.ToShortDateString().Equals(date.ToShortDateString())).ToList();
                }
                catch (Exception ex)
                {

                }
            }

            return View(appointmentVM);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await(
                from p in _db.Product
                join a in _db.ProductSelectedForAppointment
                on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductType").ToListAsync();

            var appointmentDetailsVM = new AppointmentDetailsViewModel()
            {
                Appointment = await _db.Appointment.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefaultAsync(),
                SalesPerson = await _db.ApplicationUser.ToListAsync(),
                Products = products
            };

            return View(appointmentDetailsVM);
        }

        [HttpPost,ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, AppointmentDetailsViewModel appointmentVM)
        {
            if (!ModelState.IsValid)
            {
                return View(appointmentVM);
            }

            appointmentVM.Appointment.AppointmentDate = appointmentVM.Appointment.AppointmentDate
                .AddHours(appointmentVM.Appointment.AppointmentTime.Hour)
                .AddMinutes(appointmentVM.Appointment.AppointmentTime.Minute);

            var appointmentFromDb = await _db.Appointment.Where(a => a.Id == appointmentVM.Appointment.Id).FirstOrDefaultAsync();

            appointmentFromDb.CustomerName = appointmentVM.Appointment.CustomerName;
            appointmentFromDb.CustomerEmail = appointmentVM.Appointment.CustomerEmail;
            appointmentFromDb.CustomerPhoneNumber = appointmentVM.Appointment.CustomerPhoneNumber;
            appointmentFromDb.AppointmentDate = appointmentVM.Appointment.AppointmentDate;
            appointmentFromDb.IsConfirmed = appointmentVM.Appointment.IsConfirmed;

            if (User.IsInRole(SD.SuperAdminEndUser))
            {
                appointmentFromDb.SalesPersonId = appointmentVM.Appointment.SalesPersonId;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await (
                from p in _db.Product
                join a in _db.ProductSelectedForAppointment
                on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductType").ToListAsync();

            var appointmentDetailsVM = new AppointmentDetailsViewModel()
            {
                Appointment = await _db.Appointment.Include(a => a.SalesPerson).Where(a => a.Id == id).FirstOrDefaultAsync(),
                SalesPerson = await _db.ApplicationUser.ToListAsync(),
                Products = products
            };

            return View(appointmentDetailsVM);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // What about the ProductSelectedForAppointment stuff???
            // There's dead bodies in the database!!!

            var appointment = await _db.Appointment.FindAsync(id);
            _db.Appointment.Remove(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}