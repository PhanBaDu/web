# Prompt 10: Purchase History (Lịch sử mua hàng)

## 📋 Yêu cầu nghiệp vụ
Xem lại toàn bộ các đơn hàng đã thực hiện trong quá khứ.

## 🔄 Flow chi tiết
1.  Truy cập trang `/Order/History`.
2.  Lấy danh sách đơn hàng của `CustomerID` hiện tại.
3.  Phân loại theo trạng thái (Tab-based: Tất cả, Chờ duyệt, Đang giao, Hoàn thành, Hủy).
4.  Cho phép xem chi tiết nhanh một đơn hàng (Modal hoặc Accordion).
5.  Cho phép "Mua lại" (Re-order) - tự động nạp lại các item vào giỏ hàng.

## 🎨 UI/UX (Shadcn Style)
- Mỗi đơn hàng là một Card/Row độc lập.
- Hiển thị tóm tắt: Ngày đặt, Trạng thái, Tổng tiền.
- Icons minh họa cho từng trạng thái đơn hàng.

## 💻 Backend (B-layers)
- `SalesDataService.ListOrdersAsync` (Cần lọc theo CustomerID).
- Paging `PagedResult` nếu lịch sử dài.
