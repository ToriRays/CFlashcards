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
        private readonly IDeckRepository _deckRepository;
        private readonly UserManager<FlashcardsUser> _userManager;
        private readonly ILogger<FlashcardController> _logger;

        public FlashcardController(IFlashcardRepository flashcardRepository, IDeckRepository deckRepository, UserManager<FlashcardsUser> userManager, ILogger<FlashcardController> logger)
        {
            _flashcardRepository = flashcardRepository;
            _deckRepository = deckRepository;
            _logger = logger;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> BrowseFlashcards(int deckId, int? pageNumber)
        {
            var flashcardUserId = _userManager.GetUserId(this.User) ?? "";

            var flashcards = await _flashcardRepository.GetFlashcardsByDeckId(deckId);
            if (flashcards == null)
            {
                _logger.LogError("[FlashcardController] Flashcards not found while executing _flashcardRepository.GetFlashcardsByDeckId() DeckId:{@id}", deckId);
                var badRequest = "Flashcards not found for the DeckId: " + deckId;
                return BadRequest(badRequest);
            }

            var pageSize = 4;

            var paginatedFlashcards = PaginatedList<Flashcard>.Create(flashcards.ToList(), pageNumber ?? 1, pageSize);
            if (paginatedFlashcards == null)
            {
                _logger.LogError("[FlashcardController] Paginated Flashcard list could not be created while executing PaginatedList<Flashcard>.Create().");
                var badRequest = "PaginatedList creation failed.";
                return BadRequest(badRequest);
            }

            var paginatedFlashcardsViewModel = new PaginatedFlashcardsViewModel(paginatedFlashcards, deckId, flashcardUserId);

            return View(paginatedFlashcardsViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var flashcard = await _flashcardRepository.GetFlashcardById(id);
            if (flashcard == null)
            {
                _logger.LogError("[FlashcardController] Flashcard not found when trying to edit the FlashcardId: {@id}", id);
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
            //Had to remove if(ModelState.IsValid) for this to work. Need to find out why
            var deck = await _deckRepository.GetDeckById(flashcard.DeckId);
            if (deck == null)
            {
                _logger.LogError("[FlashcardController] Deck not found when creating a new flashcard FlashcardId {@flashcardId}", flashcard.FlashcardId);
                return BadRequest("Deck not found when creating the flashcard.");
            }
            flashcard.Deck = deck;
            bool returnOk = await _flashcardRepository.Create(flashcard);
            if (returnOk)
            {
                return RedirectToAction("Details", new { id = flashcard.FlashcardId }); //redirect to the detailed view of the newly created card
            }
            _logger.LogWarning("[FlashcardController] Flashcard creation failed {@flashcard}", flashcard);
            return RedirectToAction("BrowseFlashcards", flashcard.DeckId); //redirect to the carousel view of the deck where card creation was started
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdateFlashcard(int id)
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
        public async Task<IActionResult> UpdateFlashcard(Flashcard flashcard)
        {
            //Had to remove if(ModelState.IsValid) for this to work. Need to find out why
            bool returnOk = await _flashcardRepository.Update(flashcard);
            if (returnOk)
            {
                return RedirectToAction("Details", new { id = flashcard.FlashcardId }); //redirect to the detailed view of the edited card
            }
            _logger.LogWarning("[FlashcardController] Flashcard update failed {@flashcard}", flashcard);
            return View(flashcard);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteFlashcard(int id)
        {
            var flashcard = await _flashcardRepository.GetFlashcardById(id);
            if (flashcard == null)
            {
                _logger.LogError("[FlashcardController] Flashcard not found when updating the FlashcardId {FlashcardId:0000}", id);
                return BadRequest("Flashcard not found for the FlashcardId.");
            }
            return View(flashcard);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteFlashcardConfirmed(int id, int deckId)
        {
            bool returnOk = await _flashcardRepository.Delete(id);
            if (!returnOk)
            {
                _logger.LogError("[FlashcardController] Flashcard deletion failed for the FlashcardId: {@id}", id);
                return BadRequest("Flashcard deletion failed.");
            }
            _logger.LogError("FlashcardId: {@id}, DeckId: {@deckId}", id, deckId);
            return RedirectToAction("BrowseFlashcards", new { id = deckId });
        }
    }
}
