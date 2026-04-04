# Prompt 07: Cart Management (Quản lý giỏ hàng)

## 📋 Yêu cầu nghiệp vụ
Quản lý các mặt hàng đã chọn mua.

## 🔄 Flow chi tiết
1.  Truy cập `/Cart`.
2.  Hiển thị danh sách Item trong giỏ (từ Session).
3.  **Update Quantity**: 
    -   Cập nhật số lượng trực tiếp (Ajax).
    -   Kiểm tra số lượng phải > 0.
4.  **Remove Item**: Xóa sản phẩm khỏi giỏ.
5.  **Summary Area**:
    -   Hiển thị tổng số lượng.
    -   Tính tổng tiền bảo gồm thuế/ship (nếu có).
6.  Nút "Tiến hành đặt hàng" sẽ kiểm tra đăng nhập trước khi checkout.

## 🎨 UI/UX (Shadcn Style)
- List items dạng Row sạch sẽ.
- Nút tăng giảm số lượng (+/-) nhỏ gọn, responsive.
- Empty State: Hiển thị hình minh họa cực đẹp khi giỏ hàng trống.

## 💻 Backend
- Dùng Utility class để xử lý `CartItem` lưu trong Session.
- Mọi logic tính toán tiền phải nằm trong Service hoặc Helper chuyên biệt.
