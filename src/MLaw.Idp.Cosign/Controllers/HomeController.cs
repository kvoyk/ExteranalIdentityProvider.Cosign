using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MLaw.Idp.Cosign.Models;

namespace MLaw.Idp.Cosign.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(IConfiguration confign,ILogger<HomeController> logger)
        {
      
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ErrorViewModel errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            _logger.LogError("{@ErrorViewModel}", errorViewModel);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
