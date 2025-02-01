using Microsoft.EntityFrameworkCore;
using teleScope.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Προσθήκη Logging στο DI container
builder.Services.AddLogging();

// Καθορίζουμε ότι τα logs θα εμφανίζονται στην κονσόλα
builder.Logging.ClearProviders(); // Καθαρίζει τους υπάρχοντες providers
builder.Logging.AddConsole(); // Προσθήκη console provider

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
