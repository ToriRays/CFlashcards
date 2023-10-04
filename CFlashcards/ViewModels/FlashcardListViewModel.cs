using CFlashcards.Models;

namespace CFlashcards.ViewModels
{
    //This is used in the unit tests
    public class FlashcardListViewModel
    {
        public IEnumerable<Flashcard> Flashcards;

        public FlashcardListViewModel(IEnumerable<Flashcard> flashcards) 
        {
            Flashcards = flashcards;
        }
    }
}
