using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020161.Admin;
using SV22T1020161.Admin.AppCodes;
using SV22T1020161.BusinessLayers;
using SV22T1020161.Models.Common;
using SV22T1020161.Models.Constants;
using SV22T1020161.Models.HR;

namespace SV22T1020161.Admin.Controllers
{
    /// <summary>
    /// Controller quản lý nhân viên.
    /// Chỉ Admin mới có quyền CRUD nhân viên và phân quyền.
    /// </summary>
    [Authorize]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string EMPLOYEE_SEARCH_CONDITION = "EmployeeSearchCondition";

        /// <summary>
        /// Giao diện tìm kiếm và hiển thị danh sách nhân viên
        /// </summary>
        [AuthorizePermission(Permissions.EmployeeView)]
        public IActionResult Index()
        {
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(EMPLOYEE_SEARCH_CONDITION);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }

        /// <summary>
        /// Tìm kiếm và hiển thị danh sách nhân viên (partial view)
        /// </summary>
        [AuthorizePermission(Permissions.EmployeeView)]
        public async Task<IActionResult> Search(int page = 1, string searchValue = "")
        {
            var input = new PaginationSearchInput()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue
            };
            ApplicationContext.SetSessionData(EMPLOYEE_SEARCH_CONDITION, input);
            var data = await HRDataService.ListEmployeesAsync(input);
            return PartialView(data);
        }

        /// <summary>
        /// Giao diện thêm mới nhân viên (Admin only)
        /// </summary>
        [HttpGet]
        [AuthorizePermission(Permissions.EmployeeCreate)]
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhân viên";
            var model = new Employee() { EmployeeID = 0, IsWorking = true };
            return View("Edit", model);
        }

        /// <summary>
        /// Giao diện chỉnh sửa thông tin nhân viên
        /// </summary>
        [HttpGet]
        [AuthorizePermission(Permissions.EmployeeEdit)]
        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var model = await HRDataService.GetEmployeeAsync(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// Xử lý lưu dữ liệu (thêm hoặc cập nhật)
        /// </summary>
        [HttpPost]
        [AuthorizePermission(Permissions.EmployeeCreate, Permissions.EmployeeEdit)]
        public async Task<IActionResult> SaveData(Employee data, IFormFile? uploadPhoto)
        {
            try
            {
                ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

                // Kiểm tra quyền
                if (data.EmployeeID == 0 && !ApplicationContext.HasPermission(Permissions.EmployeeCreate))
                    return RedirectToAction("AccessDenied", "Account");
                if (data.EmployeeID != 0 && !ApplicationContext.HasPermission(Permissions.EmployeeEdit))
                    return RedirectToAction("AccessDenied", "Account");

                //Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(data.FullName))
                    ModelState.AddModelError(nameof(data.FullName), "Vui lòng nhập họ tên nhân viên");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email nhân viên");
                else if (!await HRDataService.ValidateEmployeeEmailAsync(data.Email, data.EmployeeID))
                    ModelState.AddModelError(nameof(data.Email), "Email đã được sử dụng bởi nhân viên khác");

                if (!ModelState.IsValid)
                    return View("Edit", data);

                //Xử lý upload ảnh
                if (uploadPhoto != null)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(uploadPhoto.FileName)}";
                    var filePath = Path.Combine(ApplicationContext.WWWRootPath, "images/employees", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await uploadPhoto.CopyToAsync(stream);
                    }
                    data.Photo = fileName;
                }

                if (string.IsNullOrEmpty(data.Address)) data.Address = "";
                if (string.IsNullOrEmpty(data.Phone)) data.Phone = "";
                if (string.IsNullOrEmpty(data.Photo)) data.Photo = "nophoto.png";

                if (data.EmployeeID == 0)
                    await HRDataService.AddEmployeeAsync(data);
                else
                    await HRDataService.UpdateEmployeeAsync(data);

                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Hệ thống đang bận hoặc dữ liệu không hợp lệ. Vui lòng kiểm tra dữ liệu hoặc thử lại sau");
                return View("Edit", data);
            }
        }

        /// <summary>
        /// Giao diện xác nhận xóa nhân viên (Admin only)
        /// </summary>
        [HttpGet]
        [AuthorizePermission(Permissions.EmployeeDelete)]
        public async Task<IActionResult> Delete(int id)
        {
            ViewBag.Title = "Xóa nhân viên";
            var data = await HRDataService.GetEmployeeAsync(id);
            if (data == null) return RedirectToAction("Index");
            return View(data);
        }

        /// <summary>
        /// Xử lý xóa nhân viên
        /// </summary>
        [HttpPost]
        [AuthorizePermission(Permissions.EmployeeDelete)]
        public async Task<IActionResult> Delete(int id, string confirm)
        {
            bool result = await HRDataService.DeleteEmployeeAsync(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Không thể xóa nhân viên này (có thể do đang có dữ liệu liên quan).";
                return RedirectToAction("Delete", new { id = id });
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Giao diện thay đổi mật khẩu nhân viên (Admin only)
        /// </summary>
        [HttpGet]
        [AuthorizePermission(Permissions.EmployeeChangePassword)]
        public async Task<IActionResult> ChangePassword(int id)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null) return RedirectToAction("Index");
            ViewBag.Employee = employee;
            ViewBag.Title = $"Đổi mật khẩu: {employee.FullName}";
            return View();
        }

        /// <summary>
        /// Xử lý thay đổi mật khẩu nhân viên
        /// </summary>
        [HttpPost]
        [AuthorizePermission(Permissions.EmployeeChangePassword)]
        public async Task<IActionResult> ChangePassword(int id, string newPassword, string confirmPassword)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null) return RedirectToAction("Index");
            ViewBag.Employee = employee;
            ViewBag.Title = $"Đổi mật khẩu: {employee.FullName}";

            if (string.IsNullOrWhiteSpace(newPassword))
                ModelState.AddModelError(nameof(newPassword), "Vui lòng nhập mật khẩu mới");
            else if (newPassword.Length < 6)
                ModelState.AddModelError(nameof(newPassword), "Mật khẩu phải có ít nhất 6 ký tự");
            else if (newPassword != confirmPassword)
                ModelState.AddModelError(nameof(confirmPassword), "Mật khẩu xác nhận không khớp");

            if (!ModelState.IsValid)
                return View();

            var result = await HRDataService.ChangeEmployeePasswordAsync(id, newPassword);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "Không thể cập nhật mật khẩu. Vui lòng thử lại.");
                return View();
            }

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Giao diện thay đổi quyền của nhân viên (Admin only)
        /// </summary>
        [HttpGet]
        [AuthorizePermission(Permissions.EmployeeAssignRole)]
        public async Task<IActionResult> ChangeRole(int id)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null) return RedirectToAction("Index");
            ViewBag.Employee = employee;
            ViewBag.Title = $"Phân quyền: {employee.FullName}";
            return View();
        }

        /// <summary>
        /// Xử lý thay đổi quyền của nhân viên
        /// </summary>
        [HttpPost]
        [AuthorizePermission(Permissions.EmployeeAssignRole)]
        public async Task<IActionResult> ChangeRole(int id, List<string> roles)
        {
            var employee = await HRDataService.GetEmployeeAsync(id);
            if (employee == null) return RedirectToAction("Index");

            // Admin không thể bị tước quyền Admin
            if (employee.RoleNames.Contains("Admin") && !ApplicationContext.HasRole("Admin"))
            {
                TempData["ErrorMessage"] = "Bạn không có quyền thay đổi quyền của Admin khác.";
                return RedirectToAction("ChangeRole", new { id });
            }

            var roleNames = roles != null ? string.Join(",", roles) : "";
            var result = await HRDataService.UpdateEmployeeRoleNamesAsync(id, roleNames);

            if (!result)
            {
                TempData["ErrorMessage"] = "Không thể cập nhật quyền. Vui lòng thử lại.";
                return RedirectToAction("ChangeRole", new { id });
            }

            TempData["SuccessMessage"] = "Phân quyền thành công.";
            return RedirectToAction("Index");
        }
    }
}
