namespace CFlashcards.Models
{
    public class Flashcard
    {
        public int FlashcardId { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public int DeckId { get; set; }
        // Navigation property. Notice that the attribute is nullable. The reason for this is such that
        // the method ModelState.IsValid does not always return false upon Flashcard creation.
        public virtual Deck? Deck { get; set; } = default!; //Virtual keyword used for lazy loading.
        public bool IsLanguageFlashcard { get; set; } = false;
    }
}

