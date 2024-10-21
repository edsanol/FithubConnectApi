using Application.Extensions;
using Domain.Entities.Configuration;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// Configuraci�n de la cadena de conexi�n desde variables de entorno
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? Configuration.GetConnectionString("FitHubConnection");

// Configuración JWT
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? Configuration["Jwt:Secret"];

if (string.IsNullOrEmpty(jwtSecret))
{
    throw new Exception("JWT Secret is missing or empty.");
}

// Configurar JwtConfiguration directamente con el valor de jwtSecret
builder.Services.Configure<JwtConfiguration>(options =>
{
    options.Secret = jwtSecret;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

if (connectionString is null)
{
    throw new ArgumentNullException(nameof(connectionString));
}

builder.Services.AddInjectionInfrastructure(Configuration, connectionString);
builder.Services.AddInjectionApplication(Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.UseCors();
app.Run();
