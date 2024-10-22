using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManager.Endpoints;
using TaskManager.Entities;

namespace TaskManager.Slices.Users;

public static class GetUsers
{
    public record UserResponse(string Id, string Name, int Role, List<Guid> Teams);

    public record Response(List<UserResponse> Users);

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("users/{page}", Handler).WithTags("Accounts")
                .RequireAuthorization();
        }
    }

    public static async Task<IResult> Handler(int page, AppDbContext context)
    {
        var users = await context.Users
        .Select(u => new
        {
            u.Id,
            Name = u.UserClaims.FirstOrDefault(c => c.ClaimType == ClaimTypes.GivenName)!.ClaimValue,
            Role = u.UserClaims.FirstOrDefault(c => c.ClaimType == ClaimTypes.Role)!.ClaimValue,
            Teams = u.Teams.Select(t => t.Id)
        }).Skip((page - 1) * 10).Take(10).ToListAsync();


        var response = users.Select(u =>
            new UserResponse(u.Id, u.Name!, (int)Enum.Parse(typeof(UserRoles), u.Role!), u.Teams.ToList())
        ).ToList();

        return Results.Ok(new Response(response));
    }
}