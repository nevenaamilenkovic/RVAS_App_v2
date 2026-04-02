// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RvasApp.Models;

namespace RvasApp.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly SignInManager<Korisnik> _signInManager;

        public IndexModel(
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage ="Ime je obavezno za unos")]
            [StringLength(35, ErrorMessage = "Ime moze sadrzati najvise 35 karaktera")]
            [Display(Name = "Ime korisnika")]
            public string Ime { get; set; }

            [Required(ErrorMessage = "Prezime je obavezno za unos")]
            [StringLength(35, ErrorMessage = "Prezime moze sadrzati najvise 35 karaktera")]
            public string Prezime { get; set; }

            [Required(ErrorMessage = "Korisnicko ime je obavezno za unos")]
            [StringLength(45, ErrorMessage = "Korisnicko ime moze sadrzati najvise 45 karaktera")]
            public string KorisnickoIme { get; set; }
        }

        private async Task LoadAsync(Korisnik user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Ime=user.Ime,
                Prezime=user.Prezime,
                KorisnickoIme=user.KorisnickoIme
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            //ime
            if (string.IsNullOrEmpty(user.Ime))
            {
                user.Ime = Input.Ime;
            }
            else if(Input.Ime != user.Ime)
            {
                StatusMessage = "Error: Ime nije moguce menjati!";
                return RedirectToPage();
            }
            //prezime
            if (string.IsNullOrEmpty(user.Prezime))
            {
                user.Prezime = Input.Prezime;
            }
            else if (Input.Prezime != user.Prezime)
            {
                StatusMessage = "Error: Prezime nije moguce menjati!";
                return RedirectToPage();
            }
            //korisnicko ime
            if (string.IsNullOrEmpty(user.KorisnickoIme))
            {
                user.KorisnickoIme = Input.KorisnickoIme;
            }
            else if (Input.KorisnickoIme != user.KorisnickoIme)
            {
                StatusMessage = "Error: Korisnicko ime nije moguce menjati!";
                return RedirectToPage();
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                StatusMessage = "Error: Greska pri azuriranju profila";
                return RedirectToPage();
            }

            StatusMessage = "Profil je uspesno azuriran";
            return RedirectToPage();
        }
    }
}
