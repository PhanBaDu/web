# Prompt 09: Order Tracking (Theo dõi đơn hàng)

## 📋 Yêu cầu nghiệp vụ
Khách hàng có thể theo dõi tiến độ xử lý của đơn hàng họ vừa đặt.

## 🔄 Flow chi tiết
1.  Truy cập trang `/Order/Status/{orderID}`.
2.  Kiểm tra quyền truy cập (Người dùng chỉ xem được đơn hàng của chính mình).
3.  Hiển thị trạng thái hiện tại (Lấy từ bảng `OrderStatus` thông qua `SalesDataService`).
4.  Hiển thị dòng thời gian (Timeline) các mốc: Đã đặt, Đã duyệt, Đang giao, Đã hoàn thành, Đã hủy.
5.  Hiển thị thông tin người giao hàng (`Shipper`) nếu đã phân công.

## 🎨 UI/UX (Shadcn Style)
- **Timeline Component**: Thiết kế dọc (Vertical timeline) với các icon trạng thái hiện đại.
- **Badge Status**: Phân màu theo trạng thái (Ví dụ: Blue cho "Đã duyệt", Green cho "Đã giao", Red cho "Đã hủy").
- Hiển thị đầy đủ thông tin Tổng tiền và Địa chỉ nhận hàng ngay bên cạnh timeline.

## 💻 Backend (B-layers)
- `SalesDataService.GetOrderAsync`.
- Phải parse `Status` (int) sang text mô tả dựa trên bảng `OrderStatus`.
