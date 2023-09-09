using CFlashcards.Areas.Identity.Data;
using CFlashcards.Areas.Identity.Services.Email;
using CFlashcards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Diagnostics;

namespace CFlashcards.Controllers
{
    [Authorize] // so the userID will be showned only when we looged in, otherwise redirection to LogReg box on the welcome page
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; //for ILogger down there it is coresponding private readonly property which is assigned within parameter logger in brackets
        private readonly UserManager<FlashcardsUser> _userManager;
        private readonly IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger, UserManager<FlashcardsUser> userManager, IEmailSender emailSender)
        {
            _logger = logger;
            this._userManager = userManager;
            this._emailSender = emailSender;
        }

        public IActionResult Index()
        {


           // var receiver = "mail";
            //var subject = "Test";
            //var message = "Hello";

            //await _emailSender.SendEmailAsync(receiver, subject, message);

            ViewData["UserID"]= _userManager.GetUserId(this.User); //.User details of the user saved during login operation
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