using AuthAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthAPI.Data;

public static class IdentitySeeder
{
    public static async Task SeedAdminUser(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        const string adminEmail = "admin@gmail.com";
        const string adminPassword = "Admin123$";

        var userExist = await userManager.FindByEmailAsync(adminEmail);
        if (userExist == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                PhoneNumber = "0712345678",
                EmailConfirmed = true
            };

            // Create the admin user
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors));
            }
        }
    }
}