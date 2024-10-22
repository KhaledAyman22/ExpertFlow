using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities;
using TaskManager.Slices.Tasks;

namespace TaskManager.Slices.Teams;

public static class GetTeams

{
    public record TeamResponse(Guid Id, string Name);

    public record Response(
        List<TeamResponse> Teams
    );

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("teams", Handler).WithTags("Teams")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(AppDbContext context, UserManager<AppUser> userManager)
    {
        var response = await context.Teams
            .Select(t => new TeamResponse(t.Id, t.Name))
            .ToListAsync();

        return Results.Ok(new Response(response));
    }
}