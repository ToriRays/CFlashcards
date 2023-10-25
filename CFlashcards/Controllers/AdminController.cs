using CFlashcards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CFlashcards.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    // This is a Controller with functions related to the admin role.
    {
        private readonly UserManager<FlashcardsUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(UserManager<FlashcardsUser> userManager, ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> UserTable()
        {
            List<FlashcardsUser> users = await _userManager.Users.ToListAsync();
            if (users == null)
            {
                _logger.LogError("[AdminController] Users not found when trying to retrieve them using _userManager.Users.ToListAsync()");
                return BadRequest("Users not found.");
            }
            return View(users);
        }
    }
}
