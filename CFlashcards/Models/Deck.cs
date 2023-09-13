namespace CFlashcards.Models
{
    public class Deck
    {
        public int DeckId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        //navigation property
        public virtual List<Flashcard>? Flashcards { get; set; } //Virtual keyword used for lazy loading
        //navigation property
        public string FlashcardUserId { get; set; }
    }
}
