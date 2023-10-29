using CFlashcards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CFlashcards.DAL;
public static class DBInit
{

    public static void Seed(IApplicationBuilder app, RoleManager<IdentityRole> roleManager, UserManager<FlashcardsUser> userManager, IUserStore<FlashcardsUser> userStore)
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
                    Question = "Fishing rod",
                    Answer = "Fiskestang",
                    Notes = "Important to know before going on a fishing trip. :)",
                    DeckId = 1,
                    IsLanguageFlashcard = true
                },
                new Flashcard
                {
                    Question = "Brown cheese",
                    Answer = "Brunost",
                    Notes = "A national Norwegian chewy cheese with a brown colour and sweet taste.",
                    DeckId = 1,
                    IsLanguageFlashcard = true
                },
                new Flashcard
                {
                    Question = "Bench",
                    Answer = "Benk",
                    Notes = "Just a Bench :O",
                    DeckId = 1,
                    IsLanguageFlashcard = true
                },
                new Flashcard
                {
                    Question = "What is Newton's second law of physics?",
                    Answer = "F = ma",
                    Notes = "The acceleration of an object depends on the mass of the object and the amount of force applied.",
                    DeckId = 2,
                    IsLanguageFlashcard = false
                },
                new Flashcard
                {
                    Question = "What is photosynthesis?",
                    Answer = "The process by which green plants and some other organisms use sunlight to synthesize nutrients from carbon dioxide and water.",
                    DeckId = 2,
                    IsLanguageFlashcard = false
                },

            };
            context.AddRange(flashcards);
            context.SaveChanges();
        }

        context.SaveChanges();

        SeedRolesAsync(roleManager).GetAwaiter().GetResult();
        SeedUsersAsync(userManager, userStore).GetAwaiter().GetResult();


    }
    // Create roles
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "admin", "user" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role)) // Check if role exists
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
    // Create an admin user and a regular user.
    public static async Task SeedUsersAsync(UserManager<FlashcardsUser> userManager, IUserStore<FlashcardsUser> userStore)
    {
        var adminEmail = "admin@example.com";
        var userEmail = "testUser@example.com";

        if (await userManager.FindByEmailAsync(adminEmail) == null) // If user does not exist.
        {
            var admin = CreateUser();

            var emailStore = (IUserEmailStore<FlashcardsUser>)userStore;
            // Because of the way the scaffolded Indentity code is, we have to set the UserName to
            // be equal to the email.
            await userStore.SetUserNameAsync(admin, adminEmail, CancellationToken.None);
            await emailStore.SetEmailAsync(admin, adminEmail, CancellationToken.None);

            admin.NickName = "Adminsson";
            admin.LockoutEnabled = false;
            admin.PhoneNumber = "4242424242";
            admin.EmailConfirmed = true;
            admin.PhoneNumberConfirmed = true;


            var adminResult = await userManager.CreateAsync(admin, "123456#e");

            if (adminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }

        if (await userManager.FindByEmailAsync(userEmail) == null) // If user does not exist.
        {
            var user = CreateUser();

            var emailStore = (IUserEmailStore<FlashcardsUser>)userStore;
            // Because of the way the scaffolded Indentity code is, we have to set the UserName to
            // be equal to the email.
            await userStore.SetUserNameAsync(user, userEmail, CancellationToken.None);
            await emailStore.SetEmailAsync(user, userEmail, CancellationToken.None);

            user.NickName = "TestUser";
            user.LockoutEnabled = false;
            user.PhoneNumber = "6969696969";
            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = true;

            var userResult = await userManager.CreateAsync(user, "123456#e");

            if (userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "user");
            }
        }
    }

    // This function is taken from Register.cshtml.cs file.
    private static FlashcardsUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<FlashcardsUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(FlashcardsUser)}'. " +
                $"Ensure that '{nameof(FlashcardsUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }
}


