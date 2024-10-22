using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities.Enums;

namespace TaskManager.Slices.Tasks;

public class GetTasks
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
            app.MapGet("/tasks", Handler).WithTags("Tasks")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(AppDbContext context, HttpContext httpContext)
    {
        var role = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)!.Value;

        List<Response>? response;

        response = Enum.Parse<UserRoles>(role) switch
        {
            UserRoles.Administrator => await GetTasksForAdmin(context),
            UserRoles.Lead => await GetTasksForLead(context, httpContext),
            UserRoles.Normal => await GetTasksForNormal(context, httpContext)
        };

        return Results.Ok(response);
    }

    private static async Task<List<Response>> GetTasksForAdmin(AppDbContext context)
    {
        var response = await context.UserTasks
            .Select(t => new Response(
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                t.Priority,
                t.Status
            ))
            .ToListAsync();
        return response;
    }

    private static async Task<List<Response>> GetTasksForNormal(AppDbContext context, HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        var response = await context.UserTasks
            .Where(task =>
                task.Assignees.Any(a => a.Id == userId) || task.Teams.Any(t => t.Users.Any(u => u.Id == userId)))
            .Select(t => new Response(
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                t.Priority,
                t.Status
            ))
            .ToListAsync();
        return response;
    }
   
    private static async Task<List<Response>> GetTasksForLead(AppDbContext context, HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        var ownTasks = await context.UserTasks
            .Where(task =>
                task.Assignees.Any(a => a.Id == userId) || task.Teams.Any(t => t.Users.Any(u => u.Id == userId)))
            .Select(t => new Response(
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                t.Priority,
                t.Status
            ))
            .ToListAsync();
        
        var managedUsers = context.Users.Where(u => u.Id == userId)
            .SelectMany(u => u.ManagedUsers);

        var mangedUsersTasks = await managedUsers.SelectMany(u => u.Tasks)
            .Union(managedUsers.SelectMany(u => u.Teams).SelectMany(t => t.Tasks))
            .Select(t => new Response(
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                t.Priority,
                t.Status
            ))
            .ToListAsync();
        
        return ownTasks.Concat(mangedUsersTasks).ToList();
    }
}