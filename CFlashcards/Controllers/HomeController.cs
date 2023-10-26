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
    // so the userID will be showned only when we looged in, otherwise redirection to LogReg box on the welcome page
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; //for ILogger down there it is coresponding private readonly property which is assigned within parameter logger in brackets
        private readonly UserManager<FlashcardsUser> _userManager;
        private readonly SignInManager<FlashcardsUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, UserManager<FlashcardsUser> userManager, SignInManager<FlashcardsUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //[Authorize]
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("BrowseDecks", "Deck");
            }

            ViewData["UserID"]= _userManager.GetUserId(this.User); //.User details of the user saved during login operation
            // The name in the return view is needed for unit tests.
            return View("Index");
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