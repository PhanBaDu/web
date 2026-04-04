# Prompt 04: Product Catalog (Tra cứu mặt hàng)

## 📋 Yêu cầu nghiệp vụ
Cho phép khách hàng tìm kiếm và xem danh sách sản phẩm.

## 🔄 Flow chi tiết
1.  **Trang chủ/Danh sách**: Hiển thị lưới sản phẩm (Product Grid).
2.  **Bộ lọc (Filter Bar)**:
    -   Tìm kiếm theo tên (SearchValue).
    -   Lọc theo Loại hàng (Category) - dùng dropdown hoặc sidebar.
    -   Lọc theo khoảng giá (MinPrice, MaxPrice) - dùng input hoặc slider.
3.  **Phân trang**: Dùng `PagedResult` để phân trang mượt mà (không load lại trang nếu dùng Ajax).
4.  Dữ liệu lấy từ `CatalogDataService.ListProductsAsync`.

## 🎨 UI/UX (Shadcn Style)
- **Product Card**: Ảnh sản phẩm sắc nét, tên, giá bán (format theo `vi-VN`), nút "Thêm vào giỏ" nhanh.
- **Sidebar/Filter**: Thiết kế dạng "Sheet" (Mobile) hoặc Sidebar tinh tế (Desktop).
- **Skeleton Scrolling**: Hiển thị khung xám khi đang load dữ liệu.

## 💻 Backend (B-layers)
- Controller: `ProductController`.
- Action: `Index` và `Search` (PartialView).
- Query parameters phải được mapping vào `ProductSearchInput`.
