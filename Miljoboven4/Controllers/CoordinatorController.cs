using Microsoft.AspNetCore.Mvc;
using Miljöboven.Models;
using Miljoboven4.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Miljoboven4.ViewModels;

// Controller för samordnare, endast åtkomlig med rätt roll

namespace Miljöboven.Controllers;

[Authorize(Roles = "Coordinator")]
public class CoordinatorController : Controller
{
	// GET
	private readonly IEnvironmentRepository repository;

	public CoordinatorController(IEnvironmentRepository repo)
	{
		repository = repo;
	}

    // Detaljsidan för ärende
	public ViewResult CrimeCoordinator(int id)
    {
		ViewBag.Id = id;
        ViewBag.ListOfDepartments = repository.Departments.ToList();
		var errand = repository.GetErrandDetail(id);
		return View(errand);
    }
    
    // Sidan där samordnare kan registrera ärende
    public ViewResult ReportCrime()
    {
        var myErrand = HttpContext.Session.Get<Errand>("NewCoordErrand");
        if (myErrand == null)
        {
            return View();
        }
        else
        {
            return View(myErrand);
        }
    }
    
    // Startsidan där alla ärenden syns, använder ViewModel
    public ViewResult StartCoordinator()
    {
        var viewModel = new CoordinatorViewModel
        {
            Errands = repository.GetAllErrands(),
            ErrandStatuses = repository.ErrandStatuses.ToList(),
            Departments = repository.Departments.ToList()
        };
        return View(viewModel);
    }

    // När man använder filtreringen, kontrollerar mot ärendenummer först
    [HttpPost]
    public IActionResult StartCoordinator(string status, string department, string casenumber)
    {
		var errands = repository.GetAllErrands();

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

			if (department != "Välj alla")
			{
				errands = errands.Where(err => err.DepartmentName == department);
			}
		}

		var viewModel = new CoordinatorViewModel
		{
			ErrandStatuses = repository.ErrandStatuses,
			Departments = repository.Departments,
			Errands = errands
		};

		return View(viewModel);
	}
    
    public ViewResult Thanks()
    {
        var errand = HttpContext.Session.Get<Errand>("NewCoordErrand");
        if (errand != null)
        {
            repository.SaveErrand(errand);
            ViewBag.RefNumber = errand.RefNumber;
            HttpContext.Session.Remove("NewCoordErrand");
        }
        return View();
    }
    
    public ViewResult Validate(Errand errand)
    {
        HttpContext.Session.Set<Errand>("NewCoordErrand", errand);
        return View(errand);
    }

    // När man uppdaterar vilken avdelning som ska hantera ett ärende
    [HttpPost]
    public IActionResult UpdateDepartment(Errand errand)
    {
        var oldErrand = repository.GetErrandDetail(errand.ErrandId);

        if(errand != null && errand.DepartmentId != "Välj")
        {
            oldErrand.DepartmentId = errand.DepartmentId;
            repository.SaveErrand(oldErrand);
        }
        return RedirectToAction("StartCoordinator");
    }
}





































