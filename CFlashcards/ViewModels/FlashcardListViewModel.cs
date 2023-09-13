using CFlashcards.Models;

namespace CFlashcards.ViewModels
{
    public class FlashcardListViewModel
    {
        public IEnumerable<Flashcard>? Flashcards { get; set; }
        public int DeckId;

        public FlashcardListViewModel(IEnumerable<Flashcard>? flashcards, int deckId) 
        {
            Flashcards = flashcards;
            DeckId = deckId;
        }
    }
}
