using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public static class WebSocketExtensions
{
    public static IApplicationBuilder UseJwtWebSocketAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<JwtWebSocketMiddleware>();
    }
}

public class JwtWebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _conf;

    public JwtWebSocketMiddleware(RequestDelegate next, IConfiguration conf)
    {
        _next = next;
        _conf = conf;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var token = context.Request.Query["token"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                    if (jsonToken != null)
                    {
                        var validationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf["JWT:Key"])),
                            ValidateIssuer = true,
                            ValidIssuer = _conf["JWT:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = _conf["JWT:Audience"],
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };

                        SecurityToken validatedToken;
                       
                        var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
                  
                        context.User = principal;

                        await _next.Invoke(context);
                        return;
                    }
                }
                catch (Exception)
                {
                    // Token validation failed
                }
            }

            context.Response.StatusCode = 401;
            return;
        }

        // Not a WebSocket request, continue to the next middleware
        await _next.Invoke(context);
    }
}
