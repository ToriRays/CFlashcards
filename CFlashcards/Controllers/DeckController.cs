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
        {
            var flashcardUserId = _userManager.GetUserId(this.User) ?? ""; //Avoids null reference warnings
            IEnumerable<Deck>? decks;

            if (string.IsNullOrEmpty(searchString))
            {
                decks = await _deckRepository.GetAll(flashcardUserId);
                if (decks == null)
                {
                    _logger.LogError("[DeckController] Deck list not found while executing _deckRepository.GetAll()");
                    return NotFound("Deck list not found.");
                }
            }
            else
            {
                decks = await _deckRepository.SearchDecksByTitle(flashcardUserId, searchString);
                if (decks == null)
                {
                    _logger.LogError("[DeckController] Deck list not found while executing _deckRepository.SearchDecksByTitle()");
                    return NotFound("Deck list not found.");
                }
            }

            var pageSize = 6;

            ViewData["SearchTerm"] = searchString;
            return View(PaginatedList<Deck>.Create(decks.ToList(), pageNumber ?? 1, pageSize));
        }


        [HttpGet]
        [Authorize]
        public IActionResult CreateDeck()
        {
            ViewBag.AdditionalData = _userManager.GetUserId(User) ?? "";
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateDeck(Deck deck)
        {
            if (ModelState.IsValid)
            {
                deck.FlashcardUserId = _userManager.GetUserId(User) ?? ""; //Avoids null reference warnings
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
            if (ModelState.IsValid)
            {
                bool returnOk = await _deckRepository.Update(deck);
                if (returnOk)
                {
                    return RedirectToAction(nameof(BrowseDecks));
                }
            }
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
                _logger.LogError("[DeckController] Deck deletion failed for the DeckId {DeckId:0000}", id);
                return BadRequest("Deck deletion failed.");
            }
            return RedirectToAction(nameof(BrowseDecks));
        }
    }
}
