using System.Security.Claims;
using FluentValidation;
using TaskManager.Endpoints;
using TaskManager.Entities;

namespace TaskManager.Slices.Teams;

public static class CreateTeam
{
    public record Request(
        string Name
    );

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(r => r.Name).MaximumLength(50);
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("teams", Handler).WithTags("Teams")
                .RequireAuthorization(AuthPolicies.AdminPolicyKey);
        }
    }

    public static async Task<IResult> Handler(Request request, AppDbContext context, IValidator<Request> validator, HttpContext httpContext)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var team = new Team()
        {
           Name = request.Name
        };

        await context.Teams.AddAsync(team);

        await context.SaveChangesAsync();
        
        return Results.Created($"/teams/{team.Id}", null);
    }
}