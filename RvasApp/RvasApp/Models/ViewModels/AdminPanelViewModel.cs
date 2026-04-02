namespace RvasApp.Models.ViewModels
{
    public class AdminPanelViewModel
    {
        public int BrojKorisnika { get; set; }
        public int BrojPolaznika { get; set; }
        public int BrojInstruktora { get; set; }
        public List<AdminUserViewModel> Korisnici { get; set; } = new List<AdminUserViewModel>();
    }
}
