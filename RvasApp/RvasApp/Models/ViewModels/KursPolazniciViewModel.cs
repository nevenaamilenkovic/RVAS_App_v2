namespace RvasApp.Models.ViewModels
{
    public class KursPolazniciViewModel
    {
        public Kurs Kurs { get; set; } = null!;
        public IList<PolaznikNaKursuViewModel> Polaznici { get; set; }=new List<PolaznikNaKursuViewModel>();
    }
}
