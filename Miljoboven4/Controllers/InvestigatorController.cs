using Microsoft.AspNetCore.Mvc;
using Miljöboven.Models;
using Microsoft.AspNetCore.Authorization;
using Miljoboven4.ViewModels;

// Controller för handläggaren, endast åtkomlig med rätt roll

namespace Miljöboven.Controllers;

[Authorize(Roles = "Investigator")]
public class InvestigatorController : Controller
{
	// GET
	private readonly IEnvironmentRepository repository;

	private IWebHostEnvironment environment;

	public InvestigatorController(IEnvironmentRepository repo, IWebHostEnvironment env)
	{
		repository = repo;
		environment = env;
	}

	// Detaljsidan för ärende
	public ViewResult CrimeInvestigator(int id)
    {
		ViewBag.Id = id;
		ViewBag.StatusList = repository.ErrandStatuses.ToList();
		var errand = repository.GetErrandDetail(id);
		ViewBag.StatusId = errand.StatusId;
		return View(errand);
	}

    // Startsidan där alla ärenden syns, använder ViewModel och hämtar ärenden beroende på vem som loggar in
    public ViewResult StartInvestigator()
    {
		var viewModel = new InvestigatorViewModel
		{
			ErrandStatuses = repository.ErrandStatuses,
			Errands = repository.GetInvestigatorErrands()
		};
        return View(viewModel);
    }

    // När man använder filtreringen, kontrollerar mot ärendenummer först
    [HttpPost]
	public IActionResult StartInvestigator(string status, string casenumber)
	{
		var errands = repository.GetInvestigatorErrands();

		if(!string.IsNullOrEmpty(casenumber))
		{
			errands = errands.Where(err => err.RefNumber.ToString() == casenumber);
		}
		else if (status != "Välj alla")
		{
			errands = errands.Where(err => err.StatusName == status);
		}

		var viewModel = new InvestigatorViewModel
		{
			ErrandStatuses = repository.ErrandStatuses,
			Errands = errands
		};

		return View(viewModel);
	}

    // När handläggaren uppdaterar ett ärende
    [HttpPost]
	public async Task<IActionResult> UpdateErrand(Errand errand, string InvestigatorInfo, string InvestigatorAction, 
		IFormFile evidenceSample, IFormFile evidencePicture)
	{
		var oldErrand = repository.GetErrandDetail(errand.ErrandId);

		if (errand != null)
		{
			oldErrand.InvestigatorInfo += InvestigatorInfo + " ; ";
			oldErrand.InvestigatorAction += InvestigatorAction + " ; ";

			if (errand.StatusId != "Välj")
			{
				oldErrand.StatusId = errand.StatusId;
			}

			await AddSampleToErrand(oldErrand, evidenceSample);
			await AddPictureToErrand(oldErrand, evidencePicture);

			repository.SaveErrand(oldErrand);
		}
		return RedirectToAction("StartInvestigator");
	}

	// När handläggaren lägger till bevis i ärendet
	private async Task AddSampleToErrand(Errand errand, IFormFile evidenceSample)
	{
		if (evidenceSample != null && evidenceSample.Length > 0)
		{
			var sampleName = await SaveFile(evidenceSample, "evidenceSamples");
			if (!string.IsNullOrEmpty(sampleName))
			{
				errand.Samples ??= new List<Sample>();
				errand.Samples.Add(new Sample { SampleName = sampleName, ErrandID = errand.ErrandId });
			}
		}
	}

    // När handläggaren lägger till en bild i ärendet
    private async Task AddPictureToErrand(Errand errand, IFormFile evidencePicture)
	{
		if (evidencePicture != null && evidencePicture.Length > 0)
		{
			var pictureName = await SaveFile(evidencePicture, "evidencePictures");
			if (!string.IsNullOrEmpty(pictureName))
			{
				errand.Pictures ??= new List<Picture>();
				errand.Pictures.Add(new Picture { PictureName = pictureName, ErrandID = errand.ErrandId });
			}
		}
	}


	// Spara filen
	private async Task<String> SaveFile(IFormFile evidence, string targetFolderName)
	{
		var tempPath = Path.GetTempFileName();

		using (var stream = new FileStream(tempPath, FileMode.Create))
		{
			await evidence.CopyToAsync(stream);
		}

		string uniqueFileName = Guid.NewGuid().ToString() + "_" + evidence.FileName;

		var path = Path.Combine(environment.WebRootPath, targetFolderName, uniqueFileName);

		System.IO.File.Move(tempPath, path);

		return uniqueFileName;
	}
}