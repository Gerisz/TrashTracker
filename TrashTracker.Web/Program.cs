using Microsoft.EntityFrameworkCore;
using TrashTracker.Web.Models;

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

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
