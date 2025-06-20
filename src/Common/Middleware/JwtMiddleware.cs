using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TrueCodeTestTask.Common.Middleware;

public class JwtMiddleware(RequestDelegate next, IConfiguration configuration)
{

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            AttachUserToContext(context, token);
        }

        await next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["JWT:SecretKey"] ?? "your-super-secret-jwt-key-here-make-it-long-and-secure");

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["JWT:Issuer"] ?? "TrueCodeTestTask",
                ValidateAudience = true,
                ValidAudience = configuration["JWT:Audience"] ?? "TrueCodeTestTask",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "userId").Value;
            var userName = jwtToken.Claims.First(x => x.Type == "userName").Value;

            // Attach user to context on successful jwt validation
            context.Items["UserId"] = userId;
            context.Items["UserName"] = userName;
        }
        catch
        {
            // Do nothing if jwt validation fails
            // User is not attached to context so request won't have access to secure routes
        }
    }
}
