using AutoMapper;
using EmployeeTracking.Core.EmailService;
using EmployeeTracking.Core.Entities;
using EmployeeTracking.Core.Entities.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using static EmployeeTracking.Core.Entities.Enums;

namespace EmployeeTracking.API.MVC.Controllers
{

    public class AdminController : BaseService
    {
        public AdminController(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, IMapper mapper, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, EmailHelper emailHelper) : base(userManager, roleManager, signInManager, mapper, actionContextAccessor, urlHelperFactory, emailHelper)
        {

        }
        public IActionResult Index() => View();

        public async Task<IActionResult> Users() => View(await _userManager.Users.ToListAsync());

        public async Task<IActionResult> Roles() => View(await _roleManager.Roles.ToListAsync());
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpsertRole(string? id)
        {
            if (id != null)
            {
                var role = await _roleManager.FindByIdAsync(id);
                return View(_mapper.Map<UpsertRoleModel>(role));
            }
            return View(new UpsertRoleModel());
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> UpsertRole(UpsertRoleModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var isUpdate = viewModel.Id != null;

                var role = isUpdate ? await _roleManager.FindByIdAsync(viewModel.Id) : new Role() { Name = viewModel.Name };

                if (isUpdate)
                {
                    role.Name = viewModel.Name;
                }

                var result = isUpdate ? await _roleManager.UpdateAsync(role) : await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
            }
            return View(viewModel);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Role not found.");
            }
            return RedirectToAction("Roles");
        }
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return RedirectToAction("Users");
            }
            var userModel = _mapper.Map<EditUserModel>(user);

            var userRoles = await _userManager.GetRolesAsync(user);
            userModel.Roles = await _roleManager.Roles.Select(s => new AssignRoleModel
            {
                RoleId = s.Id,
                RoleName = s.Name,
                IsAssigned = userRoles.Any(a => a == s.Name)
            }).ToListAsync();

            var userClaims = await _userManager.GetClaimsAsync(user);
            var departmentClaim = userClaims.FirstOrDefault(f => f.Type == "Department");
            if (departmentClaim != null)
            {
                userModel.Department = Enum.Parse<Department>(departmentClaim.Value);
            }

            return View(userModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserModel editUserModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUserModel.Id);
                if (user != null)
                {

                    user.UserName = editUserModel.UserName;
                    user.Email = editUserModel.Email;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        // Roles
                        foreach (var item in editUserModel.Roles)
                        {
                            if (item.IsAssigned)
                            {
                                await _userManager.AddToRoleAsync(user, item.RoleName);
                            }
                            else
                            {
                                await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                            }
                        }

                        // Claims
                        var userClaims = await _userManager.GetClaimsAsync(user);
                        var departmentClaim = userClaims.FirstOrDefault(a => a.Type == "Department");
                        var claimsToAdd = new Claim("Department", Enum.GetName(editUserModel.Department)!);
                        if (departmentClaim != null)
                        {
                            await _userManager.ReplaceClaimAsync(user, departmentClaim, claimsToAdd);
                        }
                        else
                        {
                            await _userManager.AddClaimAsync(user, claimsToAdd);
                        }

                        return RedirectToAction("Users");
                    }
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));

                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }

            return View(editUserModel);
        }
    }
}
