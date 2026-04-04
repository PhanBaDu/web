---
description: Quy trình phát triển tính năng User cho phần Shop (E-commerce) sử dụng BusinessLayers hiện có.
---

# Quy trình phát triển Shop (User Features)

Khi phát triển các tính năng dành cho khách hàng (User) trong project `SV22T1020161.Shop`, bạn phải tuân thủ các nguyên tắc sau để đảm bảo tính đồng nhất với phần Admin đã được dạy:

## 1. Tận dụng BusinessLayers (B-layers)
- **KHÔNG** viết truy vấn SQL trực tiếp trong Controller hay View của Shop.
- **PHẢI** sử dụng các Service đã có trong `SV22T1020161.BusinessLayers` (ví dụ: `CatalogDataService`, `SalesDataService`).
- Nếu B-layers hiện tại thiếu chức năng cần thiết cho Shop, hãy bổ sung vào `BusinessLayers` và `DataLayers` theo đúng pattern hiện tại (Dapper Repository -> Static Service) trước khi gọi từ Shop.

## 🎯 Các tính năng bắt buộc cho Khách hàng (Customer)
Agent phải xây dựng đầy đủ 10 chức năng sau trong project `SV22T1020161.Shop`:
1.  **Đăng ký tài khoản mới**: Cho phép khách hàng tạo tài khoản.
2.  **Đăng nhập**: Hệ thống xác thực khách hàng.
3.  **Quản lý cá nhân**: Xem/Sửa thông tin cá nhân và đổi mật khẩu.
4.  **Tra cứu hàng hóa**: Xem, tìm kiếm theo loại hàng, tên hàng, khoảng giá.
5.  **Chi tiết mặt hàng**: Xem thông tin chi tiết, thuộc tính, ảnh của sản phẩm.
6.  **Thêm vào giỏ**: Đưa mặt hàng vào giỏ hàng tạm thời.
7.  **Quản lý giỏ hàng**: Xem giỏ, thay đổi số lượng, xóa mặt hàng khỏi giỏ.
8.  **Đặt hàng**: Tiến hành thanh toán và tạo đơn hàng mới.
9.  **Theo dõi trạng thái**: Xem tiến độ xử lý đơn hàng (đang xử lý, giao hàng, v.v.).
10. **Lịch sử mua hàng**: Xem danh sách các đơn hàng cũ của cá nhân.

## ⚠️ Ràng buộc quan trọng (Mandatory Constraints)
- **KHÔNG sử dụng AdminLTE theme**: Phần Shop dành cho khách hàng nên cần giao diện e-commerce hiện đại, mượt mà (dùng Bootstrap 5 thuần hoặc custom CSS). Tuyệt đối không bê nguyên giao diện quản trị AdminLTE sang.
- **B-layers Integrity**: Mọi thao tác dữ liệu vẫn phải thông qua `BusinessLayers`.
- **Session/Cookie**: Sử dụng Session để quản lý giỏ hàng và Authentication Cookie riêng biệt cho phần Shop để không bị lẫn với Admin.

## 📁 File Structure cho Shop
- `Controllers/AccountController`: Đăng ký, đăng nhập.
- `Controllers/ProductController`: Xem hàng, chi tiết hàng.
- `Controllers/CartController`: Xử lý giỏ hàng.
- `Controllers/OrderController`: Thanh toán, lịch sử đơn hàng.
