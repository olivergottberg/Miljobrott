using Microsoft.AspNetCore.Mvc;
using Miljöboven.Models;
using Miljoboven4.Infrastructure;

// Controller som hanterar det medborgare ser, med session för tack och valideringssidan

namespace Miljöboven.Controllers;

public class CitizenController : Controller
{
    // GET
    private readonly IEnvironmentRepository repository;

    public CitizenController(IEnvironmentRepository repo)
    {
        repository = repo;
    }

    public ViewResult Contact()
    {
        return View();
    }
    
    public ViewResult Faq()
    {
        return View();
    }
    
    public ViewResult Services()
    {
        return View();
    }
    
    public ViewResult Thanks()
    {
        var errand = HttpContext.Session.Get<Errand>("NewErrand");
        if (errand != null)
        {
            repository.SaveErrand(errand);
            ViewBag.RefNumber = errand.RefNumber;
            HttpContext.Session.Remove("NewErrand");
        }
        return View();
    }
    
    public ViewResult Validate(Errand errand)
    {
        HttpContext.Session.Set<Errand>("NewErrand", errand);
        return View(errand);
    }
}