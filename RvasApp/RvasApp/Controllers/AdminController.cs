using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RvasApp.Data;
using RvasApp.Konstante;
using RvasApp.Models;
using RvasApp.Models.ViewModels;

namespace RvasApp.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<Korisnik> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext context;

        public AdminController(UserManager<Korisnik> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var korisnici = await userManager.Users
                .OrderBy(x => x.Email)
                .ToListAsync();

            var korisniciVm = new List<AdminUserViewModel>();

            foreach (var korisnik in korisnici)
            {
                var uloge = await userManager.GetRolesAsync(korisnik);

                if (uloge.Contains(Roles.Admin.ToString()))
                    continue;

                korisniciVm.Add(new AdminUserViewModel
                {
                    Id=korisnik.Id,
                    Email=korisnik.Email ?? string.Empty,
                    Ime=korisnik.Ime,
                    Prezime=korisnik.Prezime,
                    KorisnickoIme=korisnik.KorisnickoIme,
                    Roles=uloge.ToList()
                });

            }

            var brojKorisnika = korisnici.Count;
            var brojPolaznika = (await userManager.GetUsersInRoleAsync(Roles.Polaznik.ToString())).Count;
            var brojInstruktora = (await userManager.GetUsersInRoleAsync(Roles.Instruktor.ToString())).Count;

            var model = new AdminPanelViewModel
            {
                BrojKorisnika=brojKorisnika,
                BrojInstruktora=brojInstruktora,
                BrojPolaznika=brojPolaznika,
                Korisnici = korisniciVm
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //Problem sa vezbi je bio kod parametra
        //Na vezbama parametar je bio string korisnikId u metodi
        //Ovde imamo model binding gde se povezuju podaci iy HTTP requesta sa parametrima metode,
        //Zato mora strogo po imenu da se poklapaju i na viewu i u kontroleru
        //<input type="hidden" name="id" value="@korisnik.Id"> name je bio id -> sad je korisnikId kao i parametar metode
        public async Task<IActionResult> DodeliInstruktorRolu(string korisnikId)
        {
            if (string.IsNullOrWhiteSpace(korisnikId))
                return RedirectToAction(nameof(Index));

            var korisnik = await userManager.FindByIdAsync(korisnikId);
            if (korisnik == null)
                return RedirectToAction(nameof(Index));

            var instruktor = Roles.Instruktor.ToString();

            if (!await roleManager.RoleExistsAsync(instruktor))
                await roleManager.CreateAsync(new IdentityRole(instruktor));

            if (!await userManager.IsInRoleAsync(korisnik, instruktor))
                await userManager.AddToRoleAsync(korisnik, instruktor);

            var polaznik = Roles.Polaznik.ToString();
            if (await userManager.IsInRoleAsync(korisnik, polaznik))
                await userManager.RemoveFromRoleAsync(korisnik, polaznik);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OduzmiInstruktorRolu(string korisnikId)
        {
            if (string.IsNullOrWhiteSpace(korisnikId))
            {
                return RedirectToAction(nameof(Index));
            }
            var korisnik = await userManager.FindByIdAsync(korisnikId);
            if (korisnik == null)
            {
                return RedirectToAction(nameof(Index));
            }
            var instruktor = Roles.Instruktor.ToString();

            if (await userManager.IsInRoleAsync(korisnik, instruktor))
            {
                await userManager.RemoveFromRoleAsync(korisnik, instruktor);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodeliPolaznikRolu(string korisnikId)
        {
            if (string.IsNullOrWhiteSpace(korisnikId))
            {
                return RedirectToAction(nameof(Index));
            }

            var korisnik = await userManager.FindByIdAsync(korisnikId);
            if (korisnik == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var polaznik = Roles.Polaznik.ToString();

            if (!await roleManager.RoleExistsAsync(polaznik))
            {
                await roleManager.CreateAsync(new IdentityRole(polaznik));
            }

            if (!await userManager.IsInRoleAsync(korisnik, polaznik))
            {
                await userManager.AddToRoleAsync(korisnik, polaznik);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OduzmiPolaznikRolu(string korisnikId)
        {
            if (string.IsNullOrWhiteSpace(korisnikId))
            {
                return RedirectToAction(nameof(Index));
            }

            var korisnik = await userManager.FindByIdAsync(korisnikId);
            if (korisnik == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var polaznik = Roles.Polaznik.ToString();

            if (await userManager.IsInRoleAsync(korisnik, polaznik))
            {
                await userManager.RemoveFromRoleAsync(korisnik, polaznik);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
