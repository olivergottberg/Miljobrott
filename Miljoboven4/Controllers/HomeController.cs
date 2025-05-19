using Microsoft.AspNetCore.Mvc;
using Miljöboven.Models;
using Miljoboven4.Infrastructure;

// Controller för Index-sidan, session gör att ifylld data sparas

namespace Miljöboven.Controllers;

public class HomeController : Controller
{
    // GET
    public ViewResult Index()
    {
        var myErrand = HttpContext.Session.Get<Errand>("NewErrand");
        if (myErrand == null)
        {
            return View();
        }
        else
        { 
            return View(myErrand);
        }
    }
}