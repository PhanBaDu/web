using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020161.Admin;
using SV22T1020161.BusinessLayers;
using SV22T1020161.Models.Constants;
using SV22T1020161.Models.Security;
using System.Security.Claims;

namespace SV22T1020161.Admin.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    /// <summary>
    /// Giao diện đăng nhập
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    /// <summary>
    /// Xử lý đăng nhập
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Vui lòng nhập email và mật khẩu.");
            return View();
        }

        var userAccount = await SecurityDataService.AuthorizeAsync(email, password, UserTypes.Employee);
        if (userAccount == null)
        {
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
            return View();
        }

        var employee = await HRDataService.GetEmployeeAsync(
            int.TryParse(userAccount.UserID, out var empId) ? empId : 0
        );

        if (employee == null)
        {
            var allEmps = await HRDataService.ListEmployeesAsync(
                new SV22T1020161.Models.Common.PaginationSearchInput { Page = 1, PageSize = 1000, SearchValue = "" }
            );
            employee = allEmps.DataItems.FirstOrDefault(e =>
                e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (employee == null)
            {
                ModelState.AddModelError("", "Không tìm thấy tài khoản nhân viên.");
                return View();
            }
        }

        // Lấy danh sách Permissions từ RoleNames
        var permissions = new List<string>();
        var roles = new List<string>();

        if (!string.IsNullOrWhiteSpace(employee.RoleNames))
        {
            roles = employee.RoleNames
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            foreach (var role in roles)
            {
                var rolePerms = Roles.GetPermissions(role);
                foreach (var perm in rolePerms)
                {
                    if (!permissions.Contains(perm))
                        permissions.Add(perm);
                }
            }
        }

        // Tạo Claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, employee.EmployeeID.ToString()),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.Name, employee.FullName),
            new Claim("Photo", employee.Photo ?? ""),
        };

        // Thêm Role Claims (mỗi role một claim riêng)
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Thêm Permission Claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            claimsPrincipal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            }
        );

        HttpContext.Session.SetInt32("EmployeeID", employee.EmployeeID);
        HttpContext.Session.SetString("EmployeeEmail", employee.Email);
        HttpContext.Session.SetString("EmployeeName", employee.FullName);
        HttpContext.Session.SetString("EmployeeRoles", employee.RoleNames);

        // Chuyển hướng
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Đăng xuất
    /// </summary>
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    /// <summary>
    /// Giao diện từ chối truy cập
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    /// <summary>
    /// Giao diện hiển thị thông tin cá nhân
    /// </summary>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var employeeId = HttpContext.Session.GetInt32("EmployeeID");
        if (employeeId == null)
            return RedirectToAction("Login");

        var employee = await HRDataService.GetEmployeeAsync(employeeId.Value);
        if (employee == null)
            return RedirectToAction("Login");

        return View(employee);
    }

    /// <summary>
    /// Cập nhật thông tin cá nhân
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Profile(
        string fullName, string email, string phone, string address,
        DateTime? birthDate, IFormFile? uploadPhoto)
    {
        var employeeId = HttpContext.Session.GetInt32("EmployeeID");
        if (employeeId == null)
            return RedirectToAction("Login");

        var employee = await HRDataService.GetEmployeeAsync(employeeId.Value);
        if (employee == null)
            return RedirectToAction("Login");

        if (string.IsNullOrWhiteSpace(fullName))
            ModelState.AddModelError(nameof(fullName), "Vui lòng nhập họ tên");
        if (string.IsNullOrWhiteSpace(email))
            ModelState.AddModelError(nameof(email), "Vui lòng nhập email");
        else if (!await HRDataService.ValidateEmployeeEmailAsync(email, employee.EmployeeID))
            ModelState.AddModelError(nameof(email), "Email đã được sử dụng bởi nhân viên khác");

        if (!ModelState.IsValid)
            return View(employee);

        // Xử lý upload ảnh
        if (uploadPhoto != null && uploadPhoto.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
            var filePath = Path.Combine(ApplicationContext.WWWRootPath, "images", "employees", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await uploadPhoto.CopyToAsync(stream);
            }
            employee.Photo = fileName;

            // Cập nhật lại claim Photo
            var identity = (ClaimsIdentity?)User.Identity;
            var photoClaim = identity?.FindFirst("Photo");
            if (photoClaim != null)
                identity?.RemoveClaim(photoClaim);
            identity?.AddClaim(new Claim("Photo", fileName));
        }

        employee.FullName = fullName.Trim();
        employee.Email = email.Trim();
        employee.Phone = phone?.Trim() ?? "";
        employee.Address = address?.Trim() ?? "";
        employee.BirthDate = birthDate;

        var result = await HRDataService.UpdateEmployeeAsync(employee);
        if (!result)
        {
            ModelState.AddModelError("", "Không thể cập nhật thông tin. Vui lòng thử lại.");
            return View(employee);
        }

        // Cập nhật session
        HttpContext.Session.SetString("EmployeeName", employee.FullName);
        HttpContext.Session.SetString("EmployeeEmail", employee.Email);

        // Cập nhật lại authentication cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var roles = employee.RoleNames
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
        var permissions = new List<string>();
        foreach (var role in roles)
        {
            var perms = Roles.GetPermissions(role);
            foreach (var perm in perms)
            {
                if (!permissions.Contains(perm))
                    permissions.Add(perm);
            }
        }

        var newClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, employee.EmployeeID.ToString()),
            new Claim(ClaimTypes.Email, employee.Email),
            new Claim(ClaimTypes.Name, employee.FullName),
            new Claim("Photo", employee.Photo ?? ""),
        };
        foreach (var role in roles)
            newClaims.Add(new Claim(ClaimTypes.Role, role));
        foreach (var perm in permissions)
            newClaims.Add(new Claim("Permission", perm));

        var newIdentity = new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        var newPrincipal = new ClaimsPrincipal(newIdentity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            newPrincipal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            }
        );

        TempData["SuccessMessage"] = "Cập nhật hồ sơ thành công.";
        return RedirectToAction("Profile");
    }

    /// <summary>
    /// Giao diện thay đổi mật khẩu
    /// </summary>
    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        if (HttpContext.Session.GetInt32("EmployeeID") == null)
            return RedirectToAction("Login");

        return View();
    }

    /// <summary>
    /// Xử lý thay đổi mật khẩu
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        var employeeId = HttpContext.Session.GetInt32("EmployeeID");
        if (employeeId == null)
            return RedirectToAction("Login");

        if (string.IsNullOrWhiteSpace(oldPassword))
            ModelState.AddModelError(nameof(oldPassword), "Vui lòng nhập mật khẩu cũ");

        if (string.IsNullOrWhiteSpace(newPassword))
            ModelState.AddModelError(nameof(newPassword), "Vui lòng nhập mật khẩu mới");
        else if (newPassword.Length < 6)
            ModelState.AddModelError(nameof(newPassword), "Mật khẩu mới phải có ít nhất 6 ký tự");

        if (newPassword != confirmPassword)
            ModelState.AddModelError(nameof(confirmPassword), "Mật khẩu xác nhận không khớp");

        if (!ModelState.IsValid)
            return View();

        var employee = await HRDataService.GetEmployeeAsync(employeeId.Value);
        if (employee == null)
            return RedirectToAction("Login");

        var authResult = await SecurityDataService.AuthorizeAsync(employee.Email, oldPassword, UserTypes.Employee);
        if (authResult == null)
        {
            ModelState.AddModelError(nameof(oldPassword), "Mật khẩu cũ không đúng");
            return View();
        }

        var result = await HRDataService.ChangeEmployeePasswordAsync(employeeId.Value, newPassword);
        if (!result)
        {
            ModelState.AddModelError("", "Không thể đổi mật khẩu. Vui lòng thử lại.");
            return View();
        }

        TempData["SuccessMessage"] = "Đổi mật khẫu thành công.";
        return RedirectToAction("ChangePassword");
    }
}
