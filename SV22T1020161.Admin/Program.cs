using SV22T1020161.Admin;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using SV22T1020161.Models.Constants;
using SV22T1020161.Admin.AppCodes;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
                .AddMvcOptions(option =>
                {
                    option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                });

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.Cookie.Name = "SV22T1020161.Admin";
                    option.LoginPath = "/Account/Login";
                    option.AccessDeniedPath = "/Account/AccessDenied";
                    option.ExpireTimeSpan = TimeSpan.FromDays(7);
                    option.SlidingExpiration = true;
                    option.Cookie.HttpOnly = true;
                    option.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });

// Configure Authorization - Đăng ký Permission Handler
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// Đăng ký Policy yêu cầu vai trò cụ thể + mỗi Permission
builder.Services.AddAuthorization(options =>
{
    // Policy mặc định: yêu cầu đăng nhập (Authenticated)
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Policy AdminOnly
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(Roles.Admin));

    // Policy ManagerOrAdmin
    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireRole(Roles.Admin, Roles.Manager));

    // Policy cho mỗi Permission
    foreach (var role in Roles.RolePermissions)
    {
        foreach (var permission in role.Value)
        {
            var policyName = "Permission_" + permission.Replace(":", "_");
            options.AddPolicy(policyName, policy =>
            {
                policy.AddRequirements(new PermissionRequirement(permission));
            });
        }
    }
});

// Configure Session
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromHours(2);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

//Configure Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//Configure default format
var cultureInfo = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

//Configure Application Context
ApplicationContext.Configure
(
    httpContextAccessor: app.Services.GetRequiredService<IHttpContextAccessor>(),
    webHostEnvironment: app.Services.GetRequiredService<IWebHostEnvironment>(),
    configuration: app.Configuration
);

//Get Connection String from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB")
    ?? throw new InvalidOperationException("ConnectionString 'LiteCommerceDB' not found.");

// Initialize Business Layer Configuration
SV22T1020161.BusinessLayers.Configuration.Initialize(connectionString);

app.Run();