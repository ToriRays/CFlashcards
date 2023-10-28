using CFlashcards.DAL;
using CFlashcards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Drawing.Printing;

namespace CFlashcards.Controllers
{
    public class DeckController : Controller
    {
        private readonly IDeckRepository _deckRepository;
        private readonly ILogger<DeckController> _logger;
        private readonly UserManager<FlashcardsUser> _userManager;

        public DeckController(IDeckRepository deckRepository, ILogger<DeckController> logger, UserManager<FlashcardsUser> userManager)
        {
            _deckRepository = deckRepository;
            _logger = logger;
            _userManager = userManager;
        }

        
        [Authorize]
        public async Task<IActionResult> BrowseDecks(string searchString, int? pageNumber)
            // The BrowseDecks View that this function returns allows the use to browse through the existing decks, create new decks and
            // search for specific decks using searchString. In addition, pagination functonality is implemented using the PaginatedList<> class.
        {
            var flashcardsUserId = _userManager.GetUserId(this.User) ?? ""; // If the flashcardsUserId cannot be retrieved, set it to "".
            IEnumerable<Deck>? decks; // Initiate the deck list.
            if (string.IsNullOrEmpty(searchString)) // Check whether the user wants to perform a search.
            {
                decks = await _deckRepository.GetAll(flashcardsUserId);
                if (decks == null)
                {
                    _logger.LogError("[DeckController] Deck list not found while executing _deckRepository.GetAll()");
                    return NotFound("Deck list not found.");
                }
            }
            else
            {
                decks = await _deckRepository.SearchDecksByTitle(flashcardsUserId, searchString);
                if (decks == null)
                {
                    _logger.LogError("[DeckController] Deck list not found while executing _deckRepository.SearchDecksByTitle()");
                    return NotFound("Deck list not found.");
                }
            }

            var pageSize = 6;
            // We return the decks through the PaginatedList<> class such that not all decks are displayed in the View at once.
            return View(PaginatedList<Deck>.Create(decks.ToList(), pageNumber ?? 1, pageSize));
        }


        [HttpGet]
        [Authorize]
        public IActionResult CreateDeck()
        {
            // Use view bag to send additional data about the FlashcardsUserId to the CreateDeck View.
            ViewBag.AdditionalData = _userManager.GetUserId(User) ?? "";
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateDeck(Deck deck)
        {
            if (ModelState.IsValid) // Server side validation.
            {
                deck.FlashcardUserId = _userManager.GetUserId(User) ?? ""; // If the flashcardsUserId cannot be retrieved, set it to "".
                bool returnOk = await _deckRepository.Create(deck);
                if (returnOk)
                {
                    return RedirectToAction(nameof(BrowseDecks));
                }
            }
            _logger.LogWarning("[DeckController] Deck creation failed {@deck}", deck);
            return View(deck);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdateDeck(int id)
        {
            var deck = await _deckRepository.GetDeckById(id);
            if (deck == null)
            {
                _logger.LogError("[DeckController] Deck not found when updating the DeckId {DeckId:0000}", id);
                return BadRequest("Deck not found for the DeckId.");
            }
            return View(deck);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateDeck(Deck deck)
        {
            if (ModelState.IsValid) // Server side validation.
            {
                bool returnOk = await _deckRepository.Update(deck);
                if (returnOk)
                {
                    return RedirectToAction(nameof(BrowseDecks));
                }
            }
            // If the deck update fails, log a warning and stay on the UpdateDeck View.
            _logger.LogWarning("[DeckController] Deck update failed {@deck}", deck);
            return View(deck);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteDeck(int id)
        {
            var deck = await _deckRepository.GetDeckById(id);
            if (deck == null)
            {
                _logger.LogError("[DeckController] Deck not found when updating the DeckId {DeckId:0000}", id);
                return BadRequest("Deck not found for the DeckId.");
            }
            return View(deck);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteDeckConfirmed(int id)
        {
            bool returnOk = await _deckRepository.Delete(id);
            if (!returnOk)
            {
                _logger.LogError("[DeckController] Deck deletion failed for the DeckId {@id}", id);
                return BadRequest("Deck deletion failed.");
            }
            return RedirectToAction(nameof(BrowseDecks));
        }
    }
}