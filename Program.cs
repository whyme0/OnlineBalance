using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using OnlineBalance.Data;
using OnlineBalance.Models;
using OnlineBalance.Mapping;
using OnlineBalance.Data.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using EmailService;

string rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        Path.Combine(rootPath, "Logging\\log-.txt"),
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// urls lowercase option
builder.Services.AddRouting(options => options.LowercaseUrls = true);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<ApplicationDbContext>(b => 
    b.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
        o => 
            {
                o.LoginPath = "/auth/signin";
            });
builder.Services.AddIdentity<User, IdentityRole>(
    o =>
    {
        o.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>();

builder.Services.AddScoped<UserMapping>();

var emailConfiguration = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfiguration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/notfound";
        await next();
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
