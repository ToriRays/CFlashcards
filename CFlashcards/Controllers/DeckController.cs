using CFlashcards.DAL;
using CFlashcards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CFlashcards.Controllers
{
    public class DeckController : Controller
    {
        private readonly IDeckRepository _deckRepository;
        private readonly ILogger<DeckController> _logger;

        public DeckController(IDeckRepository deckRepository, ILogger<DeckController> logger)
        {
            _deckRepository = deckRepository;
            _logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Browse()
        {
            var decks = await _deckRepository.GetAll();
            if (decks == null)
            {
                _logger.LogError("[DeckController] Deck list not found while executing _itemRepository.GetAll()");
                return NotFound("Deck list not found.");
            }
            return View(decks);
        }
    }
}
