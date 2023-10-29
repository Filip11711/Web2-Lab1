using SampleMvcApp.Data.Entities;

namespace SampleMvcApp.ViewModels
{
    public class RezultatViewModel
    {
        public string vrijednost { get; set; }
        public int ishod { get; set; }
        public int domacinId { get; set; }
        public string domacin {  get; set; }
        public int gostId { get; set; }
        public string gost {  get; set; }
    }
}
