using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Identity.UI.Services;
//using CFlashcards.Areas.Identity.Services.Email;
using System.Configuration;
using CFlashcards.DAL;
using CFlashcards.Models;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IDeckRepository, DeckRepository>();
builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AdventureWorks.Session"; // set any name u prefer
    options.IdleTimeout = TimeSpan.FromSeconds(1800); //Session expires after 1800 seconds of being inactive
    options.Cookie.IsEssential = false; //Indicates that the session cookie is essential for the application.
});

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information() // levels: Trace < Information < Warning < Error < Fatal
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

loggerConfiguration.Filter.ByExcluding(e => e.Properties.TryGetValue("SourceContext", out var value) && //Filters out unnecessary string from the log file
                           e.Level == LogEventLevel.Information &&
                           e.MessageTemplate.Text.Contains("Executed DbCommand"));

var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

//////////////////////////////////////////////////////  Identity /////////////////////////////////////////////////////////////////////////////////
builder.Services.AddDefaultIdentity<FlashcardsUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Add roles to Identity
    .AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    using (var scope = app.Services.CreateScope()) // This lets us access the services that are configured above.
    {
        try
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<FlashcardsUser>>();
            var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<FlashcardsUser>>();
            DBInit.Seed(app, roleManager, userManager, userStore);
        }
        catch (Exception)
        {
            throw;
        }
    }
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
//app.MapDefaultControllerRoute();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
