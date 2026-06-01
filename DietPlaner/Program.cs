using Microsoft.EntityFrameworkCore;
using DietPlaner.Data;
using DietPlaner.Models;
using DietPlaner.Helpers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DietPlanerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DietPlanerContext")
        ?? throw new InvalidOperationException("Connection string 'DietPlanerContext' not found.")));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DietPlaner.Filters.ApiAuthFilter>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed data on first run
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DietPlanerContext>();
    context.Database.Migrate();

    if (!context.Diet.Any())
    {
        context.Diet.AddRange(
            new Diet { Name = "Low Fat",    Kcal = 1500, DietType = DietType.lowFat },
            new Diet { Name = "Vegetarian", Kcal = 1800, DietType = DietType.vegetarian },
            new Diet { Name = "Standard",   Kcal = 2000, DietType = DietType.lowFat }
        );
        context.SaveChanges();
    }

    if (!context.Ward.Any())
    {
        context.Ward.AddRange(
            new Ward { Name = "Internal Medicine", Floor = 1 },
            new Ward { Name = "Cardiology",        Floor = 2 },
            new Ward { Name = "Orthopedics",        Floor = 3 }
        );
        context.SaveChanges();
    }

    var seedUsers = new[]
    {
        new User { Name = "Admin",  Surname = "Administrator", LoginName = "admin",       PasswordHash = PasswordHelper.HashPassword("admin123"), ApiToken = "a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4", UserRole = UserRole.Admin },
        new User { Name = "Anna",   Surname = "Kowalska",      LoginName = "akowalska",   PasswordHash = PasswordHelper.HashPassword("nurse123"), ApiToken = "b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5", UserRole = UserRole.Nurse },
        new User { Name = "Piotr",  Surname = "Nowak",         LoginName = "pnowak",      PasswordHash = PasswordHelper.HashPassword("nurse123"), ApiToken = "c3d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6", UserRole = UserRole.Nurse },
        new User { Name = "Maria",  Surname = "Wiśniewska",    LoginName = "mwisniewska", PasswordHash = PasswordHelper.HashPassword("diet123"),  ApiToken = "d4e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1", UserRole = UserRole.Dietician },
        new User { Name = "Tomasz", Surname = "Zając",         LoginName = "tzajac",      PasswordHash = PasswordHelper.HashPassword("diet123"),  ApiToken = "e5f6a1b2c3d4e5f6a1b2c3d4e5f6a1b2", UserRole = UserRole.Dietician }
    };
    foreach (var u in seedUsers)
    {
        var existing = context.User.FirstOrDefault(x => x.LoginName == u.LoginName);
        if (existing == null)
            context.User.Add(u);
        else if (string.IsNullOrEmpty(existing.ApiToken))
            existing.ApiToken = u.ApiToken;
    }
    context.SaveChanges();

    if (!context.Patient.Any())
    {
        var lowFat    = context.Diet.First(d => d.Name == "Low Fat");
        var vegetarian = context.Diet.First(d => d.Name == "Vegetarian");
        var standard  = context.Diet.First(d => d.Name == "Standard");
        var internal_ = context.Ward.First(w => w.Name == "Internal Medicine");
        var cardio    = context.Ward.First(w => w.Name == "Cardiology");
        var ortho     = context.Ward.First(w => w.Name == "Orthopedics");

        context.Patient.AddRange(
            new Patient { Name = "Jan",      Surname = "Kowalski",   Pesel = "65041512345", DietId = lowFat.Id,     WardId = cardio.Id },
            new Patient { Name = "Zofia",    Surname = "Wójcik",     Pesel = "48092367890", DietId = vegetarian.Id, WardId = internal_.Id },
            new Patient { Name = "Marek",    Surname = "Kaminski",   Pesel = "72031598765", DietId = standard.Id,   WardId = ortho.Id },
            new Patient { Name = "Barbara",  Surname = "Lewandowska",Pesel = "55060234567", DietId = lowFat.Id,     WardId = cardio.Id },
            new Patient { Name = "Krzysztof",Surname = "Zielinski",  Pesel = "80112056789", DietId = vegetarian.Id, WardId = internal_.Id },
            new Patient { Name = "Halina",   Surname = "Szymanska",  Pesel = "42031878901", DietId = standard.Id,   WardId = ortho.Id },
            new Patient { Name = "Robert",   Surname = "Wojcik",     Pesel = "91042312345", DietId = lowFat.Id,     WardId = internal_.Id },
            new Patient { Name = "Irena",    Surname = "Dąbrowska",  Pesel = "53072489012", DietId = vegetarian.Id, WardId = cardio.Id },
            new Patient { Name = "Grzegorz", Surname = "Kozlowski",  Pesel = "78052934567", DietId = null,          WardId = ortho.Id },
            new Patient { Name = "Elzbieta", Surname = "Mazur",      Pesel = "61083145678", DietId = standard.Id,   WardId = null }
        );
        context.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
