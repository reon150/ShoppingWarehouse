using Microsoft.AspNetCore.Identity;
using ShoppingWarehouse.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingWarehouse.Data
{
    public static class ApplicationDbInitializer
    {
        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (userManager.FindByEmailAsync("admin@unapec.edu.do").Result == null)
            {
                IdentityUser user = new()
                {
                    UserName = "admin@unapec.edu.do",
                    Email = "admin@unapec.edu.do",
                    EmailConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "Pa$$w0rd").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, Role.Admin).Wait();
                }
            }
        }
    }
}
