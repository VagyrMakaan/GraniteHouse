using System;
using System.Collections.Generic;
using System.Text;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductType> ProductType { get; set; }
        public DbSet<SpecialTag> SpecialTag { get; set; }
        public DbSet<Product> Product { get; set; }

        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<ProductSelectedForAppointment> ProductSelectedForAppointment { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
    }
}
