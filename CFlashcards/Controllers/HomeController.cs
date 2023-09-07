﻿using CFlashcards.Areas.Identity.Data;
using CFlashcards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CFlashcards.Controllers
{
    [Authorize] // so the userID will be showned only when we looged in, otherwise redirection to LogReg box on the welcome page
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger; //for ILogger down there it is coresponding private readonly property which is assigned within parameter logger in brackets
        private readonly UserManager<FlashcardsUser> _userManager;  

        public HomeController(ILogger<HomeController> logger, UserManager<FlashcardsUser> userManager)
        {
            _logger = logger;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
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