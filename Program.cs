using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Data;
using MvcMovie.Models; // Make sure to include this for the Movie class

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the database context
builder.Services.AddDbContext<MvcMovieContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MvcMovieContext")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- Seed the database with favorite movies ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MvcMovieContext>();

    // Ensure database is created
    context.Database.EnsureCreated();

    // Add favorite movies if the database is empty
    if (!context.Movie.Any())
    {
        context.Movie.AddRange(
            new Movie { Title = "The Matrix", ReleaseDate = DateTime.Parse("1999-03-31"), Genre = "Action", Price = 9.99M },
            new Movie { Title = "Inception", ReleaseDate = DateTime.Parse("2010-07-16"), Genre = "Sci-Fi", Price = 12.99M },
            new Movie { Title = "Interstellar", ReleaseDate = DateTime.Parse("2014-11-07"), Genre = "Sci-Fi", Price = 14.99M }
        );
        context.SaveChanges();
    }
}

app.Run();
