using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using TaskManager.Endpoints;
using TaskManager.Entities;

namespace TaskManager.Slices.Users;

public static class GetUserData
{
    public record Response(string Name, int Role);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("user/role", Handler).WithTags("Accounts")
                .RequireAuthorization();
        }
    }

    public static IResult Handler(AppDbContext context, HttpContext httpContext, UserManager<AppUser> userManager)
    {
        var claims = httpContext.User.Claims.Where(c => c.Type == ClaimTypes.GivenName || c.Type == ClaimTypes.Role)
            .ToDictionary(c => c.Type);
        
        
        var response = new Response(
            claims[ClaimTypes.GivenName].Value,
            (int)Enum.Parse(typeof(UserRoles), claims[ClaimTypes.Role].Value)
        );

        return Results.Ok(response);
    }
}