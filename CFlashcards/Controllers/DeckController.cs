using CFlashcards.DAL;
using CFlashcards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Browse()
        {
            var flashcardUserId = _userManager.GetUserId(this.User);
            var decks = await _deckRepository.GetAll(flashcardUserId);
            if (decks == null)
            {
                _logger.LogError("[DeckController] Deck list not found while executing _itemRepository.GetAll()");
                return NotFound("Deck list not found.");
            }
            return View(decks);
        }
    }
}
