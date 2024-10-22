using System.Security.Claims;
using FluentValidation;
using TaskManager.Endpoints;
using TaskManager.Entities;
using TaskManager.Entities.Enums;

namespace TaskManager.Slices.Tasks;

public static class CreateTask
{
    public record Request(
        string Title,
        string Description,
        DateTime DueDate,
        Priority Priority,
        Status Status,
        List<string> Attachments
    );

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Title).MaximumLength(200);
            RuleFor(r => r.Description).MaximumLength(1000);
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("tasks", Handler).WithTags("Tasks")
                .RequireAuthorization(AuthPolicies.NormalPolicyKey);
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IValidator<Request> validator, HttpContext httpContext)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var user = context.Users.First(u => u.Id == userId);

        var task = new UserTask()
        {
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            DueDate = request.DueDate,
            Priority = request.Priority,
            Assignees = [user],
            Attachments = request.Attachments?.Select(a => new Attachment()
            {
                File = Convert.FromBase64String(a),
            })?.ToList()!
        };
        
        await context.UserTasks.AddAsync(task);

        await context.SaveChangesAsync();
        
        return Results.Created($"/userTasks/{task.Id}", null);
    }
}