using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TaskManager.Entities;

namespace TaskManager;

public static class AuthPolicies
{
    public static readonly string AdminPolicyKey = nameof(AdminPolicy);
    public static readonly string LeadPolicyKey = nameof(LeadPolicy);
    public static readonly string NormalPolicyKey = nameof(NormalPolicy);
    
    public static readonly AuthorizationPolicy AdminPolicy = new AuthorizationPolicyBuilder()
        .RequireClaim(ClaimTypes.Role, UserRoles.Administrator.ToString())
        .Build();

    public static readonly AuthorizationPolicy  LeadPolicy = new AuthorizationPolicyBuilder()
        .RequireClaim(ClaimTypes.Role, UserRoles.Lead.ToString(), UserRoles.Administrator.ToString())
        .Build();

    public static readonly AuthorizationPolicy  NormalPolicy = new AuthorizationPolicyBuilder()
        .RequireClaim(ClaimTypes.Role, UserRoles.Normal.ToString(), UserRoles.Lead.ToString(), UserRoles.Administrator.ToString())
        .Build();
}