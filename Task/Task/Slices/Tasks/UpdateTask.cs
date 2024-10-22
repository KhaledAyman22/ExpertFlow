using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities;
using TaskManager.Entities.Enums;

namespace TaskManager.Slices.Tasks;

public static class UpdateTask
{
    public record Request(Guid TaskId, Status Status, List<Guid> Teams, List<string> Assignees);


    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut("tasks", Handler).WithTags("Tasks")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, UserManager<AppUser> userManager)
    {
        var task = await context.UserTasks
            .Include(t => t.Teams)
            .Include(t => t.Assignees)
            .Where(t => t.Id == request.TaskId)
            .SingleOrDefaultAsync();

        if (task is null)
            return Results.NotFound();

        task.Status = request.Status;
        
        
        var currentTeamIds = task.Teams.Select(t => t.Id).ToList();

        var teamsToRemove = task.Teams.Where(t => !request.Teams.Contains(t.Id)).ToList();

        foreach (var team in teamsToRemove)
        {
            task.Teams.Remove(team);
        }

        var teamsToAdd = await context.Teams
            .Where(t => request.Teams.Contains(t.Id) && !currentTeamIds.Contains(t.Id))
            .ToListAsync();

        foreach (var team in teamsToAdd)
        {
            task.Teams.Add(team);
        }
        
        
        var currentAssigneeIds = task.Assignees.Select(t => t.Id).ToList();

        var assigneesToRemove = task.Assignees.Where(t => !request.Assignees.Contains(t.Id)).ToList();

        foreach (var assignee in assigneesToRemove)
        {
            task.Assignees.Remove(assignee);
        }

        var assigneesToAdd = await context.Users
            .Where(t => request.Assignees.Contains(t.Id) && !currentAssigneeIds.Contains(t.Id))
            .ToListAsync();

        foreach (var assignee in assigneesToAdd)
        {
            task.Assignees.Add(assignee);
        }
        
        await context.SaveChangesAsync();
        
        return Results.Ok();
    }
}