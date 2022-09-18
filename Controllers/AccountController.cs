using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeCoreMVC.Contexts;
using PracticeCoreMVC.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace PracticeCoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly AllRepoContext _repocontext;
        public AccountController(AllRepoContext repocontext)
        {
            _repocontext = repocontext;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var userExists = await _repocontext.Register.Select(x => x).Where(x => (x.UserName == loginModel.UserName || x.Email == loginModel.UserName) && x.Password == loginModel.Password).AnyAsync();
            if(userExists)
            {
                var userDetails = await _repocontext.Register.Select(x => x).Where(x => (x.UserName == loginModel.UserName || x.Email == loginModel.UserName) && x.Password == loginModel.Password).FirstOrDefaultAsync();
                var Roledetail = await _repocontext.UserRoleMapping.Select(x => x).Where(x => x.UserId == userDetails.Id).FirstOrDefaultAsync();
                var role = await _repocontext.Roles.Select(x => x).Where(x => x.Id == Roledetail.RoleId).FirstOrDefaultAsync();
                var Identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, loginModel.UserName),
                    new Claim(ClaimTypes.Role, role.Role)
                },CookieAuthenticationDefaults.AuthenticationScheme);
                var Principal = new ClaimsPrincipal(Identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Principal);
                return RedirectToAction("Index","Home");
            }
            TempData["LoginError"] = "UserName or Password is Incorrect";
            return RedirectToAction("Login","Account");
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var userExists = await _repocontext.Register.Select(x => x).Where(x => x.UserName == registerModel.UserName || x.Email == registerModel.Email).AnyAsync();
            if(userExists)
            {
                TempData["UserAlreadyExistsError"] = "User Already Exists";
                return View();
            }
            await _repocontext.AddAsync(registerModel);
            await _repocontext.SaveChangesAsync();
            var user = await _repocontext.Register.Select(x => x).Where(x => x.UserName == registerModel.UserName && x.Email == registerModel.Email).FirstOrDefaultAsync();
            var role = await _repocontext.Roles.Select(x => x).Where(x => x.Role == "User").FirstOrDefaultAsync();
            await _repocontext.UserRoleMapping.AddAsync(new UserRoleMappingModel()
            {
                Id = new Guid(),
                UserId = user.Id,
                RoleId = role.Id
            });
            await _repocontext.SaveChangesAsync();
            return RedirectToAction("Login","Account");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var userExists = await _repocontext.Register.Select(x => x).Where(x=> x.Email == email).AnyAsync();
            if(!userExists)
            {
                TempData["UserNotExistsError"] = $"User with the email '{email}' does not Exists";
                return View();
            }
            // Not Implemented
            return RedirectToAction("Login","Account");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
