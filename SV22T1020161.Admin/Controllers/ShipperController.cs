using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020161.Admin;
using SV22T1020161.BusinessLayers;
using SV22T1020161.Models.Common;
using SV22T1020161.Models.Constants;
using SV22T1020161.Models.Partner;

namespace SV22T1020161.Admin.Controllers
{
    /// <summary>
    /// Controller quản lý người giao hàng (Shipper).
    /// </summary>
    [Authorize]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string SHIPPER_SEARCH_CONDITION = "ShipperSearchCondition";

        /// <summary>
        /// Giao diện tìm kiếm và hiển thị danh sách người giao hàng
        /// </summary>
        public IActionResult Index()
        {
            if (!ApplicationContext.HasPermission(Permissions.ShipperView))
                return RedirectToAction("AccessDenied", "Account");
            var input = ApplicationContext.GetSessionData<PaginationSearchInput>(SHIPPER_SEARCH_CONDITION);
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
        /// Tìm kiếm và hiển thị danh sách người giao hàng (partial view)
        /// </summary>
        public async Task<IActionResult> Search(int page = 1, string searchValue = "")
        {
            if (!ApplicationContext.HasPermission(Permissions.ShipperView))
                return Forbid();
            var input = new PaginationSearchInput()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue
            };
            ApplicationContext.SetSessionData(SHIPPER_SEARCH_CONDITION, input);
            var data = await PartnerDataService.ListShippersAsync(input);
            return PartialView(data);
        }

        /// <summary>
        /// Giao diện thêm mới người giao hàng
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            if (!ApplicationContext.HasPermission(Permissions.ShipperCreate))
                return RedirectToAction("AccessDenied", "Account");
            ViewBag.Title = "Bổ sung người giao hàng";
            var model = new Shipper { ShipperID = 0 };
            return View(model);
        }

        /// <summary>
        /// Xử lý thêm mới người giao hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(Shipper data)
        {
            if (!ApplicationContext.HasPermission(Permissions.ShipperCreate))
                return RedirectToAction("AccessDenied", "Account");
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Vui lòng nhập tên người giao hàng");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(data.Phone, @"^[0-9]{9,15}$"))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại phải là chữ số, từ 9 đến 15 ký tự");
            if (!ModelState.IsValid)
                return View(data);
            data.Phone ??= "";
            await PartnerDataService.AddShipperAsync(data);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Giao diện chỉnh sửa người giao hàng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.ShipperEdit))
                return RedirectToAction("AccessDenied", "Account");
            ViewBag.Title = "Cập nhật người giao hàng";
            var model = await PartnerDataService.GetShipperAsync(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// Xử lý cập nhật thông tin người giao hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Edit(Shipper data)
        {
            if (!ApplicationContext.HasPermission(Permissions.ShipperEdit))
                return RedirectToAction("AccessDenied", "Account");
            if (string.IsNullOrWhiteSpace(data.ShipperName))
                ModelState.AddModelError(nameof(data.ShipperName), "Vui lòng nhập tên người giao hàng");
            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Vui lòng nhập số điện thoại");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(data.Phone, @"^[0-9]{9,15}$"))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại phải là chữ số, từ 9 đến 15 ký tự");
            if (!ModelState.IsValid)
                return View(data);
            data.Phone ??= "";
            await PartnerDataService.UpdateShipperAsync(data);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Giao diện xác nhận xóa người giao hàng
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.ShipperDelete))
                return RedirectToAction("AccessDenied", "Account");
            ViewBag.Title = "Xóa người giao hàng";
            var model = await PartnerDataService.GetShipperAsync(id);
            if (model == null) return RedirectToAction("Index");
            return View(model);
        }

        /// <summary>
        /// Xử lý xóa người giao hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string confirm)
        {
            if (!ApplicationContext.HasPermission(Permissions.ShipperDelete))
                return RedirectToAction("AccessDenied", "Account");
            bool isUsed = await PartnerDataService.IsUsedShipperAsync(id);
            if (isUsed)
            {
                TempData["ErrorMessage"] = "Không thể xóa người giao hàng này (có thể đang có đơn hàng liên quan).";
                return RedirectToAction("Delete", new { id = id });
            }
            bool result = await PartnerDataService.DeleteShipperAsync(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Không thể xóa người giao hàng này.";
                return RedirectToAction("Delete", new { id = id });
            }
            return RedirectToAction("Index");
        }
    }
}
