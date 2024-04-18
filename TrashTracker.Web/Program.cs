using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TrachTracker.Web.Services;
using TrashTracker.Data.Models;
using TrashTracker.Data.Models.Defaults;
using TrashTracker.Data.Models.Tables;
using TrashTracker.Web;
using TrashTracker.Web.Services;
using TrashTracker.Web.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TrashTrackerDbContext>(options =>
{
    IConfigurationRoot configuration = builder.Configuration;

    // Use MSSQL database: need Microsoft.EntityFrameworkCore.SqlServer package for this
    options.UseSqlServer(
        // get connection string from appsettings.json
        configuration.GetConnectionString("SqlServerConnection"),
        // spatial data is used in database
        options => options.UseNetTopologySuite()
    );

    // Use lazy loading (don't forget the virtual keyword on novigation properties)
    options.UseLazyLoadingProxies();
});

builder.Services.AddIdentity<TrashTrackerUser, TrashTrackerIdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<TrashTrackerDbContext>()
.AddDefaultTokenProviders();

PolicyBuilder.BuildPolicies<Roles>(builder);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.TryAddTransient(_ => new HttpClient());

builder.Services.AddTransient<TrashoutService>();
builder.Services.AddHostedService<TrashoutTimedHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var loginSecrets = builder.Configuration.GetSection("Login").Get<LoginSecrets>();
if (LoginSecrets.IncompleteLoginSecrets())
{
    throw new ArgumentNullException("Login secrets are incomplete, check secrets.json", nameof(loginSecrets));
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<TrashTrackerDbContext>();
    string imageSource = app.Configuration.GetValue<string>("ImageSource");
    DbInitializer.Initialize(context, imageSource);

    // Seed the database with the default roles and users
    await DbInitializer.SeedRolesAsync(serviceScope.ServiceProvider.GetRequiredService<RoleManager<TrashTrackerIdentityRole>>());
    await DbInitializer.SeedUsersAsync(serviceScope.ServiceProvider.GetRequiredService<UserManager<TrashTrackerUser>>());
}

app.Run();
