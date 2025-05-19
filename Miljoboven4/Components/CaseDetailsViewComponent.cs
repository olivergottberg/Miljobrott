using Microsoft.AspNetCore.Mvc;
using Miljöboven.Models;

// ViewComponent som används på detaljsidan för ett ärende

namespace Miljoboven4.Components
{
	public class CaseDetailsViewComponent : ViewComponent
	{
		private readonly IEnvironmentRepository repository;

		public CaseDetailsViewComponent(IEnvironmentRepository repo)
		{
			repository = repo;
		}

		public IViewComponentResult Invoke(int Id)
		{
			var errand = repository.GetErrandDetail(Id);
			return View("CaseDetails", errand);
		}
	}
}
