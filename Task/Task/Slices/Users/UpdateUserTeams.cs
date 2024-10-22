using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities;

namespace TaskManager.Slices.Users;

public static class UpdateUserTeams
{
    public record Request(
        string UserId,
        List<Guid> Teams
    );

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPatch("users/assign", Handler).WithTags("Accounts")
                .RequireAuthorization(AuthPolicies.AdminPolicyKey);
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, UserManager<AppUser> userManager)
    {
        var user = await context.Users
            .Include(u => u.Teams)
            .FirstOrDefaultAsync(u => u.Id == request.UserId);

        if (user == null)
        {
            throw new Exception("User not found.");
        }

        var currentTeamIds = user.Teams.Select(t => t.Id).ToList();

        var teamsToRemove = user.Teams.Where(t => !request.Teams.Contains(t.Id)).ToList();

        foreach (var team in teamsToRemove)
        {
            user.Teams.Remove(team);
        }

        var teamsToAdd = await context.Teams
            .Where(t => request.Teams.Contains(t.Id) && !currentTeamIds.Contains(t.Id))
            .ToListAsync();

        foreach (var team in teamsToAdd)
        {
            user.Teams.Add(team);
        }

        await context.SaveChangesAsync();
        return Results.Ok();
    }
}