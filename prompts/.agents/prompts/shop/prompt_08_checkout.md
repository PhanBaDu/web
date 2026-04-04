# Prompt 08: Order Checkout (Đặt mua hàng)

## 📋 Yêu cầu nghiệp vụ
Xác nhận thông tin đơn hàng và tiến hành thanh toán (Thanh toán khi nhận hàng).

## 🔄 Flow chi tiết
1.  Người dùng nhấn "Đặt hàng" từ trang Giỏ hàng.
2.  Kiểm tra đăng nhập (Yêu cầu login nếu chưa).
3.  Hiển thị trang checkout:
    -   Địa chỉ nhận hàng (Lấy mặc định từ thông tin tài khoản, cho phép sửa).
    -   Chọn tỉnh/thành.
    -   Ghi chú đơn hàng.
    -   Phương thức vận chuyển (Lấy từ `PartnerDataService.ListShippersAsync`).
4.  **Confirm Order**:
    -   Tạo mới `Order` trong DB qua `SalesDataService.SaveOrderAsync`.
    -   Thêm chi tiết đơn hàng (`OrderDetails`).
    -   Làm trống giỏ hàng sau khi thành công.
5.  Hiển thị trang "Cảm ơn" (Order Success) với mã đơn hàng.

## 🎨 UI/UX (Shadcn Style)
- Layout 2 cột: Cột trái nhập địa chỉ, Cột phải tóm tắt giỏ hàng (Check-out summary list).
- Sử dụng Form-sections rõ ràng, phân cấp bằng tiêu đề đậm.

## 💻 Backend (B-layers)
- `SalesDataService` để lưu đơn hàng.
- `SalesDataService.SaveOrderAsync` (hoặc method tương đương trong SalesDataService).
