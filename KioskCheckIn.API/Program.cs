using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KioskCheckIn;
using KioskCheckIn.Data.Models;
using KioskCheckInAPI;
using KioskCheckIn.API.Services;
using KioskCheckIn.API.Repository;
using Microsoft.AspNetCore.Components;
using NLog.Web;
using KioskCheckIn.API.Helpers;
using KioskCheckIn.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


// AUTHENTICATION //

var signingKey = Environment.GetEnvironmentVariable("JWT_SECRET");

if (string.IsNullOrWhiteSpace(signingKey))
{
    throw new InvalidOperationException("JWT_SECRET environment variable not set.");
}

var keyBytes = Encoding.UTF8.GetBytes(signingKey);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        // Skip issuer/audience validation for demo purposes
//        ValidateIssuer = false,
//        ValidateAudience = false,

//        // Validate the token's lifetime (expiration)
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = "KioskCheckInAPI",

//        // Use a simple symmetric key for demo purposes
//        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
//    };
//});

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "KioskAuth";
        options.LoginPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromHours(9);
        options.AccessDeniedPath = "/denied";
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<KioskContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<KioskContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(MapperProfile).Assembly);
});

builder.Services.AddScoped<VisitorState>();
builder.Services.AddScoped<UserAuthenticationService>();
builder.Services.AddScoped<VisitorService>();
builder.Services.AddScoped(typeof(ICookie), typeof(Cookie));
builder.Services.AddScoped(typeof(IUserAuthenticationService), typeof(UserAuthenticationService));
builder.Services.AddScoped(typeof(IVisitorRepository), typeof(VisitorRepository));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
builder.Services.AddScoped(typeof(IUserSessionRepository), typeof(UserSessionRepository));

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Error);
builder.Host.UseNLog(); // Load nlog.config

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

