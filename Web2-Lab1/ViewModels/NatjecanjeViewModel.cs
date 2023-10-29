using System.Collections.Generic;
using SampleMvcApp.Data.Entities;

namespace SampleMvcApp.ViewModels
{
    public class NatjecanjeViewModel
    {
        public Natjecanje Natjecanje { get; set; }
        public List<Natjecatelj> Natjecatelji { get; set; }
        public List<Rezultat> Rezultati { get; set; }
    }
}
