using System.ComponentModel.DataAnnotations;

namespace RvasApp.Models
{
    public class LekcijaMaterijali
    {
        public int Id { get; set; }
        [Required]
        public int LekcijaId { get; set; }
        public Lekcija? Lekcija { get; set; }
        [Required, StringLength(200)]
        public string Naziv { get; set; } = null!;
        [Required, StringLength(500)]
        public string Putanja { get; set; } = null!; //"uploads/lekcije/1/file.pdf"
        [StringLength(100)]
        public string? Tip { get; set; }// pdf,docx,pptx...
        public long Velicina { get; set; } // u bajtovima?
        public DateTime DatumOtpremanja { get; set; } = DateTime.UtcNow;
        [Required]
        public string InstruktorId { get; set; } = null!;
        public Korisnik? Instruktor { get; set; }
    }
}
