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

            //admin
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

            //instruktor
            var instruktor = new Korisnik
            {
                UserName = "Instruktor1",
                Email = "instruktort1@rvas.rs",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            var instruktor_db = await userManager.FindByEmailAsync(instruktor.Email);
            if(instruktor_db == null)
            {
                await userManager.CreateAsync(instruktor, "Admin123$");
                await userManager.AddToRoleAsync(instruktor, Roles.Instruktor.ToString());
            }


        }

        //predefinisani kursevi
        public static async Task SeedKursAsync(ApplicationDbContext context,UserManager<Korisnik> userManager)
        {
            if (!context.Kursevi.Any())
            {
                var instruktor = await userManager.FindByEmailAsync("instruktort1@rvas.rs");
                if (instruktor == null)
                    return;

                var kursevi = new List<Kurs>() {
                    new Kurs
                    {
                        Naziv = "Adobe InDesign",
                        Opis = "U okviru ovog kursa, polaznici će se upoznati sa Adobe InDesignom, najkvalitetnijim profesionalnim programom za stono izdavaštvo.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(1),
                        MaksimalanBrojPolaznika = 23,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "Adobe Illustrator",
                        Opis = "Adobe Illustrator predstavlja profesionalni standard programa za vektorsko crtanje, štampu, multimediju i onlajn grafiku. U okviru ovog kursa, polaznici će se upoznati sa Adobe Illustratorom, najkvalitetnijim programom za vektorsko crtanje.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(2),
                        MaksimalanBrojPolaznika = 10,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "Adobe Photoshop",
                        Opis = "Adobe Photoshop je neophodan alat za svakog dizajnera, bez koga je praktično nemoguće baviti se profesionalno dizajnom, fotografijom i izradom digitalnih umetničkih slika.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(3),
                        MaksimalanBrojPolaznika = 15,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "Web developer",
                        Opis = "Web developer je koncipiran tako da kroz predavanja i vežbe u učionici i samostalni rad pod tutorstvom naših iskusnih predavača naučite i savladate sve potrebne veštine za uspešan start svoje karijere.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(4),
                        MaksimalanBrojPolaznika = 35,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "WordPress",
                        Opis = "roz ovu obuku naučićete da samostalno kreirate portfolio web sajt, blog, web sajt preduzeća, katalog proizvoda. Moći ćete i da održavate i unapređujete postojeće WordPress sajtove u skladu sa potrebama klijenata.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(1),
                        MaksimalanBrojPolaznika = 5,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "CompTIA A+",
                        Opis = "Na ovom kursu ćete steći osnovne informacije i veštine koje su vam neophodne da instalirate, nadogradite, popravite, konfigurišete, optimizujete, ustanovite i otklonite probleme na personalnim računarima,mreži i operativnim sistemima.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(1),
                        MaksimalanBrojPolaznika = 25,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "Java osnovni kurs",
                        Opis = "Kurs vam pruža osnovu za učenje Java SE (Standard Edition), tako da možete napravite svoje prve aplikacije ili proširite vaše  znanje ukoliko imate određeno iskustvo u programiranju.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(2),
                        MaksimalanBrojPolaznika = 15,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "Excel Napredni",
                        Opis = "Ukoliko svakodnevno baratate velikim brojem podataka ili informacija, razmišljate kako da bolje organizujete svoje poslovanje ili se često pitate postoji li efikasniji način da se obavi neki komplikovaniji proračun – napredni kurs Microsoft Excela pružiće vam praktična i odmah primenljiva rešenja.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(2),
                        MaksimalanBrojPolaznika = 15,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "Excel Osnovni",
                        Opis = "Microsoft Excel je i izvanredna alatka za analziranje i vizuelizaciju podataka. Prvenstveno je namenjen poslovnom okruženju, kada želite da jasno i precizno predstavite podatke sebi i drugima.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(3),
                        MaksimalanBrojPolaznika = 10,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "Word Osnovni",
                        Opis = "Kao jedan od najstarijih programa za pisanje, Word je postavio čitav niz principa koji su postali standard za slične programe, te se znanja stečena tokom ovog kursa odnose kako na sve verzije samog Microsoft Worda, tako i na praktično sve slične programe za unos i obradu teksta.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(3),
                        MaksimalanBrojPolaznika = 40,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "Osnovni kurs računara",
                        Opis = "Upotreba računara više se ne smatra posebnom veštinom, već osnovnom pismenošću, a sa ubrazanim razvojem tehnologija, sami programi postaju sve jednostavniji za upotrebu, ali se zato platforme umnožavaju i svakodnevno inoviraju. Zato je ovaj kurs namenjen baš svima – ukoliko želite da unapredite svoja znanja i veštine kako rada na računarima, tako i mobilnih platofrmi, smart tehnologija i tableta.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(3),
                        MaksimalanBrojPolaznika = 5,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "MS EXCEL – POWER PIVOT",
                        Opis = "Kurs „Analyzing Data with Excel“ nas detaljno provodi kroz značaj korišćenja PowerPivot modela u MS Excel 2016, kao i nedostatke i limite dosadašnjih klasičnih pivot tabela.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(3),
                        MaksimalanBrojPolaznika = 20,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "20761 Querying Data with Transact-SQL",
                        Opis = "Kurs je namenjen svima koji u svom radu koriste velike skupine podataka koje moraju da obrađuju i prave razne vrste izveštaja.\r\n\r\nKurs je namenjen i profesionalcima koji rade sa bazama podataka na Microsoft SQL Serveru, programerima aplikacija koje pristupaju relacionim bazama i kreatorima BI (Business Inteligence) sistema. Predstavlja početni kurs za Microsoft SQL Server i osnovu neophodnu za sve naredne kurseve.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(1),
                        MaksimalanBrojPolaznika = 30,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "20762 Developing SQL Databases",
                        Opis = "Ovaj kurs pruža znanja i veštine potrebne za kreiranje baza podataka na Microsoft SQL Serveru 2016. Namenjen je IT Profesionalcima koji žele da nauče da dizajniraju i kreiraju baze podataka, njihove objekte, obezbede integritet podataka, kreiraju brojne programske elemente i manipulišu sa XML podacima u okviru SQL Servera.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(1),
                        MaksimalanBrojPolaznika = 20,
                        InstruktorId = instruktor.Id
                    },
                    new Kurs
                    {
                        Naziv = "20486B Developing ASP.NET MVC 5 Web Applications",
                        Opis = "Tokom ovog Microsoftovog zvaničnog kursa, polaznici se upoznaju sa razvojem naprednih ASP.NET MVC aplikacija korišćenjem .NET FRameworka 4.5. Fokus je na razvoju koda koji poboljšava performanse i prilagodljivost jedne web aplikacije. Tokom kursa, razvoj ASP.NET MVC  se uči upoređujući sa Web Forms aplikacijom, kako bi polaznici u stvarnim uslovima znali kada je prikladno koristiti koju od navedene dve tehnologije razvoja Web aplikacije.",
                        DatumPocetka = DateTime.Now,
                        DatumZavrsetka = DateTime.Now.AddMonths(2),
                        MaksimalanBrojPolaznika = 10,
                        InstruktorId = instruktor.Id
                    }
                };

                await context.Kursevi.AddRangeAsync(kursevi);
                await context.SaveChangesAsync();
            }
        }
    }
}
