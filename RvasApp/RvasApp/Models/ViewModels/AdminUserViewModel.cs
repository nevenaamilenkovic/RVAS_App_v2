namespace RvasApp.Models.ViewModels
{
    public class AdminUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string KorisnickoIme { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
