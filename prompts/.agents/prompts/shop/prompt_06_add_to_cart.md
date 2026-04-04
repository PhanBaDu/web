# Prompt 06: Add to Cart (Thêm vào giỏ hàng)

## 📋 Yêu cầu nghiệp vụ
Xử lý logic thêm sản phẩm vào giỏ hàng từ trang danh sách hoặc chi tiết.

## 🔄 Flow chi tiết
1.  Nút "Thêm vào giỏ" kích hoạt sự kiện `POST` (Ajax).
2.  Kiểm tra sản phẩm có tồn tại và đang bán không (`IsSelling`).
3.  Thêm mới hoặc cộng dồn số lượng vào Session `Cart`.
4.  Backend trả về số lượng giỏ hàng hiện tại (hiển thị trên Badge của Icon giỏ hàng trên Header).
5.  Hiển thị thông báo nhỏ (Toast/Notification) góc màn hình: "Đã thêm thành công sản phẩm XYZ vào giỏ".

## 🎨 UI/UX (Shadcn Style)
- Micro-animation khi click nút (Icon bay vào giỏ hoặc nút đổi trạng thái "Đã thêm").
- Toast notification có style thanh lịch (Grayish/Zinc background, lướt từ phải sang).

## 💻 Backend
- Controller: `CartController`, Action: `Add`.
- Model: `CartItem` (ProductID, ProductName, Photo, Price, Quantity).
