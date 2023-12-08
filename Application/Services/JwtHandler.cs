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
                new Claim("field2", gym.Email.ToString()),
                new Claim("field3", Guid.NewGuid().ToString()),
                new Claim("field4", Guid.NewGuid().ToString()),
            };

            string stringToken = await GenerateTokenFunc(claims, 15);
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
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(expirationTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.Aes128CbcHmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            await Task.Run(() => stringToken);
            return stringToken;
        }

        public bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!)),
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

        public int ExtractGymIdFromToken()
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
    }
}
