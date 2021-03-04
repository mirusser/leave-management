using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_management.Data.SeedData
{
    public static class SeedData
    {
        public static void Seed(
            UserManager<Employee> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<Employee> userManager)
        {
            var foo = userManager.Users.ToList();
            if (userManager.FindByNameAsync("admin@admin.com").Result is null)
            {
                var user = new Employee
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com"
                };

                var createAdminResult = userManager.CreateAsync(user, "Password1!").Result;

                if (createAdminResult.Succeeded)
                {
                    var rolesForAdmin = new List<string>() { "admin", "employee" };
                    userManager.AddToRolesAsync(user, rolesForAdmin).Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("admin").Result)
            {
                roleManager.CreateAsync(new IdentityRole { Name = "admin" }).Wait();
            }

            if (!roleManager.RoleExistsAsync("employee").Result)
            {
                roleManager.CreateAsync(new IdentityRole { Name = "employee" }).Wait();
            }
        }
    }
}
