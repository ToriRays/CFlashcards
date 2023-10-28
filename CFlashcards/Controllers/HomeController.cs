//using CFlashcards.Areas.Identity.Services.Email;
using CFlashcards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;

namespace CFlashcards.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<FlashcardsUser> _userManager;
        private readonly SignInManager<FlashcardsUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<FlashcardsUser> userManager, SignInManager<FlashcardsUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        public IActionResult Index()
        // This function redirects the user to two different landing pages depending on if the user is logged in or not.
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index1", "Home");
            }

            ViewData["UserID"]= _userManager.GetUserId(this.User); // User details of the user saved during login operation
            // The name in the return view is needed for unit tests.
            return View("Index");
        }

        public IActionResult Index1()
        {
            return View("Index1");
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