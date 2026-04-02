using System.ComponentModel.DataAnnotations;

namespace RvasApp.Models.ViewModels
{
    public class PolaznikNaKursuViewModel
    {
        [Display(Name ="Ime polaznika")]
        public string? Ime { get; set; }

        [Display(Name = "Prezime polaznika")]
        public string? Prezime { get; set; }
        [Display(Name = "Email adresa polaznika")]
        public string? Email { get; set; }
        [Display(Name = "Datum upisa na kurs")]
        [DataType(DataType.Date)]
        public DateTime DatumPrijave { get; set; }

    }
}
