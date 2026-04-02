using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RvasApp.Data;
using RvasApp.Konstante;
using RvasApp.Models;
using RvasApp.Models.ViewModels;
using System.Security.Claims;

namespace RvasApp.Controllers
{
    [Authorize]//samo ulogovani useri mogu da pristupe
    public class KursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KursController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kursevi = await _context.Kursevi
                .Include(k => k.Instruktor)
                .OrderBy(k => k.Naziv)
                .ToListAsync();

            return View(kursevi);
        }

        [Authorize(Roles = nameof(Roles.Instruktor))]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = nameof(Roles.Instruktor))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Naziv,Opis,DatumPocetka,DatumZavrsetka,MaksimalanBrojPolaznika")] Kurs kurs)
        {
            if (!ModelState.IsValid)
                return View(kurs);

            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            kurs.InstruktorId = instruktorId;

            _context.Kursevi.Add(kurs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var kurs = await _context.Kursevi
                .Include(k => k.Instruktor)
                .Include(k => k.Lekcije)
                    .ThenInclude(l => l.Materijali)
                .FirstOrDefaultAsync(k => k.Id == id.Value);
            if (kurs == null)
                return NotFound();

            //ako je polaznik na details stranici treba ispitati da li je upisan na taj kurs
            var polaznikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //pretpostavimo da nije prijavljen
            bool jePrijavljen = false;
            if (!string.IsNullOrEmpty(polaznikId))
            {
                jePrijavljen = await _context.Prijave.AnyAsync(p => p.KursId == id && p.PolaznikId == polaznikId);
            }
            ViewBag.JePrijavljen = jePrijavljen;

            return View(kurs);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Roles.Polaznik))]
        public async Task<IActionResult> PrijaviSe(int id)
        {
            //prvo trazimo kurs na koji zeli da se prijavi po kurs id-ju
            var kurs = await _context.Kursevi.FindAsync(id);
            if (kurs == null)
                return NotFound();

            //dohvatamo id ulogovanog polaznika
            var polaznikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(polaznikId))
                return RedirectToAction(nameof(Index));

            //proveravamo da li je prijavljen na taj kurs
            var jePrijavljen = await _context.Prijave.AnyAsync(p => p.KursId == id && p.PolaznikId == polaznikId);
            //ako je true prijavljen je i ne moze opet da se prijavi
            //ako nije prijavljen/upisan, upisuje se
            if (!jePrijavljen)
            {
                var prijava = new Prijava
                {
                    KursId = id,
                    PolaznikId = polaznikId
                };
                _context.Prijave.Add(prijava);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Details), new { id = id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Roles.Polaznik))]
        public async Task<IActionResult> IspisiSe(int id)
        {
            var polaznikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(polaznikId))
                return RedirectToAction(nameof(Index));

            //trazimo prijavu
            var prijava = await _context.Prijave.FirstOrDefaultAsync(p => p.KursId == id && p.PolaznikId == polaznikId);

            //ako postoji prijava brisemo je
            if (prijava != null)
            {
                _context.Prijave.Remove(prijava);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = id });

        }

        [Authorize(Roles = nameof(Roles.Instruktor))]
        public async Task<IActionResult> MojiKursevi()
        {
            //dohvatamo id instruktora koji je ulogovan
            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(instruktorId))
                return RedirectToAction(nameof(Index));

            //dohvatamo sve kurseve tog instruktora
            var kursevi = await _context.Kursevi
                .Where(k => k.InstruktorId == instruktorId)
                .OrderBy(k => k.Naziv)
                .ToListAsync();

            var kursId = kursevi.Select(k => k.Id).ToList();

            //neka ideja je da prikazemo koliko polaznika se prijavilo na taj njegov kurs
            var brojPrijava = await _context.Prijave
                .Where(p => kursId.Contains(p.KursId))
                .GroupBy(p => p.KursId)
                .Select(b => new { Kursid = b.Key, Broj = b.Count() })
                .ToDictionaryAsync(x => x.Kursid, x => x.Broj);

            //mapiranje
            var model = kursevi.Select(k => new InstruktorKursViewModel
            {
                Kurs = k,
                BrojPrijavljenih = brojPrijava.GetValueOrDefault(k.Id)
            }).ToList();
            return View(model);
        }

        [Authorize(Roles = nameof(Roles.Instruktor))]
        public async Task<IActionResult> IzmeniKurs(int? id)
        {
            if (id == null)
                return NotFound();
            //dohvatamo id ulogovanog instruktora
            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //dohvatamo kurs koji zeli da menja
            var kurs = await DohvatiKurs(id.Value, instruktorId);
            if (kurs == null)
                return NotFound();

            return View(kurs);

        }

        //pomocna metoda koja vraca kurs
        private async Task<Kurs?> DohvatiKurs(int id, string? instruktorId)
        {
            if (string.IsNullOrWhiteSpace(instruktorId))
                return null;

            return await _context.Kursevi
                .FirstOrDefaultAsync(k => k.Id == id && k.InstruktorId == instruktorId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Roles.Instruktor))]
        public async Task<IActionResult> IzmeniKurs(int id, [Bind("Id,Naziv,Opis,DatumPocetka,DatumZavrsetka,MaksimalanBrojPolaznika")] Kurs kurs)
        {
            if (id != kurs.Id)
                return NotFound();

            //dohvatamo instruktora koji menja kurs
            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //dohvatamo kurs
            var k = await _context.Kursevi.FindAsync(id);

            if (k == null || k.InstruktorId != instruktorId)
                return NotFound();

            k.Naziv = kurs.Naziv;
            k.Opis = kurs.Opis;
            k.DatumPocetka = kurs.DatumPocetka;
            k.DatumZavrsetka = kurs.DatumZavrsetka;
            k.MaksimalanBrojPolaznika = kurs.MaksimalanBrojPolaznika;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MojiKursevi));
        }


        [Authorize(Roles = nameof(Roles.Instruktor))]
        public async Task<IActionResult> UkloniKurs(int? id)
        {
            if (id == null)
                return NotFound();
            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kurs = await DohvatiKurs(id.Value, instruktorId);
            if (kurs == null)
                return NotFound();

            //Pre nego sto obrise kurs ne bi bilo lose da vidi broj polaznika na tom kursu
            var brojPrijavljenih = await _context.Prijave.CountAsync(p => p.KursId == kurs.Id);
            ViewBag.BrojPrijavljenih = brojPrijavljenih;

            return View(kurs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Roles.Instruktor))]
        public async Task<IActionResult> UkloniKurs(int id)
        {
            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kurs = await DohvatiKurs(id, instruktorId);
            if (kurs == null)
                return NotFound();

            _context.Kursevi.Remove(kurs);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MojiKursevi));
        }

        [Authorize(Roles = nameof(Roles.Instruktor))]
        public async Task<IActionResult> PolazniciNaKursu(int? id)
        {
            if (id == null)
                return NotFound();

            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var kurs = await DohvatiKurs(id.Value, instruktorId);
            if (kurs == null)
                return NotFound();

            var prijave = await _context.Prijave
                .AsNoTracking()//ne cuva objekte u memoriji,samo mapira u view model radi prikaza na view-u
                .Where(p => p.KursId == id.Value)
                .Include(p => p.Polaznik)
                .OrderBy(p => p.DatumPrijave)
                .ToListAsync();

            var model = new KursPolazniciViewModel
            {
                Kurs = kurs,
                Polaznici = prijave.Select(p => new PolaznikNaKursuViewModel
                {
                    Ime = p.Polaznik.Ime,
                    Prezime = p.Polaznik.Prezime,
                    Email = p.Polaznik.Email,
                    DatumPrijave = p.DatumPrijave
                }).ToList()
            };

            return View(model);
        }


        [Authorize(Roles = nameof(Roles.Instruktor))]
        public IActionResult DodajLekciju(int id)
        {
            var lekcija = new Lekcija
            {
                KursId = id
            };

            return View(lekcija);
        }


        [Authorize(Roles = nameof(Roles.Instruktor))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DodajLekciju(int id,[Bind("Naziv,Sadrzaj")] Lekcija lekcija)
        {
            lekcija.KursId = id;
            if (!ModelState.IsValid)
                return View(lekcija);
            _context.Add(lekcija);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MojiKursevi));
        }

        [Authorize(Roles = nameof(Roles.Instruktor))]
        public async Task<IActionResult> LekcijeMaterijali(int id)
        {
            var instruktorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(instruktorId))
                return Forbid();

            var kurs = await _context.Kursevi
                .Include(k => k.Lekcije)
                    .ThenInclude(l => l.Materijali)
                .FirstOrDefaultAsync(k => k.Id == id);

            if (kurs == null)
                return NotFound();

            if (kurs.InstruktorId != instruktorId)
                return Forbid();

            return View(kurs);
        }
    }
}
