using CFlashcards.DAL;
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
        public IActionResult Decks()
        {
            return View();
        }
    }
}
