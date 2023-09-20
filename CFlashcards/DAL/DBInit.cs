using CFlashcards.Models;
using Microsoft.EntityFrameworkCore;

namespace CFlashcards.DAL;

public static class DBInit
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        AuthDbContext context = serviceScope.ServiceProvider.GetRequiredService<AuthDbContext>();
        context.Database.EnsureCreated();

        if (!context.Decks.Any())
        {
            var decks = new List<Deck>
            {
                new Deck
                {
                    Title = "Norwegian flashcards",
                    Description = "This is a demo deck: A deck containing basic Norwegian language cards for learning.",
                    FlashcardUserId = "demo"
                },
                new Deck
                {
                    Title = "Science quiz flashcard",
                    Description = "This is a demo deck: A deck containing science questions and answers ",
                    FlashcardUserId = "demo"
                }
            };
            context.AddRange(decks);
            context.SaveChanges();
        }
        if (!context.Flashcards.Any())
        {
            var flashcards = new List<Flashcard>
            {
                new Flashcard
                {
                    Question = "Fiskestang",
                    Answer = "Fishing rod",
                    Notes = "Important to know before going on a fishing trip. :)",
                    DeckId = 1
                },
                new Flashcard
                {
                    Question = "Brunost",
                    Answer = "Brown cheese",
                    Notes = "A national Norwegian chewy cheese with a brown colour and sweet taste.",
                    DeckId = 1
                },
                new Flashcard
                {
                    Question = "Benk",
                    Answer = "Bench",
                    Notes = "Just a Bench :O",
                    DeckId = 1
                },
                new Flashcard
                {
                    Question = "What is Newton's second law of physics?",
                    Answer = "F = ma",
                    Notes = "The acceleration of an object depends on the mass of the object and the amount of force applied.",
                    DeckId = 2
                },
                new Flashcard
                {
                    Question = "What is photosynthesis?",
                    Answer = "The process by which green plants and some other organisms use sunlight to synthesize nutrients from carbon dioxide and water.",
                    DeckId = 2
                },

            };
            context.AddRange(flashcards);
            context.SaveChanges();
        }
        //Maybe add card number relative to deck in Flashcards.
        context.SaveChanges();
    }
}
