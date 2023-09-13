using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Identity.UI.Services;
using CFlashcards.Areas.Identity.Services.Email;
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

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AdventureWorks.Session"; // set any name u prefer
    options.IdleTimeout = TimeSpan.FromSeconds(1800); //Session expires after 1800 seconds of being inactive
    options.Cookie.IsEssential = true; //Indicates that the session cookie is essential for the application.
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
builder.Services.AddDefaultIdentity<FlashcardsUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AuthDbContext>(); // I turned off confirmation/verification of the account
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddTransient<IEmailSender, EmailSender>();

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
    DBInit.Seed(app);
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();
//app.UseAuthentication();
//app.MapDefaultControllerRoute();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
