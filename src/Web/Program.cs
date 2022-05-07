using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("SandboxConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

/* Adds the Database developer page exception filter (requires 'Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore' NuGet package)
 * Provides ASP.NET Core middleware for EF Core error pages. This middleware helps to detect and diagnose errors with EF Core migrations.
 * The Database developer page exception filter AddDatabaseDeveloperPageExceptionFilter captures database-related exceptions that can be resolved by using Entity Framework Core migrations. When these exceptions occur, an HTML response is generated with details of possible actions to resolve the issue. This page is enabled only in the Development environment.
 */
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDbContext>();

// Google OAuth 2.0 authentication
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
#region security-related middleware components in order, see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0
if (!app.Environment.IsDevelopment())
{
    // Exception handling middleware in non-Development environments.
    app.UseExceptionHandler("/Home/Error");

    // HTTP Strict Transport Security Protocol (HSTS).
    // UseHsts() is an extension method that enforces HSTS when the app is not in development mode.
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Register Authentication middleware
// Authentication Middleware attempts to authenticate the user before they're allowed access to secure resources.
app.UseAuthentication();

// Authorization Middleware authorizes a user to access secure resources.
app.UseAuthorization();
#endregion

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
