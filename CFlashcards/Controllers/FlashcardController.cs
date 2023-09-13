using CFlashcards.Models;
using CFlashcards.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CFlashcards.Controllers
{
    public class FlashcardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}
