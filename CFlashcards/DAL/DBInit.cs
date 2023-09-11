using CFlashcards.Models;

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
                    Description = "A deck containing basic Norwegian language cards for learning."
                }
            };
        }
    }
}
