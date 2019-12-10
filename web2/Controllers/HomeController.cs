using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using web2.Models;

namespace web2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;

        public Profile Profile { get; }

        public HomeController(ILogger<HomeController> logger, IOptionsSnapshot<Profile> ProfileOpt)
        {
            this.logger = logger;
            this.Profile = ProfileOpt.Value;
        }

        public IActionResult Index()
        {
            return View(Profile);
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
