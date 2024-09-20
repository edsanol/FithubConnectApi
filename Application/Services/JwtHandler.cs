using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class JwtHandler : IJwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor httpContextAccessor;

        public JwtHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateToken(Gym gym)
        {
            var claims = new Claim[]
            {
                new Claim("field1", gym.GymId.ToString()),
                new Claim("field2", "gimnasio"),
                new Claim("field3", "auth"),
                new Claim("field4", Guid.NewGuid().ToString()),
            };

            string stringToken = await GenerateTokenFunc(claims, 60);
            return stringToken;
        }

        public async Task<string> GenerateRefreshToken(Gym gym)
        {
            var claims = new Claim[]
            {
                new Claim("field1", gym.GymId.ToString()),
                new Claim("field2", "gimnasio"),
                new Claim("field3", "refresh"),
            };

            string stringToken = await GenerateTokenFunc(claims, 21600);
            return stringToken;
        }

        public async Task<string> GenerateAthleteToken(Athlete athlete)
        {
            var claims = new Claim[]
            {
                new Claim("field1", athlete.AthleteId.ToString()),
                new Claim("field2", "deportista"),
                new Claim("field3", "auth"),
                new Claim("field4", Guid.NewGuid().ToString()),
            };

            string stringToken = await GenerateTokenFunc(claims, 60);
            await Task.Run(() => stringToken);
            return stringToken;
        }

        public async Task<string> GenerateAthleteRefreshToken(Athlete athlete)
        {
            var claims = new Claim[]
            {
                new Claim("field1", athlete.AthleteId.ToString()),
                new Claim("field2", "deportista"),
                new Claim("field3", "refresh"),
            };

            string stringToken = await GenerateTokenFunc(claims, 21600);
            await Task.Run(() => stringToken);
            return stringToken;
        }

        public async Task<string> GeneratePasswordResetToken(int userId)
        {
            var claims = new Claim[]
            {
                new Claim("field1", userId.ToString()),
                new Claim("field2", Guid.NewGuid().ToString()),
                new Claim("field3", Guid.NewGuid().ToString()),
            };

            string stringToken = await GenerateTokenFunc(claims, 15);
            await Task.Run(() => stringToken);
            return stringToken;
        }

        private async Task<string> GenerateTokenFunc(Claim[] claims, int expirationTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecret = GetJwtSecret();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(expirationTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecret), SecurityAlgorithms.Aes128CbcHmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            await Task.Run(() => stringToken);
            return stringToken;
        }

        // Método helper para obtener el secreto JWT
        private byte[] GetJwtSecret()
        {
            return Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? _configuration["Jwt:Secret"]!);
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(GetJwtSecret()),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                return validatedToken is JwtSecurityToken;
            }
            catch
            {
                return false;
            }
        }

        public int ExtractIdFromToken()
        {
            string? userId;
            var result = 0;
            if (httpContextAccessor != null)
            {
                userId = httpContextAccessor.HttpContext!.User?.Claims?
                            .FirstOrDefault(x => x.Type == "field1")?.Value;

                if (userId != null)
                {
                    result = int.Parse(userId);
                }
                else result = 0;
            }

            return result;
        }

        public int GetIdFromRefreshToken(string refreshToken)
        {
            var userId = string.Empty;
            var result = 0;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(GetJwtSecret()),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out var validatedToken);
                userId = ((JwtSecurityToken)validatedToken).Claims
                    .FirstOrDefault(x => x.Type == "field1")?.Value;
            }
            catch (Exception)
            {
                result = 0;
            }

            userId ??= string.Empty;

            if (userId != string.Empty)
            {
                result = int.Parse(userId);
            }

            return result;
        }

        public string GetRoleFromRefreshToken(string token)
        {
            var result = string.Empty;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(GetJwtSecret()),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                result = ((JwtSecurityToken)validatedToken).Claims
                    .FirstOrDefault(x => x.Type == "field2")?.Value;
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            result ??= string.Empty;

            return result;
        }

        public string GetRoleFromToken()
        {
            string? user;
            var result = string.Empty;
            if (httpContextAccessor != null)
            {
                user = httpContextAccessor.HttpContext!.User?.Claims?
                            .FirstOrDefault(x => x.Type == "field2")?.Value;

                if (user != null)
                {
                    result = user;
                }
                else result = string.Empty;
            }

            return result;
        }

        public string GetTokenTypeFromRefreshToken(string token)
        {
            var result = string.Empty;

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(GetJwtSecret()),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                result = ((JwtSecurityToken)validatedToken).Claims
                    .FirstOrDefault(x => x.Type == "field3")?.Value;
            }
            catch (Exception)
            {
                result = string.Empty;
            }

            result ??= string.Empty;

            return result;
        }
    }
}
