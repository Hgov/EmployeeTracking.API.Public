using AutoMapper;
using EmployeeTracking.Core.EmailService;
using EmployeeTracking.Core.Entities;
using EmployeeTracking.Core.Entities.DataTransferObjects;
using EmployeeTracking.Core.LoggerManager;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace EmployeeTracking.API.MVC.Controllers
{
    public class UserController : BaseService
    {
        public UserController(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, ILoggerManager loggerManager, IMapper mapper, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, EmailHelper emailHelper) : base(userManager, roleManager, signInManager, loggerManager, mapper, actionContextAccessor, urlHelperFactory, emailHelper)
        {

        }

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(SignUpModel signUpModel)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(signUpModel);
                var result = await _userManager.CreateAsync(user, signUpModel.Password);
                if (result.Succeeded)
                {
                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "User", new
                    {
                        userId = user.Id,
                        token = confirmationToken
                    }, HttpContext.Request.Scheme);

                    await _emailHelper.SendAsync(new()
                    {
                        Subject = "Confirm e-mail",
                        Body = $"Please <a href='{confirmationLink}'>click</a> to confirm your e-mail address.",
                        To = user.Email
                    });

                    return RedirectToAction("Login");
                }
                result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
            }
            return View(signUpModel);
        }

        public IActionResult Login(string? returnUrl)
        {
            if (returnUrl != null)
            {
                TempData["ReturnUrl"] = returnUrl;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(SignInModel signInModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(signInModel.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();

                    var result = await _signInManager.PasswordSignInAsync(user, signInModel.Password, signInModel.RememberMe, true);

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null);

                        var returnUrl = TempData["ReturnUrl"];
                        if (returnUrl != null)
                        {
                            return Redirect(returnUrl.ToString() ?? "/");
                        }
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (result.IsLockedOut)
                    {
                        var lockoutEndUtc = await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockoutEndUtc.Value - DateTime.UtcNow;
                        ModelState.AddModelError(string.Empty, $"This account has been locked out, please try again {timeLeft.Minutes} minutes later.");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "You need to confirm your e-mail address.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid e-mail or password.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid e-mail or password.");
                }
            }
            return View(signInModel);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public async Task Logout() => await _signInManager.SignOutAsync();

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.FindByNameAsync(User.Identity?.Name);
            if (user == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(_mapper.Map<UpdateProfileModel>(user));
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UpdateProfileModel updateProfileModel)
        {
            if (ModelState.IsValid)
            {
                var me = await _userManager.FindByNameAsync(User.Identity?.Name);
                if (me != null)
                {

                    me.UserName = updateProfileModel.UserName;
                    me.Email = updateProfileModel.Email;

                    var result = await _userManager.UpdateAsync(me);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(me);
                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(me, true);

                        return RedirectToAction("Index", "Admin");
                    }
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));

                }
                else
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(updateProfileModel);
        }

        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity?.Name);

                var passwordValid = await _userManager.CheckPasswordAsync(user, changePasswordModel.Password);
                if (passwordValid)
                {
                    var result = await _userManager.ChangePasswordAsync(user, changePasswordModel.Password, changePasswordModel.NewPassword);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);

                        await _signInManager.SignOutAsync();
                        await _signInManager.SignInAsync(user, true);

                        return RedirectToAction("Index", "Admin");
                    }
                    result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Password is invalid.");
                }
            }

            return View();
        }

        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
                if (user != null)
                {
                    var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordLink = Url.Action("ResetPassword", "User", new
                    {
                        userId = user.Id,
                        token = passwordResetToken
                    }, HttpContext.Request.Scheme);

                    await _emailHelper.SendAsync(new()
                    {
                        Subject = "Reset password",
                        Body = $"Please <a href='{passwordLink}'>click</a> to reset your password.",
                        To = user.Email
                    });

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(forgotPasswordModel);
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Login", "User");
            }

            return View(new ResetPasswordModel
            {
                UserId = userId,
                Token = token
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(resetPasswordModel.UserId);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.UpdateSecurityStampAsync(user);

                        return RedirectToAction("Login", "User");
                    }
                    else
                    {
                        result.Errors.ToList().ForEach(f => ModelState.AddModelError(string.Empty, f.Description));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(resetPasswordModel);
        }
    }
}
