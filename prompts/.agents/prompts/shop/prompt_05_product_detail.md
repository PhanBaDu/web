# Prompt 05: Product Detail (Chi tiết mặt hàng)

## 📋 Yêu cầu nghiệp vụ
Hiển thị đầy đủ thông tin về một sản phẩm cụ thể.

## 🔄 Flow chi tiết
1.  Truy cập `/Product/Detail/{id}`.
2.  Lấy dữ liệu từ `CatalogDataService.GetProductAsync`.
3.  Lấy danh sách ảnh bổ sung từ `CatalogDataService.ListPhotosAsync`.
4.  Lấy danh sách thuộc tính từ `CatalogDataService.ListAttributesAsync`.
5.  Tính toán giá khuyến mãi (nếu có).
6.  Hiển thị thông tin: Tên, Mô tả, Giá, Đơn vị tính, Thương hiệu (Nhà cung cấp).

## 🎨 UI/UX (Shadcn Style)
- **Image Gallery**: Carousel ảnh chính xác, mượt mà (không dùng Plugin nặng nề).
- **Product Info Area**: Text phân cấp rõ ràng, giá bán nổi bật (Font-bold).
- **Tabs**: Mô tả sản phẩm và Thông số kỹ thuật (Attributes) được chia theo Tab hiện đại.
- **CTA Sections**: Chọn số lượng và nút "Thêm vào giỏ" to, nổi bật.

## 💻 Backend (B-layers)
- `CatalogDataService` chịu trách nhiệm lấy toàn bộ Data.
- ViewModel phải tổng hợp đủ Product + Photos + Attributes.
