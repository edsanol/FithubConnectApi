using Api.Hubs;
using Application.Extensions;
using Domain.Entities.Configuration;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
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

var firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS_PATH")
    ?? builder.Configuration["Firebase:CredentialsPath"];
if (string.IsNullOrEmpty(firebaseCredentialsPath))
{
    throw new Exception("La ruta del archivo de credenciales de Firebase no está configurada.");
}

// Configurar JwtConfiguration directamente con el valor de jwtSecret
builder.Services.Configure<JwtConfiguration>(options =>
{
    options.Secret = jwtSecret;
});

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebaseCredentialsPath)
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

// 7) Otros servicios: Swagger, etc.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 8) CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        // Aquí indicas el front que permites
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Esto es clave si SignalR va con credenciales
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("CorsPolicy");

//if (app.Environment.IsDevelopment())
//{

//}

// 9) Middleware de Swagger
app.UseSwagger();
app.UseSwaggerUI();

// 10) Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// 11) Mapeo de controladores
app.MapControllers();

// 13) Mapeo de tu Hub (SignalR)
app.MapHub<NotificationHub>("/hubs/notification");
app.Run();
