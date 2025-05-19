using Miljöboven.Models;

// ViewModel för de ärenden handläggare ser, med listor för filtrering

namespace Miljoboven4.ViewModels
{
    public class InvestigatorViewModel
    {
        public IEnumerable<ErrandStatus> ErrandStatuses { get; set; }
        public IEnumerable<ErrandViewModel> Errands { get; set; }
    }
}
