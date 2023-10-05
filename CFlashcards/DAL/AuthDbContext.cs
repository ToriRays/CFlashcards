using CFlashcards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CFlashcards.DAL;

public class AuthDbContext : IdentityDbContext<FlashcardsUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }
    // Add Flashcard entity
    public DbSet<Flashcard> Flashcards { get; set; }
    // Add Deck entity
    public DbSet<Deck> Decks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        string ADMIN_ID = "02174cf0–9412–4cfe-afbf-59f706d72cf6";
        string ROLE_ID = "341743f0-asd2–42de-afbf-59kmkkmk72cf6";

        //seed admin role
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Name = "SuperAdmin",
            NormalizedName = "SUPERADMIN",
            Id = ROLE_ID,
            ConcurrencyStamp = ROLE_ID
        });

        //create user
        var appUser = new FlashcardsUser
        {
            Id = ADMIN_ID,
            Email = "frankofoedu@gmail.com",
            EmailConfirmed = true,
            FirstName = "Frank",
            LastName = "foedu",
            UserName = "frankofoedu@gmail.com",
            NormalizedUserName = "FRANKOFOEDU@GMAIL.COM"
        };

        //set user password
        PasswordHasher<FlashcardsUser> ph = new PasswordHasher<FlashcardsUser>();
        appUser.PasswordHash = ph.HashPassword(appUser, "mypassword_ ?");

        //seed user
        builder.Entity<FlashcardsUser>().HasData(appUser);

        //set user role to admin
        builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
        {
            RoleId = ROLE_ID,
            UserId = ADMIN_ID
        });
    }
}
