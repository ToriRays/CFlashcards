using CFlashcards.Models;

namespace CFlashcards.ViewModels
{
    //This is used in the unit tests
    public class DeckListViewModel
    {
        public IEnumerable<Deck> Decks;

        public DeckListViewModel(IEnumerable<Deck> decks)
        {
            Decks = decks;
        }
    }
}
