using Domain.Entities.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Api.Attributes
{
    public class ValidateRefreshTokenAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("RefreshToken", out var token))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var jwtKey = context.HttpContext.RequestServices.GetRequiredService<IOptions<JwtConfiguration>>().Value;
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            }
            catch
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
