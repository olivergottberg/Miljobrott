using Microsoft.AspNetCore.Mvc;
using Miljoboven4.ViewModels;

// ViewComponent som används för tabellen med ärenden när man loggar in

namespace Miljoboven4.Components
{
	public class ErrandTableViewComponent : ViewComponent
	{
		//Tar in parametrar för att länka rätt beroende på vilken roll som loggat in

		public IViewComponentResult Invoke(IEnumerable<ErrandViewModel> errands, string controller, string action)
		{
			ViewBag.Controller = controller;
			ViewBag.Action = action;
			return View("ErrandTable", errands);
		}
	}
}
