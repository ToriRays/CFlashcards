using CFlashcards.Models;

namespace CFlashcards.ViewModels
{
    public class FlashcardViewModel
    {
        public Flashcard Flashcard { get; set; }
        public int DeckId;

        public FlashcardViewModel(Flashcard flashcard, int deckId) 
        {
            Flashcard = flashcard;
            DeckId = deckId;
        }
    }
}
