using Miljöboven.Models;

// ViewModel för de ärenden samordnare ser, med listor för filtrering

namespace Miljoboven4.ViewModels
{
    public class CoordinatorViewModel
    {
        public IEnumerable<ErrandStatus> ErrandStatuses { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<ErrandViewModel> Errands { get; set; }
    }
}
