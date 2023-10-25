using CFlashcards.Models;

namespace CFlashcards.ViewModels
    // ViewModel class for sending additional information into the BrowseFlashcards View.
{
    public class PaginatedFlashcardsViewModel
    {
        public PaginatedList<Flashcard> Flashcards;
        public int DeckId;
        public string FlashcardsUserId;

        public PaginatedFlashcardsViewModel(PaginatedList<Flashcard> flashcards, int deckId, string flashcardsUserId) { 
            Flashcards = flashcards;
            DeckId = deckId;
            FlashcardsUserId = flashcardsUserId;
        }
    }
}
