using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SV22T1020161.BusinessLayers;
using SV22T1020161.Models.Partner;
using SV22T1020161.Models.Security;
using SV22T1020161.Shop;
using SV22T1020161.Shop.Models;

namespace SV22T1020161.Shop.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Customer data, string confirmPassword, string returnUrl = "")
        {
            // TC-R3: Validate truong bat buoc
            if (string.IsNullOrWhiteSpace(data.CustomerName))
                ModelState.AddModelError(nameof(data.CustomerName), "Họ tên không được để trống.");
            else if (data.CustomerName.Trim().Length < 2)
                ModelState.AddModelError(nameof(data.CustomerName), "Họ tên phải có ít nhất 2 ký tự.");

            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập Email.");
            else if (!data.Email.Contains('@') || !data.Email.Contains('.'))
                ModelState.AddModelError(nameof(data.Email), "Email không đúng định dạng.");

            if (string.IsNullOrWhiteSpace(data.Phone))
                ModelState.AddModelError(nameof(data.Phone), "Số điện thoại không được để trống.");
            else
            {
                string digits = new string(data.Phone.Where(char.IsDigit).ToArray());
                if (digits.Length < 10 || digits.Length > 11)
                    ModelState.AddModelError(nameof(data.Phone), "Số điện thoại phải từ 10-11 chữ số.");
            }

            // TC-R3: Tinh/Thanh pho bat buoc
            if (string.IsNullOrWhiteSpace(data.Province))
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành phố.");

            if (string.IsNullOrWhiteSpace(data.Password))
                ModelState.AddModelError(nameof(data.Password), "Vui lòng nhập mật khẩu.");
            else
            {
                // TC-R1: Password strength - 1 chu hoa, 1 so, 1 ky tu dac biet, toi thieu 6 ky tu
                var pw = data.Password;
                bool hasUpper = pw.Any(char.IsUpper);
                bool hasDigit = pw.Any(char.IsDigit);
                bool hasSpecial = pw.Any(ch => !char.IsLetterOrDigit(ch));

                if (pw.Length < 6)
                    ModelState.AddModelError(nameof(data.Password), "Mật khẩu phải có ít nhất 6 ký tự.");
                else if (!hasUpper)
                    ModelState.AddModelError(nameof(data.Password), "Mật khẩu phải chứa ít nhất 1 chữ HOA.");
                else if (!hasDigit)
                    ModelState.AddModelError(nameof(data.Password), "Mật khẩu phải chứa ít nhất 1 chữ số.");
                else if (!hasSpecial)
                    ModelState.AddModelError(nameof(data.Password), "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt (!@#$...).");
            }

            if (data.Password != confirmPassword)
                ModelState.AddModelError(string.Empty, "Mật khẩu xác nhận không khớp.");

            // TC-R2: Email trung
            if (!string.IsNullOrWhiteSpace(data.Email))
            {
                bool isEmailValid = await PartnerDataService.ValidateCustomerEmailAsync(data.Email);
                if (!isEmailValid)
                    ModelState.AddModelError(nameof(data.Email), "Email này đã được sử dụng. Vui lòng dùng email khác.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(data);
            }

            try
            {
                data.ContactName = data.CustomerName;
                data.IsLocked = false;
                // Hash MD5 trước khi gửi xuống repository
                data.Password = CryptHelper.MD5Hash(data.Password ?? "");
                await PartnerDataService.AddCustomerAsync(data);

                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return RedirectToAction("Login", new { returnUrl });
                return RedirectToAction("Login");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi hệ thống, vui lòng thử lại sau.");
                ViewBag.ReturnUrl = returnUrl;
                return View(data);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe = false, string returnUrl = "")
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ email và mật khẩu.");
                return View();
            }

            // TC-L2: Sai email hoac mat khau — mat khau phai hash MD5 giong luc dang ky (CustomerAccountRepository so sanh voi Password da luu)
            var account = await SecurityDataService.AuthorizeAsync(email, CryptHelper.MD5Hash(password), UserTypes.Customer);
            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                ViewData["Email"] = email;
                return View();
            }

            // TC-L3: Tai khoan bi khoa
            var customer = await PartnerDataService.GetCustomerAsync(int.Parse(account.UserID));
            if (customer != null && customer.IsLocked == true)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ bộ phận hỗ trợ.");
                ViewData["Email"] = email;
                return View();
            }

            // TC-L4: Remember me
            var userData = new WebUserData
            {
                UserId = account.UserID,
                UserName = account.Email,
                DisplayName = account.FullName,
                Photo = account.Photo,
                Roles = new List<string> { "customer" }
            };

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(userData.CreatePrincipal(), authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Login");

            var customer = await PartnerDataService.GetCustomerAsync(userId);
            if (customer == null)
                return RedirectToAction("Login");

            return View(customer);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Customer model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Login");

            var customer = await PartnerDataService.GetCustomerAsync(userId);
            if (customer == null)
                return RedirectToAction("Login");

            // Validate
            if (string.IsNullOrWhiteSpace(model.CustomerName))
            {
                TempData["ErrorMessage"] = "Họ tên không được để trống.";
                return RedirectToAction("Profile");
            }

            if (!string.IsNullOrWhiteSpace(model.Phone))
            {
                string digits = new string(model.Phone.Where(char.IsDigit).ToArray());
                if (digits.Length < 10 || digits.Length > 11)
                {
                    TempData["ErrorMessage"] = "Số điện thoại phải từ 10-11 chữ số.";
                    return RedirectToAction("Profile");
                }
            }

            customer.CustomerName = model.CustomerName.Trim();
            customer.ContactName = model.CustomerName.Trim();
            customer.Phone = model.Phone;
            customer.Province = model.Province;
            customer.Address = model.Address;

            bool ok = await PartnerDataService.UpdateCustomerAsync(customer);
            if (ok)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";

                var userData = new WebUserData
                {
                    UserId = customer.CustomerID.ToString(),
                    UserName = customer.Email,
                    DisplayName = customer.CustomerName,
                    Photo = User.FindFirst("Photo")?.Value ?? "",
                    Roles = new List<string> { "customer" }
                };
                await HttpContext.SignInAsync(userData.CreatePrincipal());
            }
            else
            {
                TempData["ErrorMessage"] = "Cập nhật thông tin thất bại. Vui lòng thử lại.";
            }

            return RedirectToAction("Profile");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ tất cả các trường.";
                return RedirectToAction("Profile");
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp.";
                return RedirectToAction("Profile");
            }

            var pw = newPassword;
            bool hasUpper = pw.Any(char.IsUpper);
            bool hasDigit = pw.Any(char.IsDigit);
            bool hasSpecial = pw.Any(ch => !char.IsLetterOrDigit(ch));

            if (pw.Length < 6)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return RedirectToAction("Profile");
            }
            if (!hasUpper || !hasDigit || !hasSpecial)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải chứa chữ HOA, chữ số và ký tự đặc biệt.";
                return RedirectToAction("Profile");
            }

            var email = User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login");

            // Kiem tra mat khau cu (so sanh voi hash trong DB)
            var account = await SecurityDataService.AuthorizeAsync(email, CryptHelper.MD5Hash(oldPassword), UserTypes.Customer);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Mật khẩu cũ không chính xác.";
                return RedirectToAction("Profile");
            }

            bool result = await SecurityDataService.ChangePasswordAsync(email, CryptHelper.MD5Hash(newPassword), UserTypes.Customer);
            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Đổi mật khẩu thành công!" : "Đã có lỗi xảy ra khi đổi mật khẩu.";

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
