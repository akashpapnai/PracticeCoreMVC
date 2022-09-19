using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticeCoreMVC.Contexts;
using PracticeCoreMVC.Data;
using PracticeCoreMVC.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Principal;

namespace PracticeCoreMVC.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly AllRepoContext _repocontext;
        public AccountController(AllRepoContext repocontext)
        {
            _repocontext = repocontext;
        }
        #region LoginAndRegister
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var userExists = await _repocontext.Register.Select(x => x).Where(x => (x.UserName == loginModel.UserName || x.Email == loginModel.UserName) && x.Password == loginModel.Password).AnyAsync();
            if(userExists)
            {
                var userDetails = await _repocontext.Register.Select(x => x).Where(x => (x.UserName == loginModel.UserName || x.Email == loginModel.UserName) && x.Password == loginModel.Password).FirstOrDefaultAsync();
                var Roledetail = await _repocontext.UserRoleMapping.Select(x => x).Where(x => x.UserId == userDetails.Id).FirstOrDefaultAsync();
                var role = await _repocontext.Roles.Select(x => x).Where(x => x.Id == Roledetail.RoleId).FirstOrDefaultAsync();
                var Identity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginModel.UserName),
                    new Claim("FullName", loginModel.UserName),
                    new Claim(ClaimTypes.Role, role.Role)
                },CookieAuthenticationDefaults.AuthenticationScheme);
                var Principal = new ClaimsPrincipal(Identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, Principal);
                return RedirectToAction("Index","Home");
            }
            TempData["LoginError"] = "UserName or Password is Incorrect";
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
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
            // TODO: Try to Implement Forgot Password corrected via Email
            return RedirectToAction("Login","Account");
        }
        #endregion
        
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ChangeRole()
        {
            var roleslist = await _repocontext.Roles.Select(x => x).ToListAsync();
            var userslist = await _repocontext.Register.Select(x => x).ToListAsync();
            return View(new AllModels()
            {
                roleModelList = roleslist,
                registerModelList = userslist
            });
        }
        [Authorize(Roles = "Admin, Manager")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string username, string role)
        {
            var identity = (ClaimsIdentity)User.Identity;
            List<Claim> claims = (List<Claim>)identity.Claims;
            var roledata = await _repocontext.Roles.SingleAsync(x => x.Role == role);
            var userdetails = await _repocontext.Register.SingleAsync(x => x.UserName == username);
            var map = await _repocontext.UserRoleMapping.SingleAsync(x => x.UserId == userdetails.Id);
            map.RoleId = roledata.Id;
            await _repocontext.SaveChangesAsync();
            if (claims[1].Value == username)
            {
                await Logout();
            }
            return RedirectToAction("Index","Home");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddRole()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddRole(string Role)
        {
            Validation validation = new Validation();
            if(string.IsNullOrWhiteSpace(Role) || validation.containsNumeralOrSpecialCharacters(Role))
            {
                TempData["InvalidRole"] = $"'{Role}' is Invalid Name";
                return View();
            }
            var roledata = await _repocontext.Roles.SingleOrDefaultAsync(x => x.Role.ToLower() == Role.ToLower());
            if(roledata != null)
            {
                TempData["RoleExistsError"] = $"{Role} Role Already Exists";
                return View();
            }
            await _repocontext.Roles.AddAsync(new RoleModel()
            {
                Id = new Guid(),
                Role = Role
            });
            await _repocontext.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }
    }
}
