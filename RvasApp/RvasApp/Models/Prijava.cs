using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RvasApp.Models
{
    [Index(nameof(KursId), nameof(PolaznikId), IsUnique = true)]
    public class Prijava
    {
        public int Id { get; set; }

        [Required]
        public int KursId { get; set; }

        [ForeignKey(nameof(KursId))]
        public Kurs Kurs { get; set; }

        [Required]
        public string PolaznikId { get; set; }

        [ForeignKey(nameof(PolaznikId))]
        public Korisnik Polaznik { get; set; }

        public DateTime DatumPrijave { get; set; } = DateTime.UtcNow;
    }
}
