using Microsoft.AspNetCore.Identity;
using RvasApp.Models;
using RvasApp.Konstante;

namespace RvasApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<Korisnik>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Instruktor.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Polaznik.ToString()));

            var korisnik = new Korisnik
            {
                UserName = "korisnik1@rvas.rs",
                Email = "korisnik1@rvas.rs",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var korisnik_db = await userManager.FindByEmailAsync(korisnik.Email);
            if(korisnik_db == null)
            {
                await userManager.CreateAsync(korisnik,"Admin123$");
                await userManager.AddToRoleAsync(korisnik,Roles.Admin.ToString());
            }
            

        }
    }
}
