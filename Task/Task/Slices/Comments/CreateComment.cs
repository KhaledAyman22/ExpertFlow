using System.Security.Claims;
using FluentValidation;
using TaskManager.Endpoints;
using TaskManager.Entities;

namespace TaskManager.Slices.Comments;

public static class CreateComment
{
    public record Request(
        string Text,
        DateTime Date,
        Guid TaskId
    );

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Text).MaximumLength(1000);
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("comments", Handler).WithTags("Comments")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IValidator<Request> validator, HttpContext httpContext)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var comment = new Comment()
        {
            Text = request.Text,
            Date = request.Date,
            TaskId =  request.TaskId,
            UserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        };

        await context.Comments.AddAsync(comment);

        await context.SaveChangesAsync();
        
        return Results.Created();
    }
}