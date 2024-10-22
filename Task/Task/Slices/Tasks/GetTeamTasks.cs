using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities;
using TaskManager.Entities.Enums;

namespace TaskManager.Slices.Tasks;

public static class GetTeamTasks

{
    public record Response(
        Guid Id,
        string Title,
        string Description,
        DateTime DueDate,
        Priority Priority,
        Status Status
    );

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("teams/{teamId}/tasks", Handler).WithTags("Tasks")
                .RequireAuthorization(AuthPolicies.AdminPolicyKey);
        }
    }

    public static async Task<IResult> Handler(Guid teamId, AppDbContext context, UserManager<AppUser> userManager)
    {
        var response = await context.UserTasks
            .Where(task => task.Teams.Any(t => t.Id == teamId))
            .Select(t => new Response(
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                t.Priority,
                t.Status
            ))
            .ToListAsync();
        

        return Results.Ok(response);
    }
}