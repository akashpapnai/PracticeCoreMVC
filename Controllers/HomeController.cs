using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PracticeCoreMVC.Models;
using PracticeCoreMVC.Services;
using System.Data;
using System.Diagnostics;

namespace PracticeCoreMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CommonClass cmnCls;

        public HomeController(ILogger<HomeController> logger, CommonClass Common)
        {
            _logger = logger;
            cmnCls = Common;
        }

        public IActionResult Index()
        {
            DataTable? RoleTable = cmnCls.GetDataByQuery("Select * from Register");
            if(RoleTable != null) {
                return View(RoleTable);
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}