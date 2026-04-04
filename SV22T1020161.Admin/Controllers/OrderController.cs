using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SV22T1020161.Admin;
using SV22T1020161.Admin.AppCodes;
using SV22T1020161.BusinessLayers;
using SV22T1020161.Models.Common;
using SV22T1020161.Models.Constants;
using SV22T1020161.Models.Sales;
using SV22T1020161.Models.Catalog;
using SV22T1020161.Models.Partner;
using System.Text.Json;

namespace SV22T1020161.Admin.Controllers
{
    /// <summary>
    /// Controller quản lý đơn hàng (Order).
    /// Phân quyền chi tiết theo từng hành động.
    /// </summary>
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 10;
        private const string ORDER_SEARCH_CONDITION = "OrderSearchCondition";

        // ===== Search & Index =====

        /// <summary>
        /// Giao diện tìm kiếm và hiển thị danh sách đơn hàng
        /// </summary>
        public IActionResult Index()
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderView))
                return RedirectToAction("AccessDenied", "Account");
            var input = ApplicationContext.GetSessionData<OrderSearchInput>(ORDER_SEARCH_CONDITION);
            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    CustomerID = null,
                    DateFrom = null,
                    DateTo = null
                };
            }
            return View(input);
        }

        /// <summary>
        /// Tìm kiếm và hiển thị danh sách đơn hàng (partial view)
        /// </summary>
        public async Task<IActionResult> Search(int page = 1, int status = 0, string searchValue = "",
            int? customerID = null, string? dateFrom = null, string? dateTo = null)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderView))
                return Forbid();
            DateTime? fromDate = null, toDate = null;
            if (!string.IsNullOrEmpty(dateFrom))
                fromDate = DateTime.TryParse(dateFrom, out var fd) ? fd : null;
            if (!string.IsNullOrEmpty(dateTo))
                toDate = DateTime.TryParse(dateTo, out var td) ? td : null;

            var input = new OrderSearchInput()
            {
                Page = page,
                PageSize = PAGE_SIZE,
                SearchValue = searchValue ?? "",
                Status = (OrderStatusEnum)status,
                CustomerID = customerID,
                DateFrom = fromDate,
                DateTo = toDate
            };
            ApplicationContext.SetSessionData(ORDER_SEARCH_CONDITION, input);

            var data = await SalesDataService.ListOrdersAsync(input);
            return PartialView(data);
        }

        // ===== Detail =====

        /// <summary>
        /// Giao diện xem thông tin chi tiết của một đơn hàng
        /// </summary>
        public async Task<IActionResult> Detail(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderView))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");

            var details = await SalesDataService.ListDetailsAsync(id);
            ViewBag.Details = details;
            ViewBag.Title = $"Chi tiết đơn hàng #{id}";
            return View(order);
        }

        // ===== Create Order =====

        /// <summary>
        /// Giao diện lập đơn hàng mới
        /// </summary>
        public async Task<IActionResult> Create()
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderCreate))
                return RedirectToAction("AccessDenied", "Account");

            var customers = await PartnerDataService.ListCustomersAsync(new PaginationSearchInput { Page = 1, PageSize = 500, SearchValue = "" });
            var products = await CatalogDataService.ListProductsAsync(new ProductSearchInput { Page = 1, PageSize = 20, SearchValue = "" });
            var provinces = await DictionaryDataService.ListProvincesAsync();
            var cart = GetCart();

            ViewBag.Customers = customers.DataItems;
            ViewBag.Products = products.DataItems;
            ViewBag.Provinces = provinces;
            ViewBag.Cart = cart;
            ViewBag.Title = "Lập đơn hàng mới";
            return View();
        }

        /// <summary>
        /// Tìm sản phẩm khi lập đơn hàng (AJAX)
        /// </summary>
        public async Task<IActionResult> SearchProducts(string searchValue = "", int page = 1)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderCreate))
                return Forbid();
            var input = new ProductSearchInput { Page = page, PageSize = 20, SearchValue = searchValue };
            var products = await CatalogDataService.ListProductsAsync(input);
            return Json(products);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng (AJAX)
        /// </summary>
        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderCreate))
                return Json(new { success = false, message = "Bạn không có quyền thực hiện." });
            var product = CatalogDataService.GetProductAsync(productId).Result;
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.ProductID == productId);
            if (existing != null)
                existing.Quantity++;
            else
                cart.Add(new OrderDetail { OrderID = 0, ProductID = product.ProductID, SalePrice = product.Price, Quantity = 1 });
            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.Sum(c => c.Quantity) });
        }

        /// <summary>
        /// Cập nhật số lượng sản phẩm trong giỏ hàng (AJAX)
        /// </summary>
        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderCreate))
                return Json(new { success = false, message = "Bạn không có quyền thực hiện." });
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductID == productId);
            if (item != null)
            {
                if (quantity <= 0) cart.Remove(item);
                else item.Quantity = quantity;
                SaveCart(cart);
            }
            return Json(new { success = true, total = cart.Sum(c => c.TotalPrice) });
        }

        /// <summary>
        /// Xóa sản phẩm khỏi giỏ hàng (AJAX)
        /// </summary>
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderCreate))
                return Json(new { success = false, message = "Bạn không có quyền thực hiện." });
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.ProductID == productId);
            if (item != null) cart.Remove(item);
            SaveCart(cart);
            return Json(new { success = true, cartCount = cart.Sum(c => c.Quantity) });
        }

        /// <summary>
        /// Lưu đơn hàng mới
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveOrder(int customerID, string deliveryProvince, string deliveryAddress)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderCreate))
                return RedirectToAction("AccessDenied", "Account");
            if (customerID == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn khách hàng.";
                return RedirectToAction("Create");
            }
            var cart = GetCart();
            if (cart.Count == 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm.";
                return RedirectToAction("Create");
            }
            var order = new Order
            {
                CustomerID = customerID,
                DeliveryProvince = deliveryProvince ?? "",
                DeliveryAddress = deliveryAddress ?? "",
                Status = OrderStatusEnum.New,
                OrderTime = DateTime.Now
            };
            var orderID = await SalesDataService.AddOrderAsync(order);
            foreach (var item in cart)
            {
                item.OrderID = orderID;
                await SalesDataService.AddDetailAsync(item);
            }
            SaveCart(new List<OrderDetail>());
            TempData["SuccessMessage"] = "Đơn hàng đã được tạo thành công.";
            return RedirectToAction("Detail", new { id = orderID });
        }

        // ===== Cart Management (Modal) =====

        /// <summary>
        /// Giao diện chỉnh sửa mặt hàng trong giỏ hàng
        /// </summary>
        public async Task<IActionResult> EditCartItem(int id, int productId)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderView))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            var detail = await SalesDataService.GetDetailAsync(id, productId);
            if (detail == null) return RedirectToAction("Detail", new { id });
            ViewBag.Order = order;
            ViewBag.Title = $"Sửa: {detail.ProductName}";
            return View(detail);
        }

        /// <summary>
        /// Xử lý cập nhật mặt hàng trong giỏ hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> EditCartItem(int id, int productId, int quantity, decimal salePrice)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderDetail))
                return RedirectToAction("AccessDenied", "Account");
            if (quantity <= 0)
                await SalesDataService.DeleteDetailAsync(id, productId);
            else
                await SalesDataService.UpdateDetailAsync(new OrderDetail { OrderID = id, ProductID = productId, Quantity = quantity, SalePrice = salePrice });
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Giao diện xác nhận xóa mặt hàng khỏi giỏ hàng
        /// </summary>
        public async Task<IActionResult> DeleteCartItem(int id, int productId)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderView))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            var detail = await SalesDataService.GetDetailAsync(id, productId);
            if (detail == null) return RedirectToAction("Detail", new { id });
            ViewBag.Order = order;
            ViewBag.Title = $"Xóa: {detail.ProductName}";
            return View(detail);
        }

        /// <summary>
        /// Xử lý xóa mặt hàng khỏi giỏ hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int id, int productId, string confirm)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderDetail))
                return RedirectToAction("AccessDenied", "Account");
            await SalesDataService.DeleteDetailAsync(id, productId);
            return RedirectToAction("Detail", new { id });
        }

        // ===== Order Status =====

        /// <summary>
        /// Giao diện xác nhận duyệt đơn hàng
        /// </summary>
        public async Task<IActionResult> Accept(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderAccept))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            ViewBag.Order = order;
            ViewBag.Title = $"Duyệt đơn hàng #{id}";
            return View(order);
        }

        /// <summary>
        /// Xử lý duyệt đơn hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Accept(int id, string confirm)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderAccept))
                return RedirectToAction("AccessDenied", "Account");
            var employeeId = HttpContext.Session.GetInt32("EmployeeID") ?? 1;
            var result = await SalesDataService.AcceptOrderAsync(id, employeeId);
            if (!result)
                TempData["ErrorMessage"] = "Không thể duyệt đơn hàng này (trạng thái không hợp lệ).";
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Giao diện xác nhận từ chối đơn hàng
        /// </summary>
        public async Task<IActionResult> Reject(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderReject))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            ViewBag.Order = order;
            ViewBag.Title = $"Từ chối đơn hàng #{id}";
            return View(order);
        }

        /// <summary>
        /// Xử lý từ chối đơn hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string confirm)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderReject))
                return RedirectToAction("AccessDenied", "Account");
            var employeeId = HttpContext.Session.GetInt32("EmployeeID") ?? 1;
            var result = await SalesDataService.RejectOrderAsync(id, employeeId);
            if (!result)
                TempData["ErrorMessage"] = "Không thể từ chối đơn hàng này (trạng thái không hợp lệ).";
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Giao diện xác nhận hủy đơn hàng
        /// </summary>
        public async Task<IActionResult> Cancel(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderCancel))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            ViewBag.Order = order;
            ViewBag.Title = $"Hủy đơn hàng #{id}";
            return View(order);
        }

        /// <summary>
        /// Xử lý hủy đơn hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Cancel(int id, string confirm)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderCancel))
                return RedirectToAction("AccessDenied", "Account");
            var result = await SalesDataService.CancelOrderAsync(id);
            if (!result)
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng này (trạng thái không hợp lệ).";
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Giao diện giao đơn hàng cho người giao hàng
        /// </summary>
        public async Task<IActionResult> Shipping(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderShip))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            var shippers = await PartnerDataService.ListShippersAsync(new PaginationSearchInput { Page = 1, PageSize = 500, SearchValue = "" });
            ViewBag.Order = order;
            ViewBag.Shippers = shippers.DataItems;
            ViewBag.Title = $"Giao đơn hàng #{id}";
            return View(order);
        }

        /// <summary>
        /// Xử lý giao đơn hàng cho người giao hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Shipping(int id, int shipperID)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderShip))
                return RedirectToAction("AccessDenied", "Account");
            if (shipperID == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn người giao hàng.";
                return RedirectToAction("Shipping", new { id });
            }
            var result = await SalesDataService.ShipOrderAsync(id, shipperID);
            if (!result)
                TempData["ErrorMessage"] = "Không thể giao đơn hàng này (trạng thái không hợp lệ).";
            return RedirectToAction("Detail", new { id });
        }

        /// <summary>
        /// Giao diện xác nhận hoàn tất đơn hàng
        /// </summary>
        public async Task<IActionResult> Finish(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderComplete))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            ViewBag.Order = order;
            ViewBag.Title = $"Hoàn tất đơn hàng #{id}";
            return View(order);
        }

        /// <summary>
        /// Xử lý hoàn tất đơn hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Finish(int id, string confirm)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderComplete))
                return RedirectToAction("AccessDenied", "Account");
            var result = await SalesDataService.CompleteOrderAsync(id);
            if (!result)
                TempData["ErrorMessage"] = "Không thể hoàn tất đơn hàng này (trạng thái không hợp lệ).";
            return RedirectToAction("Detail", new { id });
        }

        // ===== Delete =====

        /// <summary>
        /// Giao diện xác nhận xóa đơn hàng
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderDelete))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            ViewBag.Title = $"Xóa đơn hàng #{id}";
            return View(order);
        }

        /// <summary>
        /// Xử lý xóa đơn hàng
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string confirm)
        {
            if (!ApplicationContext.HasPermission(Permissions.OrderDelete))
                return RedirectToAction("AccessDenied", "Account");
            var order = await SalesDataService.GetOrderAsync(id);
            if (order == null) return RedirectToAction("Index");
            if (order.Status != OrderStatusEnum.New &&
                order.Status != OrderStatusEnum.Cancelled &&
                order.Status != OrderStatusEnum.Rejected &&
                order.Status != OrderStatusEnum.Completed)
            {
                TempData["ErrorMessage"] = "Không thể xóa đơn hàng đang xử lý.";
                return RedirectToAction("Detail", new { id });
            }
            var result = await SalesDataService.DeleteOrderAsync(id);
            if (!result)
                TempData["ErrorMessage"] = "Không thể xóa đơn hàng này.";
            return RedirectToAction("Index");
        }

        // ===== Cart Helpers =====

        private const string CART_KEY = "OrderCart";

        private List<OrderDetail> GetCart()
        {
            var data = HttpContext.Session.GetString(CART_KEY);
            if (string.IsNullOrEmpty(data))
                return new List<OrderDetail>();
            return JsonSerializer.Deserialize<List<OrderDetail>>(data) ?? new List<OrderDetail>();
        }

        private void SaveCart(List<OrderDetail> cart)
        {
            HttpContext.Session.SetString(CART_KEY, JsonSerializer.Serialize(cart));
        }
    }
}
