﻿using System.ComponentModel.DataAnnotations;

namespace CFlashcards.Models
{
    public class Deck
    {
        public int DeckId { get; set; }
        [RegularExpression(@"[0-9a-zA-ZæøåÆØÅ. \-]{2,20}", ErrorMessage = "The Title must be numbers or letters and between 2 to 20 characters.")]
        //[Display(Title = "Deck title")]
        public string Title { get; set; } = string.Empty;
        [StringLength(300)]
        public string? Description { get; set; }
        //navigation property
        public virtual List<Flashcard>? Flashcards { get; set; } //Virtual keyword used for lazy loading
        //navigation property
        public string FlashcardUserId { get; set; }
    }
}
