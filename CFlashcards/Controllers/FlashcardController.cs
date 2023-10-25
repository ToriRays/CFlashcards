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
        // The BrowseFlashcards View that this function returns allows the use to browse through the existing flashcards inside of a deck,
        // create new flashcards, and implements the pagination functonality using the PaginatedList<> class.
        {
            var flashcards = await _flashcardRepository.GetFlashcardsByDeckId(deckId);
            if (flashcards == null)
            {
                _logger.LogError("[FlashcardController] Flashcards not found while executing _flashcardRepository.GetFlashcardsByDeckId() DeckId:{@id}", deckId);
                var badRequest = "Flashcards not found for the DeckId: " + deckId;
                return BadRequest(badRequest);
            }

            
            // Retrieve flashcardsUserId in one of two ways. The reason for this is such that the flashcardsUserId of the demo decks is retrieved correctly.
#pragma warning disable CS8602 // Dereference of a possibly null reference. We disable this warning because it appears even though we check that the Deck object is not null.
            // The reason for it appearing is that Deck is defined as nullable in the Flashcard Model.
            var flashcardsUserId = (flashcards.Any() && flashcards.First().Deck != null) ? flashcards.First().Deck.FlashcardUserId : _userManager.GetUserId(this.User) ?? "";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var pageSize = 4;
            // The flashcards are wrapped using the PaginatedList<> class such that not all flashcards are displayed in the View at once.
            var paginatedFlashcards = PaginatedList<Flashcard>.Create(flashcards.ToList(), pageNumber ?? 1, pageSize);
            if (paginatedFlashcards == null)
            {
                _logger.LogError("[FlashcardController] Paginated Flashcard list could not be created while executing PaginatedList<Flashcard>.Create().");
                var badRequest = "PaginatedList creation failed.";
                return BadRequest(badRequest);
            }
            // Use the PaginatedFlashcardsViewModel to send the paginated flashcards and additional information to the BrowseFlashcards View.
            var paginatedFlashcardsViewModel = new PaginatedFlashcardsViewModel(paginatedFlashcards, deckId, flashcardsUserId);

            return View(paginatedFlashcardsViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
            // The Details View that his function returns provides a more detailed view of a single chosen flashcard in a deck.
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
            // Use view bag to send additional data about the DeclId to the CreateFlashcard View.
            ViewBag.AdditionalData = deckId;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateFlashcard(Flashcard flashcard)
        {
            if (ModelState.IsValid) // Server side validation.
            {
                var deck = await _deckRepository.GetDeckById(flashcard.DeckId); // Retrieve the deck that the flashcards belong to such that it can be assigned to the Deck attribute.
                if (deck == null)
                {
                    _logger.LogError("[FlashcardController] Deck not found when creating a new flashcard FlashcardId {@flashcardId}", flashcard.FlashcardId);
                    return BadRequest("Deck not found when creating the flashcard.");
                }
                flashcard.Deck = deck;
                bool returnOk = await _flashcardRepository.Create(flashcard);
                if (returnOk)
                {
                    return RedirectToAction("Details", new { id = flashcard.FlashcardId }); // Redirects to the Details View of the newly created card.
                }
            }
            // If the flashcard creation fails, log a warning and get redirected to the BrowseFlashcards View.
            _logger.LogWarning("[FlashcardController] Flashcard creation failed {@flashcard}", flashcard);
            return RedirectToAction("BrowseFlashcards", flashcard.DeckId); // Redirects to the BrowseFlashcards view of the deck where card creation was started.
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
            if (ModelState.IsValid) // Server side validation.
            {
                bool returnOk = await _flashcardRepository.Update(flashcard);
                if (returnOk)
                {
                    return RedirectToAction("Details", new { id = flashcard.FlashcardId }); // Redirect to the Details View of the edited card.
                }
            }
            // If the flashcard update fails, log a warning and stay on the UpdateFlashcard View.
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
                _logger.LogError("[FlashcardController] Flashcard not found when updating the FlashcardId {@id}", id);
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
            return RedirectToAction("BrowseFlashcards", new { deckId, pageNumber = 1 });
        }
    }
}
