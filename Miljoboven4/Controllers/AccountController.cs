using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Miljoboven4.Models;

// Controller som hanterar inloggningen och skickar användaren till rätt sida

namespace Miljoboven4.Controllers
{
    [Authorize]
	public class AccountController : Controller
	{
		private readonly UserManager<IdentityUser> userManager;
		private readonly SignInManager<IdentityUser> signInManager;

		public AccountController(UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr)
		{
			userManager = userMgr;
			signInManager = signInMgr;
		}

		// Loginsida tillåter alla
		[AllowAnonymous]
		public ViewResult Login(string returnUrl)
		{
			return View(new LoginModel
			{
				ReturnUrl = returnUrl
			});
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel loginModel)
		{
			if (ModelState.IsValid)
			{
				IdentityUser user = await userManager.FindByNameAsync(loginModel.UserName);
				if (user != null)
				{
					await signInManager.SignOutAsync();
					if((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded)
					{
						if(await userManager.IsInRoleAsync(user, "Investigator"))
						{
							return Redirect("/Investigator/StartInvestigator");
						}

                        if (await userManager.IsInRoleAsync(user, "Coordinator"))
						{
                            return Redirect("/Coordinator/StartCoordinator");
                        }

                        if (await userManager.IsInRoleAsync(user, "Manager"))
						{
                            return Redirect("/Manager/StartManager");
                        }
                    }
				}
			}
			ModelState.AddModelError("", "Felaktigt användarnamn eller lösenord");
			return View(loginModel);
        }

		public async Task<RedirectResult> Logout(string returnUrl = "/")
		{
			await signInManager.SignOutAsync();
			return Redirect(returnUrl);
		}

		// AccessDenied tillåter alla
		[AllowAnonymous]
		public ViewResult AccessDenied()
		{
			return View();
		}
	}
}
