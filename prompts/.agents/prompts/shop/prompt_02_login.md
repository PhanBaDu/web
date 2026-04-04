# Prompt 02: Customer Login (Đăng nhập)

## 📋 Yêu cầu nghiệp vụ
Xây dựng chức năng đăng nhập an toàn cho khách hàng.

## 🔄 Flow chi tiết
1.  Truy cập `/Account/Login`.
2.  Nhập Email và Mật khẩu.
3.  Gọi `SecurityDataService.AuthorizeAccountAsync` để kiểm tra thông tin.
4.  Nếu đúng:
    -   Thiết lập Cookie Authentication (Scheme riêng cho Shop).
    -   Lưu thông tin cơ bản vào Session.
    -   Chuyển hướng về trang chủ hoặc trang trước đó (ReturnUrl).
5.  Nếu sai: Hiển thị thông báo lỗi "Email hoặc mật khẩu không chính xác" (Style UI chuẩn).
6.  Xử lý trường hợp tài khoản bị khóa (`IsLocked`).

## 🎨 UI/UX (Shadcn Style)
- Layout tối giản, hiện đại.
- Hiệu ứng highlight khi click vào input.
- Link "Quên mật khẩu" và "Đăng ký ngay" được bố trí tinh tế.

## 💻 Backend (B-layers)
- `SecurityDataService` để xác thực.
- Sử dụng `HttpContext.SignInAsync` với Cookie Scheme của Shop.
