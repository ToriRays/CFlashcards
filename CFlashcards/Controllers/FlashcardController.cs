using CFlashcards.DAL;
using CFlashcards.Models;
using CFlashcards.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CFlashcards.Controllers
{
    public class FlashcardController : Controller
    {
        private readonly IFlashcardRepository _flashcardRepository;
        private readonly ILogger<FlashcardController> _logger;

        public FlashcardController(IFlashcardRepository flashcardRepository, ILogger<FlashcardController> logger)
        {
            _flashcardRepository = flashcardRepository;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var flashcard = await _flashcardRepository.GetFlashcardById(id);
            if (flashcard == null)
            {
                _logger.LogError("[FlashcardController] Flashcard not found when trying to edit the FlashcardId {FlashcardId:0000}", id);
                return BadRequest("Flashcard not found for the FlashcardId.");
            }
            return View(flashcard);
        }

        [HttpGet]
        [Authorize]
        public IActionResult CreateFlashcard(int deckId)
        {
            ViewBag.AdditionalData = deckId;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateFlashcard(Flashcard flashcard)
        {
            if (ModelState.IsValid)
            {
                bool returnOk = await _flashcardRepository.Create(flashcard);
                if (returnOk)
                {
                    return RedirectToAction("Details", new { id = flashcard.FlashcardId }); //redirect to the detailed view of the newly created card
                }
            }
            _logger.LogWarning("[FlashcardController] Flashcard creation failed {@flashcard}", flashcard);
            return RedirectToAction("Carousel", "DeckController", new {id = flashcard.DeckId}); //redirect to the carousel view of the deck where card creation was started
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditFlashcard(int id)
        {
            var flashcard = await _flashcardRepository.GetFlashcardById(id);
            if (flashcard == null)
            {
                _logger.LogError("[FlashcardController] Flashcard not found when trying to edit the FlashcardId {FlashcardId:0000}", id);
                return BadRequest("Flashcard not found for the FlashcardId.");
            }
            return View(flashcard);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditFlashcard(Flashcard flashcard)
        {
            if (ModelState.IsValid)
            {
                bool returnOk = await _flashcardRepository.Update(flashcard);
                if (returnOk)
                {
                    return RedirectToAction("Details" , new {id = flashcard.FlashcardId}); //redirect to the detailed view of the edited card
                }
            }
            _logger.LogWarning("[FlashcardController] Flashcard update failed {@flashcard}", flashcard);
            return View(flashcard);
        }
    }
}
