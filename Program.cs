using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;
using Microsoft.EntityFrameworkCore;
using OnlineBalance.Data;
using OnlineBalance.Models;
using OnlineBalance.Mapping;
using OnlineBalance.Data.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using EmailService;

string rootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\"));
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings-Email.json", optional: true, reloadOnChange: true);
var configuration = builder.Configuration;

// Logging
Log.Logger = new LoggerConfiguration()
    //.WriteTo.File(
    //    Path.Combine(rootPath, "Logging\\log-.txt"),
    //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
    //    rollingInterval: RollingInterval.Day,
    //    restrictedToMinimumLevel: LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// urls lowercase option
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Postgress Options
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
builder.Services.AddDbContext<ApplicationDbContext>(b => 
    b.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
);

// AUTH, IDENTITY
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
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromMinutes(5));

// Mapping
builder.Services.AddScoped<UserMapping>();

// Inject Emailing service
var emailConfiguration = configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfiguration);
builder.Services.AddScoped<IEmailSender, EmailSender>();

//######################################
//######################################
//#############   APP   ################
//######################################
//######################################

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
