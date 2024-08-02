using EmployeeProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
namespace EmployeeProject.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
	{
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public AccountController(UserManager<IdentityUser> userManager,SignInManager<IdentityUser>signInManager)
        {
			_userManager=userManager;
			_signInManager=signInManager;
				
        }
		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction ("Index","Home");
		}
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
		{
			return View();
		}	
		[HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user=new IdentityUser { UserName=model.Email, Email=model.Email };
				var result=await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToAction("Index","Home");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(model);
		} 
		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			return View();
		}
  //      [AcceptVerbs("Get","Post")]
  //      [AllowAnonymous]
  //      public async Task<IActionResult> IsEmailInUse(string email)
		//{
		//	var user = await _userManager.FindByEmailAsync(email);
		//	if (user is not null)
		//	{
  //              return Json($"this {email} is already in use");

		//	}
		//	else
		//	{
  //              return Json(true);

  //          }

  //      }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LogInViewModel model,string returnUrl=null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
				if (result.Succeeded)
				{
					if (!string.IsNullOrEmpty(returnUrl)&& Url.IsLocalUrl(returnUrl))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Home");
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
					return View(model);
				}
			}
			return View(model);
		}
	}
}
