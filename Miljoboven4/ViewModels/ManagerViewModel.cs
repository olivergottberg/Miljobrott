using Miljöboven.Models;

// ViewModel för de ärenden chefer ser, med listor för filtrering

namespace Miljoboven4.ViewModels
{
    public class ManagerViewModel
    {
        public IEnumerable<ErrandStatus> ErrandStatuses { get; set; }
        public IEnumerable<Employee> Employees { get; set; }
        public IEnumerable<ErrandViewModel> Errands { get; set; }
    }
}
