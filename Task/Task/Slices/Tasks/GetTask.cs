using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities.Enums;

namespace TaskManager.Slices.Tasks;

public static class GetTask
{
    public record CommentResponse(string Text, DateTime Date, string UserName);
    public record AssigneeResponse(string Id, string Name);
    public record TeamResponse(Guid Id, string Name);

    public record Others(List<AssigneeResponse> Users, List<TeamResponse> Teams);
    
    public record Response(
        Guid Id,
        string Title,
        string Description,
        DateTime DueDate,
        Priority Priority,
        Status Status,
        List<AssigneeResponse>? Assignees,
        List<TeamResponse>? Teams,
        List<CommentResponse>? Comments,
        Others? Others
    );

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("tasks/{id}", Handler).WithTags("Tasks")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(Guid id, AppDbContext context, HttpContext httpContext)
    {
        var userTask = await context.UserTasks
            .Where(t => t.Id == id)
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Description,
                t.DueDate,
                t.Priority,
                t.Status,
                Assignees = t.Assignees.Select(a => new
                {
                    a.Id,
                    Name = a.UserClaims
                        .First(c => c.ClaimType == ClaimTypes.GivenName).ClaimValue
                }),
                Teams = t.Teams.Select(t => new
                {
                    t.Id,
                    t.Name
                }),
                Comments = t.Comments.Select(c => new
                {
                    c.Text,
                    c.Date,
                    Name = c.AppUser.UserClaims.First(c => c.ClaimType == ClaimTypes.GivenName).ClaimValue
                })

            }).SingleOrDefaultAsync();
        
        if (userTask is null)
            return Results.NotFound();

        Others? other = null;
        
        if (httpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value != UserRoles.Normal.ToString())
        {
            var otherTeams = await context.Teams.Where(t => !userTask.Teams.Select(tt => tt.Id).Contains(t.Id))
                .Select(t => new TeamResponse(t.Id, t.Name)).ToListAsync();
            
            var otherUsers = await context.Users.Where(u => !userTask.Assignees.Select(ut => ut.Id).Contains(u.Id))
                .Select(t => new AssigneeResponse(t.Id, t.UserClaims.First(c => c.ClaimType == ClaimTypes.GivenName).ClaimValue!)).ToListAsync();
            
            other = new Others(otherUsers, otherTeams);
        }
        var response = new Response(
            userTask.Id,
            userTask.Title,
            userTask.Description,
            userTask.DueDate,
            userTask.Priority,
            userTask.Status,
            userTask.Assignees.Select(a => new AssigneeResponse(a.Id,a.Name!)).ToList(),
            userTask.Teams.Select(t => new TeamResponse(t.Id, t.Name)).ToList(),
            userTask.Comments.Select(c => new CommentResponse(c.Text, c.Date, c.Name!)).ToList(),
            other
        );

        return Results.Ok(response);
    }
}