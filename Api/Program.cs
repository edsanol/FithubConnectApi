using Api.Hubs;
using Application.Extensions;
using Domain.Entities.Configuration;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

// 1) Configuración de la cadena de conexión desde variables de entorno
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
    ?? Configuration.GetConnectionString("FitHubConnection");

if (connectionString is null)
{
    throw new ArgumentNullException(nameof(connectionString));
}

// 2) Configuración JWT
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

// 3) Autenticación JWT
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

// 4) Inyección de dependencias personalizadas (infra y app)
builder.Services.AddInjectionInfrastructure(Configuration, connectionString);
builder.Services.AddInjectionApplication(Configuration);

builder.Services.AddHttpContextAccessor();

// 5) Controladores
builder.Services.AddControllers();

// 6) Añadir SignalR 
builder.Services.AddSignalR();

// 7) Otros servicios: Swagger, etc.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 8) CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{

//}

// 9) Middleware de Swagger
app.UseSwagger();
app.UseSwaggerUI();

// 10) CORS
app.UseCors();

// 11) Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// 12) Mapeo de controladores
app.MapControllers();

// 13) Mapeo de tu Hub (SignalR)
app.MapHub<NotificationHub>("/hubs/notification");
app.Run();
