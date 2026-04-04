# Prompt 03: Profile Management (Thông tin cá nhân & Mật khẩu)

## 📋 Yêu cầu nghiệp vụ
Khách hàng có thể xem và cập nhật thông tin cá nhân hoặc thay đổi mật khẩu của mình.

## 🔄 Flow chi tiết
1.  Truy cập `/Account/Profile`.
2.  **Thông tin cá nhân**:
    -   Hiển thị thông tin hiện tại (từ `AccountController/Profile` gọi `PartnerDataService.GetCustomerAsync`).
    -   Cho phép sửa: Họ tên, Điện thoại, Địa chỉ, Tỉnh/thành.
    -   Xử lý cập nhật qua `PartnerDataService.UpdateCustomerAsync`.
3.  **Thay đổi mật khẩu**:
    -   Người dùng nhập: Mật khẩu cũ, Mật khẩu mới, Xác nhận mật khẩu mới.
    -   Kiểm tra mật khẩu cũ khớp với DB (gọi `SecurityDataService.ChangePasswordAsync` hoặc logic tương đương).
    -   Cập nhật mật khẩu mới.
4.  Lưu log thay đổi.
5.  Thông báo kết quả (Success/Error).

## 🎨 UI/UX (Shadcn Style)
- Sử dụng Sidebar-layout cho trang Dashboard cá nhân.
- Các input dạng Floating Labels hoặc Label rõ ràng bên trên.
- Button "Lưu thay đổi" có màu sắc hài hòa (Primary).
- Mật khẩu cũ/mới nên có icon 👁️ (Show/Hide).

## 💻 Backend (B-layers)
- Sử dụng `PartnerDataService` và `SecurityDataService`.
- Tận dụng `SV22T1020161.Models.Partner.Customer`.
