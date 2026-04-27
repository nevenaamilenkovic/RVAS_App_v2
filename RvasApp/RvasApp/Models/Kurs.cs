using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RvasApp.Models
{
    public class Kurs
    {

        public int Id { get; set; }
        [Required(ErrorMessage ="Naziv kursa je obavezan")]
        [StringLength(100,ErrorMessage ="Naziv kursa moze sadrzati najvise 100 karaktera")]
        [Display(Name ="Naziv kursa")]
        public string Naziv { get; set; }

        //[StringLength(525, ErrorMessage = "Opis kursa moze sadrzati najvise 525 karaktera")]
        [Display(Name = "Opis kursa")]
        public string? Opis { get; set; }
        [Display(Name = "Datum pocetka kursa")]
        [DataType(DataType.Date)]
        public DateTime? DatumPocetka { get; set; }
        [Display(Name = "Datum zavrsetka kursa")]
        [DataType(DataType.Date)]
        public DateTime? DatumZavrsetka { get; set; }
        [Range(1,50,ErrorMessage ="Maksimalan broj polaznika je 50")]
        [Display(Name = "Maksimalan broj polaznika")]
        public int? MaksimalanBrojPolaznika { get; set; }

        [Display(Name = "Instruktor")]
        public string? InstruktorId { get; set; }
        [ForeignKey(nameof(InstruktorId))]
        public Korisnik? Instruktor { get; set; }

        public ICollection<Prijava> Prijave { get; set; } = new List<Prijava>();
        public ICollection<Lekcija> Lekcije { get; set; } = new List<Lekcija>();


    }
}
