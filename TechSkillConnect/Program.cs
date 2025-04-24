using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechSkillConnect.Data;
using TechSkillConnect.Models;


var builder = WebApplication.CreateBuilder(args);
// ✅ Configure Cookie Policy to allow TempData persistence
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax; // Allows cookies across requests
});

// ✅ Enable session management
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AspNetCore.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;  // ✅ Allows cross-site cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;  // ✅ Allows HTTP (Only for development)
});


// ✅ Ensure TempData uses session storage instead of cookies
builder.Services.AddRazorPages().AddSessionStateTempDataProvider();



// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
   .AddRoles<IdentityRole>()
   .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin");

    options.Conventions.AuthorizePage("/Admin/Tutors");
    options.Conventions.AuthorizePage("/Admin/TutorProfiles");
    options.Conventions.AuthorizePage("/Admin/Learners");
    options.Conventions.AuthorizePage("/Admin/Connections");
    options.Conventions.AuthorizePage("/Admin/Transactions");

    //    options.Conventions.AllowAnonymousToPage("/Index"); // excluded page

    //  options.Conventions.AllowAnonymousToFolder("/Students"); // excluded folder
});

builder.Services.AddRazorPages();



var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Exception handling if seeding might throw an error
    try
    {
        SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseCookiePolicy();
app.UseSession();

app.UseRouting();

app.UseAuthorization();

// Map the root URL to the TechSkillConnect_homepage.html
//app.MapGet("/", async context =>
//{
//    context.Response.Redirect("/html/TechSkillConnect_homepage.html"); // Redirect to your HTML file
//);


app.MapRazorPages();

app.Run();


