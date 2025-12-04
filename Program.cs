using Microsoft.EntityFrameworkCore;
using MovieSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// هنا تحط كود الـ Seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Add Actors if not exist
    if (!context.Actors.Any(a => a.Name == "Matthew McConaughey"))
    {
        // Actors
        var matthew = new Actor { Name = "Matthew McConaughey", Age = 52, Bio = "Oscar-winning actor known for Interstellar.", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/1/10/Matthew_McConaughey_Cannes_2014.jpg" };
        var anne = new Actor { Name = "Anne Hathaway", Age = 41, Bio = "Academy Award-winning actress.", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/8/88/Anne_Hathaway_2015.jpg" };
        var christian = new Actor { Name = "Christian Bale", Age = 50, Bio = "Known for The Dark Knight Trilogy.", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/f/f1/Christian_Bale_2014.jpg" };
        var leo = new Actor { Name = "Leonardo DiCaprio", Age = 48, Bio = "Oscar-winning actor.", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/f/f4/Leonardo_DiCaprio_66ème_Festival_de_Venise_%28Mostra%29.jpg" };
        var tom = new Actor { Name = "Tom Hardy", Age = 46, Bio = "Known for Inception and Mad Max.", ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/70/Tom_Hardy_Cannes_2015.jpg" };

        context.Actors.AddRange(matthew, anne, christian, leo, tom);

        // Movies
        var shawshank = new Movie { Title = "The Shawshank Redemption", Description = "Two imprisoned men bond over a number of years...", Genre = "Drama", ReleaseYear = 1994, ImageUrl = "https://upload.wikimedia.org/wikipedia/en/8/81/ShawshankRedemptionMoviePoster.jpg" };
        var godfather = new Movie { Title = "The Godfather", Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.", Genre = "Crime", ReleaseYear = 1972, ImageUrl = "https://upload.wikimedia.org/wikipedia/en/1/1c/Godfather_ver1.jpg" };
        var interstellar = new Movie { Title = "Interstellar", Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.", Genre = "Sci-Fi", ReleaseYear = 2014, ImageUrl = "https://upload.wikimedia.org/wikipedia/en/b/bc/Interstellar_film_poster.jpg" };
        var inception = new Movie { Title = "Inception", Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea.", Genre = "Sci-Fi", ReleaseYear = 2010, ImageUrl = "https://upload.wikimedia.org/wikipedia/en/7/7f/Inception_ver3.jpg" };

        context.Movies.AddRange(shawshank, godfather, interstellar, inception);

        // Link Actors to Movies
        interstellar.Actors.Add(matthew);
        interstellar.Actors.Add(anne);

        inception.Actors.Add(leo);
        inception.Actors.Add(tom);
        inception.Actors.Add(anne);

        christian.Movies.Add(inception);

        // Save all changes
        context.SaveChanges();
    }
}


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
    pattern: "{controller=Movie}/{action=Index}/{id?}");

app.Run();
