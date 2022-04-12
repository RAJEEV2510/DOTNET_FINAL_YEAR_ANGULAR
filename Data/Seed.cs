
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Data
{
    public class Seed
    {

        public static async Task SeedUsers(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager)
        {

            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeed.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData); //convert into objects

            var roles = new List<AppRole> { 
                new  AppRole{Name="Member" },
                new  AppRole{Name="Admin" },
                new  AppRole{Name="Moderator" }
            };
            
            foreach(var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users)
            {
                user.UserName = user.UserName.ToLower();
                user.Created = user.Created.ToUniversalTime();
                user.DateOfBirth = user.DateOfBirth.ToUniversalTime();
                user.LastActive = user.LastActive.ToUniversalTime();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");

            }
            var admin = new AppUser
            {
                UserName = "admin",
                Created= DateTime.Now.ToUniversalTime(),
                DateOfBirth= DateTime.Now.ToUniversalTime(),
                LastActive=DateTime.Now.ToUniversalTime()
                
            };

            //seed admin user and add role to database
            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRoleAsync(admin, "Admin");
            await userManager.AddToRoleAsync(admin, "Moderator");
        }
    }
}
