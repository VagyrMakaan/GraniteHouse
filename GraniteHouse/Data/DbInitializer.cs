using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GraniteHouse.Utility;
using GraniteHouse.Models;

namespace GraniteHouse.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async void Initialize()
        {
            // Apply all migrations to the current database

            if (_db.Database.GetPendingMigrations().Count() > 0)
            {
                _db.Database.Migrate();
            }

            // Add the user roles, only if they don't exist

            if (_db.Roles.Any(r => r.Name != SD.SuperAdminEndUser))
            {
                _roleManager.CreateAsync(new IdentityRole(SD.SuperAdminEndUser)).GetAwaiter().GetResult();
            }

            if (_db.Roles.Any(r => r.Name != SD.AdminEndUser))
            {
                _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser)).GetAwaiter().GetResult();
            }

            // Add the initial super admin & admin users, but only, if they don't exist

            if (_db.ApplicationUser.Any(u => u.Email != "super.admin@granite.com"))
            {
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "super.admin@granite.com",
                    Email = "super.admin@granite.com",
                    Name = "Super Admin",
                    EmailConfirmed = true
                }, "Admin123*").GetAwaiter().GetResult();

                IdentityUser user = await _db.Users.Where(u => u.Email == "super.admin@granite.com").FirstOrDefaultAsync();
                await _userManager.AddToRoleAsync(user, SD.SuperAdminEndUser);
            }

            if (_db.ApplicationUser.Any(u => u.Email != "admin@granite.com"))
            {
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@granite.com",
                    Email = "admin@granite.com",
                    Name = "Admin",
                    EmailConfirmed = true
                }, "Admin123*").GetAwaiter().GetResult();

                IdentityUser user = await _db.Users.Where(u => u.Email == "admin@granite.com").FirstOrDefaultAsync();
                await _userManager.AddToRoleAsync(user, SD.AdminEndUser);
            }

            // TODO: Add other entity fixtures (e.g. product types, special tags, etc.)
        }
    }
}
