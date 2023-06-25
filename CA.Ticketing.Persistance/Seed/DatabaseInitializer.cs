using CA.Ticketing.Common.Constants;
using CA.Ticketing.Common.Extensions;
using CA.Ticketing.Persistance.Context;
using CA.Ticketing.Persistance.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CA.Ticketing.Persistance.Seed
{
    public class DatabaseInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly CATicketingContext _context;

        private readonly IWebHostEnvironment _hostEnvironment;

        private const string _adminUserName = "admin";

        private const string _adminInitialPassword = "I@mAdm1nUs3r";

        public DatabaseInitializer(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            CATicketingContext context, 
            IWebHostEnvironment hostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task InitializeAsync()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }

            await CreateRoles();
            await CreateBaseUser();
        }

        private async Task CreateRoles()
        {
            foreach (string roleName in StringExtensions.GetRoleNames())
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    continue;
                }

                role = new IdentityRole(roleName);

                await _roleManager.CreateAsync(role);
            }
        }

        private async Task CreateBaseUser()
        {
            var adminUser = await _userManager.FindByNameAsync(_adminUserName);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser()
                {
                    Email = "info@readydev.tech",
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Admin",
                    UserName = _adminUserName
                };

                await _userManager.CreateAsync(adminUser, _adminInitialPassword);
            }

            var adminRoles = await _userManager.GetRolesAsync(adminUser);

            if (!adminRoles.Contains(RoleNames.Admin))
            {
                await _userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
            }
        }
    }
}
