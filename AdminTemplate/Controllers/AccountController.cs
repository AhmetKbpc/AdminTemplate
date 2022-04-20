
using AdminTemplate.Models.Email;
using AdminTemplate.Models.Identity;
using AdminTemplate.Models.Role;
using AdminTemplate.Services.Email;
using AdminTemplate.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace AdminTemplate.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _emailService = emailService;
        _roleManager = roleManager;
        _signInManager = signInManager;
        CheckRoles();
    }

    private void CheckRoles()
    {
        foreach (var item in Roles.RoleList)
        {
            if (_roleManager.RoleExistsAsync(item).Result)
                continue;
            var result = _roleManager.CreateAsync(new ApplicationRole()
            {
                Name = item
            }).Result;
        }
    }

    [HttpGet("~/Register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("~/Register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "Error!");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            Name = model.Name,
            Surname = model.Surname
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            //Rol Atama
            var count = _userManager.Users.Count();
            result = await _userManager.AddToRoleAsync(user, count == 1 ? Roles.Admin : Roles.Passive);

            ////Email gönderme - Aktivasyon
            //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code },
            //    protocol: Request.Scheme);

            //var email = new MailModel()
            //{
            //    To = new List<EmailModel>
            //    {
            //        new EmailModel()
            //            { Adress = user.Email, Name = user.UserName }
            //    },
            //    Body =
            //        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
            //    Subject = "Confirm your email"
            //};

            //await _emailService.SendMailAsync(email);
            //TODO: Login olma
            return RedirectToAction("Login");
        }

        var messages = string.Join("<br>", result.Errors.Select(x => x.Description));
        ModelState.AddModelError(string.Empty, messages);
        return View(model);
    }

    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        ViewBag.StatusMessage =
            result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

        if (result.Succeeded && _userManager.IsInRoleAsync(user, Roles.Passive).Result)
        {
            await _userManager.RemoveFromRoleAsync(user, Roles.Passive);
            await _userManager.AddToRoleAsync(user, Roles.User);
        }

        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.UserName);

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else if (result.IsLockedOut)
        {
            //TODO: Kilitlenmişse ne yapılacağı
        }
        else if (result.RequiresTwoFactor)
        {
            //TODO: 2fa yönlendirmesi yapılacak
        }
        ModelState.AddModelError(string.Empty, "Incorrect password or username!");
        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            ViewBag.Message = "Password change mail is sent to you.";
        }
        else
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("ConfirmResetPassword", "Account", new { userId = user.Id, code = code },
                Request.Scheme);

            var emailMessage = new MailModel()
            {
                To = new List<EmailModel>
                {
                    new EmailModel()
                        { Adress = user.Email, Name = user.UserName }
                },
                Body =
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.",
                Subject = "Reset Password"
            };

            await _emailService.SendMailAsync(emailMessage);

            ViewBag.Message = "Password update mail is sent to you.";
        }
        return View();
    }

    public IActionResult ConfirmResetPassword(string userId, string code)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
        {
            return BadRequest("Bad Request");
        }

        ViewBag.Code = code;
        ViewBag.UserId = userId;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "User coud not be found!");
            return View();
        }
        var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
        var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);
        if (result.Succeeded)
        {
            //email gönder
            TempData["Message"] = "Password change is successful.";
            return View();
        }
        else
        {
            var message = string.Join("<br>", result.Errors.Select(x => x.Description));
            TempData["Message"] = message;
            return View();
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name);
        var model = new UserProfileViewModel()
        {
            Email = user.Email,
            Name = user.Name,
            Surname = user.Surname
        };
        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Profile(UserProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity!.Name);
        user.Name = model.Name;
        user.Surname = model.Surname;
        user.Email = model.Email;

        bool isAdmin = await _userManager.IsInRoleAsync(user, Roles.Admin);
        if (isAdmin && user.Email != model.Email)
        {
            await _userManager.RemoveFromRoleAsync(user, Roles.User);
            await _userManager.AddToRoleAsync(user, Roles.Passive);
            user.EmailConfirmed = false;

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code },
                Request.Scheme);

            var emailMessage = new MailModel()
            {
                To = new List<EmailModel>
                {
                    new() {Adress = model.Email, Name = user.Name }
                },
                Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here </a>.",
                Subject = "Confirm your email"
            };
            await _emailService.SendMailAsync(emailMessage);
        }
        user.Name = model.Name;
        user.Surname = model.Surname;
        user.Email = model.Email;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            ViewBag.Message = "Update Successful!";
        }
        else
        {
            var message = string.Join("<br>", result.Errors.Select(x => x.Description));
            ViewBag.Message = message;
        }
        return View(model);
    }
}