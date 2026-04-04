# Prompt 01: Customer Registration (Đăng ký tài khoản)

## 📋 Yêu cầu nghiệp vụ
Xây dựng tính năng cho phép khách hàng mới đăng ký tài khoản thành viên.

## 🔄 Flow chi tiết
1.  Người dùng truy cập `/Account/Register`.
2.  Điền thông tin: Họ tên, Email, Số điện thoại, Địa chỉ, Tỉnh/thành, Mật khẩu, Xác nhận mật khẩu.
3.  **Validate (Client-side & Server-side)**:
    -   Email phải đúng định dạng và chưa tồn tại trong DB (gọi `PartnerDataService.ValidatelCustomerEmailAsync`).
    -   Mật khẩu phải đủ mạnh (phải có ký tự đặc biệt, số).
    -   Xác nhận mật khẩu phải khớp.
4.  Lưu thông tin qua `PartnerDataService.AddCustomerAsync`.
5.  Thông báo đăng ký thành công (Toast message mượt mà) và chuyển hướng về trang Đăng nhập.

## 🎨 UI/UX (Shadcn Style)
- Form đăng ký nằm giữa màn hình (Card-based).
- Các bước điền thông tin rõ ràng hoặc grouped hợp lý.
- Hiển thị validation lỗi ngay dưới mỗi field (inline error).
- Nút "Đăng ký" có trạng thái loading khi đang xử lý.

## 💻 Backend (B-layers)
- Controller: `AccountController`, Action: `Register` (GET/POST).
- Model: `SV22T1020161.Models.Partner.Customer`.
- Dùng `PartnerDataService` để tương tác DB.
