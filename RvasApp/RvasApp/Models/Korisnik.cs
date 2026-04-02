using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace RvasApp.Models
{
    public class Korisnik:IdentityUser
    {
        [StringLength(35,ErrorMessage ="Ime moze sadrzati najvise 35 karaktera")]
        [Display(Name ="Ime korisnika")]
        public string? Ime { get; set; }
        [StringLength(35, ErrorMessage = "Prezime moze sadrzati najvise 35 karaktera")]
        public string? Prezime { get; set; }
        //Username kod IdentityUser-a je uvek email!
        [StringLength(45, ErrorMessage = "Korisnicko ime moze sadrzati najvise 45 karaktera")]
        public string? KorisnickoIme { get; set; }
        public DateTime DatumRegistracije { get; set; } = DateTime.UtcNow;


    }
}
