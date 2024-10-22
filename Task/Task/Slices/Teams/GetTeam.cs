using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities;
using TaskManager.Slices.Tasks;

namespace TaskManager.Slices.Teams;

public static class GetTeam
{
    public record UserResponse(string Id, string Name, string Role);

    public record Response(
        string Name,
        List<UserResponse>? Members
    );

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("teams/{id}", Handler).WithTags("Teams")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(Guid id, AppDbContext context, UserManager<AppUser> userManager)
    {
        var team = await context.Teams
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Name,
                Users = t.Users.Select(u => new
                {
                    Id = u.Id,
                    
                    Name = u.UserClaims
                        .FirstOrDefault(c => c.ClaimType == ClaimTypes.GivenName)!.ClaimValue,

                    Role = u.UserClaims
                        .FirstOrDefault(c => c.ClaimType == ClaimTypes.Role)!.ClaimValue
                })
            })
            .FirstOrDefaultAsync();

        
        if (team is null)
            return Results.NotFound();
        
        var response = new Response(
           team.Name,
           team.Users.Select(u => new UserResponse(u.Id, u.Name!, u.Role!)).ToList()
        );

        return Results.Ok(response);
    }
}