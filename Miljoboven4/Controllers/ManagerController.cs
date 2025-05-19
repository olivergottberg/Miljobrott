using Microsoft.AspNetCore.Mvc;
using Miljöboven.Models;
using Microsoft.AspNetCore.Authorization;
using Miljoboven4.ViewModels;

// Controller för avdelningschef, endast åtkomlig med rätt roll

namespace Miljöboven.Controllers;

[Authorize(Roles = "Manager")]
public class ManagerController : Controller
{
	// GET
	private readonly IEnvironmentRepository repository;

    public ManagerController(IEnvironmentRepository repo)
    {
        repository = repo;
    }

    // Detaljsidan för ärende
    public ViewResult CrimeManager(int id)
    {
        ViewBag.Id = id;
        ViewBag.EmployeeList = repository.FilterEmployees();
		var errand = repository.GetErrandDetail(id);
        ViewBag.StatusId = errand.StatusId;
		return View(errand);
	}

    // Startsidan där alla ärenden syns, använder ViewModel och hämtar ärenden och anställda beroende på vem som loggar in
    public ViewResult StartManager()
    {
		var filteredEmployees = repository.FilterEmployees();
		var viewModel = new ManagerViewModel
        {
            ErrandStatuses = repository.ErrandStatuses,
            Employees = filteredEmployees,
            Errands = repository.GetManagerErrands()
        };
        return View(viewModel);
    }

    // När man använder filtreringen, kontrollerar mot ärendenummer först
    [HttpPost]
    public IActionResult StartManager(string status, string investigator, string casenumber)
    {
		var errands = repository.GetManagerErrands();
        var filteredEmployees = repository.FilterEmployees();

		if (!string.IsNullOrEmpty(casenumber))
		{
			errands = errands.Where(err => err.RefNumber.ToString() == casenumber);
		}
		else
		{
			if (status != "Välj alla")
			{
				errands = errands.Where(err => err.StatusName == status);
			}

			if (investigator != "Välj alla")
			{
				errands = errands.Where(err => err.EmployeeName == investigator);
			}
		}

		var viewModel = new ManagerViewModel
		{
			ErrandStatuses = repository.ErrandStatuses,
            Employees = filteredEmployees,
			Errands = errands
		};

		return View(viewModel);
	}

    // När chefen uppdaterar ett ärende
    [HttpPost]
    public IActionResult UpdateErrand(Errand errand, bool noAction, string Reason)
    {
		var oldErrand = repository.GetErrandDetail(errand.ErrandId);

        if (errand != null)
        {
            if (noAction)
            {
                oldErrand.StatusId = "S_B";
                oldErrand.EmployeeId = null;
                oldErrand.InvestigatorInfo = Reason;
            } else if (errand.EmployeeId != "Välj")
            {
                oldErrand.StatusId = "S_A";
                oldErrand.EmployeeId = errand.EmployeeId;
            }
            repository.SaveErrand(oldErrand);
        }
        return RedirectToAction("StartManager");
	}
}