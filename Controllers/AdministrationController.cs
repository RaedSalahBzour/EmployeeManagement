using EmployeeProject.Models;
using EmployeeProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EmployeeProject.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize(Policy = "AdminRolePolicy")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AdministrationController> _logger;

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AdministrationController>logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user= await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"The Id : {userId} cannot be found";
                return View("NotFoundError");
            }
            var existiongUserClaims=await _userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = userId,
            };
            foreach (Claim claim in ClaimsStore.AllClaims )
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                };
                if (existiongUserClaims.Any(c=>(c.Type == claim.Type) && (c.Value == "true")))
                {
                    userClaim.IsChecked= true;
                }
                model.Claims.Add(userClaim);
            }
                return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"The Id : {model.UserId} cannot be found";
                return View("NotFoundError");
            }
            var claims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }
            result = await _userManager.AddClaimsAsync(user,
                model.Claims.Select(c => new Claim ( c.ClaimType, c.IsChecked?"true":"false" )));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected claims to user");
                return View(model);
            }
            return RedirectToAction("EditUser",new {Id=model.UserId});

        }


        [HttpGet]
        public async Task<IActionResult>ManageUserRoles(string userId)
        {
            ViewBag.UserId = userId;
            var user=await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"The Id : {userId} cannot be found";
                return View("NotFoundError");
            }
            var model=new List<UserRolesViewModel>();
            foreach (var role in _roleManager.Roles.ToList())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsChecked=await _userManager.IsInRoleAsync(user,role.Name)
                };
                
                model.Add(userRolesViewModel);

            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel>model,string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
            {
                ViewBag.ErrorMessage = $"The Id : {userId} cannot be found";
                return View("NotFoundError");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user,
             model.Where(x => x.IsChecked).Select(y => y.RoleName));
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("EditUser",new {Id=userId});


        }
        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult>DeleteRole(string id)
        { 
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"The Id : {id} cannot be found";
                return View("NotFoundError");
            }
            else
            {
                try
                {

                    var result = await _roleManager.DeleteAsync(role);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Roles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("Roles");
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError($"Exception Occured : {ex}");
                    ViewBag.ErrorTitle = $"{role.Name} role is in use";
                    ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role." +
                        $" If you want to delete this role, " +
                        $"please remove the users from the role and then try to delete";
                    return View("Error");
                }
            }

        }
        [HttpGet]
        public IActionResult Users()
        {
            var users = _userManager.Users;
            return View(users);
        }
        [HttpGet]
        [Authorize(Policy = "CreateRolePolicy")]
        public IActionResult CreateRole() 
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "CreateRolePolicy")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName,
                };
                IdentityResult result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", "administration");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Roles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"The Id : {id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name

            };
            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"The Id : {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims.Select(c =>c.Type+" : "+ c.Value).ToList(),
                Roles = userRoles
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return View("NotFoundError");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with id : {roleId} cannot be found";
                return View("NotFoundError");
            }
            var model = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsChecked = true;
                }
                else
                {
                    userRoleViewModel.IsChecked = false;

                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel>model,string roleId)
        {
            var role=await _roleManager.FindByIdAsync(roleId);
            if (role is null)
            {
                ViewBag.ErrorMessage = $"Role with id : {roleId} cannot be found";
                return View("NotFoundError");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                
                IdentityResult result = null;
                if (model[i].IsChecked && !(await _userManager.IsInRoleAsync(user,role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsChecked && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);

                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i<model.Count-1)
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { id = roleId });
                    }
                }
               
            }

            return RedirectToAction("EditRole", new { id = roleId });

        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                ViewBag.ErrorMessage = $"The Id : {id} cannot be found";
                return View("NotFoundError");
            }
            else
            {

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View("Users");
            }

        }

    }
}
