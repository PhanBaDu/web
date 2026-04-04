# Master Style Prompt: Shadcn/UI for ASP.NET MVC

## 🎯 Objective
Thiết kế giao diện cho project `SV22T1020161.Shop` theo phong cách **shadcn/ui** (Minimalist, Modern, High-accessibility, Clean).

## 🎨 Design Principles
- **Typography**: Sử dụng font hiện đại (Inter hoặc Sans-serif chuẩn), kích thước chữ tối ưu (hệ thống 0.25rem).
- **Colors**: Bảng màu trung tính (slate/zinc) kết hợp với các màu nhấn (primary) tinh tế. Không dùng màu rực rỡ quá mức.
- **Components**: 
  - **Borders**: Bo góc chuẩn (`rounded-md`, `rounded-lg`).
  - **Shadows**: Đổ bóng nhẹ nhàng để phân lớp (elevation).
  - **Spacing**: Khoảng trắng (whitespace) rộng rãi để người dùng không bị ngợp.
- **Interactivity**: Hiệu ứng hover, focus mượt mà.

## 🛠 Tech Stack for UI
- **Tailwind CSS**: Dùng bộ CDN hoặc setup build để quản lý style (Utility-first).
- **Lucide Icons**: Dùng các icon mảnh, hiện đại thay cho FontAwesome hay Bootstrap Icons cũ.
- **Partial Views as Components**: Mỗi component UI (Button, Input, Card, Modal) nên được tách thành các Partial View để tái sử dụng, mô phỏng đúng cách shadcn hoạt động.

## 🏗 Coding Standard
- **Clean HTML**: HTML phải semantic, gọn gàng.
- **Validation UI**: Error messages hiển thị ngay dưới input, style đỏ nhạt tinh tế (slate-900 border-red-500).
- **Loading States**: Phải có skeleton hoặc spinner khi gọi API/Database.

**Ghi nhớ**: Tuyệt đối không để lại bất kỳ dấu vết nào của AdminLTE. Giao diện phải mang lại cảm giác cao cấp của các trang thương mại điện tử hiện đại.
