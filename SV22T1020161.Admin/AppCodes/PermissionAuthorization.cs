using Microsoft.AspNetCore.Authorization;
using SV22T1020161.Models.Constants;

namespace SV22T1020161.Admin.AppCodes;

/// <summary>
/// Requirement cho kiểm tra quyền cụ thể
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

/// <summary>
/// Handler kiểm tra quyền dựa trên Claims của user
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var roleClaims = context.User.FindAll(System.Security.Claims.ClaimTypes.Role).ToList();

        if (!roleClaims.Any())
        {
            return Task.CompletedTask;
        }

        foreach (var roleClaim in roleClaims)
        {
            var roleName = roleClaim.Value;
            var permissions = Roles.GetPermissions(roleName);

            if (permissions.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Attribute cho phép yêu cầu quyền cụ thể trên Action/Controller
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizePermissionAttribute : AuthorizeAttribute
{
    public AuthorizePermissionAttribute(params string[] permissions)
    {
        var combinedPolicy = "Permission_" + string.Join("_", permissions.OrderBy(p => p)).Replace(":", "_");
        Policy = combinedPolicy;
    }
}

/// <summary>
/// Attribute cho phép yêu cầu quyền với quyền mặc định
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeRoleAttribute : AuthorizeAttribute
{
    public AuthorizeRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
