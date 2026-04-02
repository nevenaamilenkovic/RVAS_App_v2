using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RvasApp.Models
{
    public class Lekcija
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Morate uneti naziv lekcije")]
        [Display(Name = "Naziv lekcije")]
        [StringLength(125)]
        public string Naziv { get; set;
        }
        [Required(ErrorMessage="Morate uneti sadrzaj lekcije")]
        [Display(Name ="Sadrzaj lekcije")]
        public string Sadrzaj { get; set; }
        [Required]
        public int KursId { get; set; }
        [ForeignKey(nameof(KursId))]
        public Kurs? Kurs { get; set; }

        public ICollection<LekcijaMaterijali> Materijali { get; set; } = new List<LekcijaMaterijali>();
    }
}
