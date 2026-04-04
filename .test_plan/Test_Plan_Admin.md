# Kế hoạch kiểm thử hệ thống SV22T1020161.Admin

## 1. Tổng quan

**Ngày test:** 04/04/2026
**Người thực hiện:** QA Team
**Môi trường:** Development / Local
**Phạm vi:** Toàn bộ tính năng Admin Portal

---

## 2. Hệ thống phân quyền

### 2.1. Các vai trò (Roles)

| Vai trò | Mô tả | Phạm vi |
|---------|--------|---------|
| `Admin` | Toàn quyền hệ thống | Tất cả dữ liệu & chức năng |
| `Manager` | Quản lý dữ liệu nghiệp vụ | Không có nhân sự |
| `Sales` | Nhân viên bán hàng | Chỉ đơn hàng & khách hàng |
| `Inventory` | Nhân viên kho | Chỉ hàng hóa & xem đơn |

### 2.2. Danh sách Permissions

| Nhóm | Permission | Admin | Manager | Sales | Inventory |
|------|-----------|-------|---------|-------|-----------|
| Dashboard | `dashboard:view` | ✅ | ✅ | ✅ | ✅ |
| Supplier | `supplier:view` | ✅ | ✅ | - | ✅ |
| Supplier | `supplier:create` | ✅ | ✅ | - | ✅ |
| Supplier | `supplier:edit` | ✅ | ✅ | - | ✅ |
| Supplier | `supplier:delete` | ✅ | - | - | - |
| Customer | `customer:view` | ✅ | ✅ | ✅ | - |
| Customer | `customer:create` | ✅ | ✅ | ✅ | - |
| Customer | `customer:edit` | ✅ | ✅ | ✅ | - |
| Customer | `customer:delete` | ✅ | - | - | - |
| Customer | `customer:changepassword` | ✅ | - | - | - |
| Shipper | `shipper:view` | ✅ | ✅ | - | - |
| Shipper | `shipper:create` | ✅ | ✅ | - | - |
| Shipper | `shipper:edit` | ✅ | ✅ | - | - |
| Shipper | `shipper:delete` | ✅ | - | - | - |
| Employee | `employee:view` | ✅ | - | - | - |
| Employee | `employee:create` | ✅ | - | - | - |
| Employee | `employee:edit` | ✅ | - | - | - |
| Employee | `employee:delete` | ✅ | - | - | - |
| Employee | `employee:assignrole` | ✅ | - | - | - |
| Employee | `employee:changepassword` | ✅ | - | - | - |
| Category | `category:view` | ✅ | ✅ | - | ✅ |
| Category | `category:create` | ✅ | ✅ | - | ✅ |
| Category | `category:edit` | ✅ | ✅ | - | ✅ |
| Category | `category:delete` | ✅ | - | - | ✅ |
| Product | `product:view` | ✅ | ✅ | - | ✅ |
| Product | `product:create` | ✅ | ✅ | - | ✅ |
| Product | `product:edit` | ✅ | ✅ | - | ✅ |
| Product | `product:delete` | ✅ | - | - | ✅ |
| Product | `product:managephoto` | ✅ | ✅ | - | ✅ |
| Product | `product:manageattribute` | ✅ | ✅ | - | ✅ |
| Order | `order:view` | ✅ | ✅ | ✅ | ✅ |
| Order | `order:create` | ✅ | ✅ | ✅ | - |
| Order | `order:detail` | ✅ | ✅ | ✅ | ✅ |
| Order | `order:accept` | ✅ | ✅ | ✅ | - |
| Order | `order:reject` | ✅ | ✅ | ✅ | - |
| Order | `order:cancel` | ✅ | ✅ | ✅ | - |
| Order | `order:ship` | ✅ | ✅ | ✅ | - |
| Order | `order:complete` | ✅ | ✅ | ✅ | - |
| Order | `order:delete` | ✅ | - | - | - |

---

## 3. Test Cases chi tiết

### PHASE 1: Xác thực & Ủy quyền (Authentication & Authorization)

#### TC-AUTH-001: Đăng nhập thành công
- **Mục tiêu:** Kiểm tra đăng nhập với thông tin hợp lệ
- **Dữ liệu:** Email: `admin@tnp.vn`, Password: `123456` (tài khoản có Role=Admin)
- **Bước thực hiện:**
  1. Truy cập `/Account/Login`
  2. Nhập email và password hợp lệ
  3. Click "Đăng nhập"
- **Kết quả mong đợi:**
  - Redirect về `/Home/Index`
  - Cookie authentication được tạo
  - Session lưu `EmployeeID`, `EmployeeName`, `EmployeeEmail`, `EmployeeRoles`
  - Header hiển thị badge vai trò (Admin badge màu đỏ)
  - Sidebar hiển thị tất cả menu
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-002: Đăng nhập thất bại - sai password
- **Mục tiêu:** Kiểm tra thông báo lỗi khi sai password
- **Dữ liệu:** Email: `admin@tnp.vn`, Password: `sai_password`
- **Bước thực hiện:**
  1. Truy cập `/Account/Login`
  2. Nhập sai password
  3. Click "Đăng nhập"
- **Kết quả mong đợi:**
  - Vẫn ở trang login
  - Hiển thị thông báo lỗi: "Email hoặc mật khẩu không đúng"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-003: Đăng nhập thất bại - email không tồn tại
- **Mục tiêu:** Kiểm tra thông báo lỗi khi email không tồn tại
- **Dữ liệu:** Email: `khongtonaitai@tnp.vn`, Password: `123456`
- **Kết quả mong đợi:**
  - Hiển thị thông báo lỗi: "Email hoặc mật khẩu không đúng"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-004: Truy cập trang yêu cầu đăng nhập khi chưa login
- **Mục tiêu:** Kiểm tra redirect về trang login khi chưa xác thực
- **Bước thực hiện:**
  1. Truy cập `/Home/Index` khi chưa đăng nhập
- **Kết quả mong đợi:**
  - Redirect về `/Account/Login`
  - ReturnUrl được giữ để quay lại sau khi login
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-005: Đăng xuất
- **Mục tiêu:** Kiểm tra đăng xuất
- **Bước thực hiện:**
  1. Đăng nhập với tài khoản bất kỳ
  2. Click "Thoát" trong dropdown user menu
- **Kết quả mong đợi:**
  - Cookie bị xóa
  - Session được clear
  - Redirect về trang login
  - Truy cập các trang yêu cầu login sẽ redirect về login
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-006: Truy cập trang không có quyền - hiển thị AccessDenied
- **Mục tiêu:** Kiểm tra trang từ chối truy cập
- **Dữ liệu:** Đăng nhập với tài khoản có Role=Sales
- **Bước thực hiện:**
  1. Đăng nhập với tài khoản Sales
  2. Truy cập `/Employee` (trang nhân viên - Sales không có quyền)
- **Kết quả mong đợi:**
  - Redirect đến `/Account/AccessDenied`
  - Hiển thị thông báo: "Bạn không có quyền truy cập trang này"
  - Có nút "Về trang chủ" và "Đăng xuất"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-007: Sidebar ẩn menu không có quyền - Role Admin
- **Mục tiêu:** Kiểm tra sidebar với vai trò Admin
- **Dữ liệu:** Đăng nhập với tài khoản Admin
- **Kết quả mong đợi:**
  - Sidebar hiển thị: Trang chủ, Quản lý dữ liệu (4 mục), Quản lý hàng hóa (2 mục), Quản lý bán hàng (2 mục)
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-008: Sidebar ẩn menu không có quyền - Role Sales
- **Mục tiêu:** Kiểm tra sidebar với vai trò Sales
- **Dữ liệu:** Đăng nhập với tài khoản Sales
- **Kết quả mong đợi:**
  - Sidebar chỉ hiển thị: Trang chủ, Quản lý dữ liệu (chỉ Khách hàng), Quản lý bán hàng (cả 2 mục)
  - Không hiển thị: Nhà cung cấp, Người giao hàng, Nhân viên, Loại hàng, Mặt hàng
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-009: Sidebar ẩn menu không có quyền - Role Inventory
- **Mục tiêu:** Kiểm tra sidebar với vai trò Inventory
- **Dữ liệu:** Đăng nhập với tài khoản Inventory
- **Kết quả mong đợi:**
  - Sidebar hiển thị: Trang chủ, Quản lý dữ liệu (chỉ Nhà cung cấp), Quản lý hàng hóa (2 mục), Quản lý bán hàng (chỉ Quản lý đơn hàng)
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-AUTH-010: Header hiển thị badge vai trò
- **Mục tiêu:** Kiểm tra header hiển thị vai trò đúng màu
- **Bước thực hiện:** Đăng nhập với từng vai trò, kiểm tra màu badge
- **Kết quả mong đợi:**
  - Admin: badge màu đỏ (danger)
  - Manager: badge màu xanh dương (primary)
  - Sales: badge màu xanh lá (success)
  - Inventory: badge màu cam (warning)
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 2: Dashboard (Trang chủ)

#### TC-DASH-001: Dashboard hiển thị thống kê
- **Mục tiêu:** Kiểm tra trang Dashboard với Admin
- **Dữ liệu:** Đăng nhập với tài khoản Admin
- **Kết quả mong đợi:**
  - 4 thẻ thống kê: Doanh thu hôm nay, Đơn hàng chờ duyệt, Khách hàng, Sản phẩm đang bán
  - Biểu đồ doanh thu theo tháng (6 tháng gần nhất)
  - Danh sách đơn hàng mới cần xử lý (tối đa 5)
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-DASH-002: Dashboard - Sales không có quyền xem
- **Mục tiêu:** Sales không được xem Dashboard
- **Dữ liệu:** Đăng nhập với tài khoản Sales
- **Kết quả mong đợi:**
  - Truy cập `/Home/Index` → Access Denied
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 3: Quản lý Nhân viên (Employee)

#### TC-EMP-001: Danh sách nhân viên - Admin xem được
- **Mục tiêu:** Kiểm tra trang danh sách nhân viên
- **Dữ liệu:** Đăng nhập với tài khoản Admin, đã có dữ liệu nhân viên
- **Bước thực hiện:**
  1. Đăng nhập Admin
  2. Click menu "Nhân viên" trong sidebar
  3. Nhập từ khóa tìm kiếm
- **Kết quả mong đợi:**
  - Bảng danh sách nhân viên hiển thị với cột: Ảnh, Họ tên, Ngày sinh, Điện thoại, Email, Địa chỉ, Trạng thái, Thao tác
  - Có nút Thêm mới (Admin thấy)
  - Nút Sửa, Đổi mật khẩu, Phân quyền, Xóa hiển thị
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-002: Thêm mới nhân viên
- **Mục tiêu:** Kiểm tra chức năng thêm nhân viên
- **Dữ liệu:**
  - FullName: "Nguyễn Văn Test"
  - Email: "test@tnp.vn"
  - BirthDate: 01/01/2000
  - Address: "123 Test Street"
  - Phone: "0909123456"
  - RoleNames: "Manager"
- **Bước thực hiện:**
  1. Admin đăng nhập → Nhân viên → Thêm mới
  2. Điền thông tin và bấm Lưu
- **Kết quả mong đợi:**
  - Redirect về trang danh sách
  - Nhân viên mới xuất hiện trong danh sách
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-003: Thêm nhân viên - email trùng lặp
- **Mục tiêu:** Kiểm tra validation email trùng
- **Bước thực hiện:**
  1. Thêm nhân viên với email đã tồn tại
- **Kết quả mong đợi:**
  - Hiển thị lỗi: "Email đã được sử dụng bởi nhân viên khác"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-004: Sửa nhân viên
- **Mục tiêu:** Kiểm tra chức năng sửa nhân viên
- **Bước thực hiện:**
  1. Chọn 1 nhân viên → Sửa
  2. Thay đổi họ tên và lưu
- **Kết quả mong đợi:**
  - Redirect về danh sách
  - Thông tin được cập nhật
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-005: Phân quyền nhân viên
- **Mục tiêu:** Kiểm tra chức năng gán vai trò
- **Bước thực hiện:**
  1. Chọn nhân viên → Phân quyền
  2. Tick chọn vai trò "Sales"
  3. Lưu
- **Kết quả mong đợi:**
  - Redirect về danh sách
  - Nhân viên có Sales badge trên sidebar khi đăng nhập
  - Các menu bị ẩn theo quyền Sales
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-006: Đổi mật khẩu nhân viên
- **Mục tiêu:** Kiểm tra chức năng đổi mật khẩu nhân viên
- **Bước thực hiện:**
  1. Chọn nhân viên → Đổi mật khẩu
  2. Nhập password mới và xác nhận
- **Kết quả mong đợi:**
  - Thông báo thành công
  - Có thể đăng nhập với password mới
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-007: Xóa nhân viên (chưa có đơn hàng)
- **Mục tiêu:** Kiểm tra xóa nhân viên hợp lệ
- **Bước thực hiện:**
  1. Tạo nhân viên test
  2. Xóa nhân viên test đó
- **Kết quả mong đợi:**
  - Nhân viên bị xóa khỏi danh sách
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-008: Xóa nhân viên có đơn hàng - không cho phép
- **Mục tiêu:** Kiểm tra không thể xóa nhân viên có dữ liệu liên quan
- **Bước thực hiện:**
  1. Chọn nhân viên đã có đơn hàng
  2. Click xóa
- **Kết quả mong đợi:**
  - Hiển thị lỗi: "Không thể xóa nhân viên này (có thể do đang có dữ liệu liên quan)"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-009: Sales không truy cập được trang nhân viên
- **Mục tiêu:** Kiểm tra quyền truy cập nhân viên
- **Dữ liệu:** Đăng nhập với tài khoản Sales
- **Kết quả mong đợi:**
  - Truy cập `/Employee` → Access Denied
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-EMP-010: Upload ảnh nhân viên
- **Mục tiêu:** Kiểm tra upload ảnh khi thêm/sửa nhân viên
- **Bước thực hiện:**
  1. Thêm/sửa nhân viên
  2. Upload file ảnh hợp lệ (.jpg, .png)
- **Kết quả mong đợi:**
  - Ảnh được lưu vào `/wwwroot/images/employees/`
  - Hiển thị ảnh trong danh sách và chi tiết
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 4: Quản lý Nhà cung cấp (Supplier)

#### TC-SUP-001: Danh sách nhà cung cấp - phân quyền
- **Dữ liệu:** Test với Admin, Manager, Sales, Inventory
- **Kết quả mong đợi:**
  - Admin/Manager/Inventory: Thấy menu và xem được danh sách
  - Sales: Không thấy menu, truy cập trực tiếp → Access Denied
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-SUP-002: Thêm nhà cung cấp
- **Dữ liệu:** SupplierName: "Công ty Test", ContactName: "Người test", Province: "TP.HCM"
- **Kết quả mong đợi:** Nhà cung cấp mới xuất hiện trong danh sách
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-SUP-003: Sửa nhà cung cấp
- **Kết quả mong đợi:** Thông tin cập nhật thành công
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-SUP-004: Xóa nhà cung cấp - không cho xóa khi có sản phẩm
- **Kết quả mong đợi:** Thông báo lỗi nếu có sản phẩm liên quan
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-SUP-005: Nút xóa ẩn với Manager
- **Dữ liệu:** Đăng nhập với tài khoản Manager
- **Kết quả mong đợi:** Không có nút Xóa trên bảng nhà cung cấp
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 5: Quản lý Khách hàng (Customer)

#### TC-CUS-001: Danh sách khách hàng - phân quyền
- **Kết quả mong đợi:**
  - Admin/Manager/Sales: Thấy menu và xem được
  - Inventory: Không thấy menu
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-CUS-002: Thêm khách hàng
- **Dữ liệu:** CustomerName: "Khách Test", ContactName: "Người liên hệ", Email: "test@customer.vn", Province: "Hà Nội"
- **Kết quả mong đợi:** Khách hàng mới tạo thành công
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-CUS-003: Thêm khách hàng - email trùng
- **Kết quả mong đợi:** Báo lỗi validation email trùng
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-CUS-004: Đổi mật khẩu khách hàng - Admin thấy, Manager/Sales không thấy
- **Dữ liệu:** Test với từng vai trò
- **Kết quả mong đợi:**
  - Admin: Thấy nút Đổi mật khẩu
  - Manager/Sales: Không thấy nút
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 6: Quản lý Người giao hàng (Shipper)

#### TC-SHIP-001: Danh sách shipper - phân quyền
- **Kết quả mong đợi:**
  - Admin/Manager: Thấy menu và xem được
  - Sales/Inventory: Không thấy menu
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-SHIP-002: Thêm người giao hàng - validation số điện thoại
- **Dữ liệu:** ShipperName: "Người giao Test", Phone: "12345" (không hợp lệ)
- **Kết quả mong đợi:** Báo lỗi: "Số điện thoại phải là chữ số, từ 9 đến 15 ký tự"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 7: Quản lý Loại hàng (Category)

#### TC-CAT-001: Danh sách loại hàng - phân quyền
- **Kết quả mong đợi:**
  - Admin/Manager/Inventory: Thấy menu và xem được
  - Sales: Không thấy menu
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-CAT-002: Thêm loại hàng
- **Dữ liệu:** CategoryName: "Test Category", Description: "Mô tả test"
- **Kết quả mong đợi:** Loại hàng mới xuất hiện
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-CAT-003: Xóa loại hàng có sản phẩm - không cho xóa
- **Kết quả mong đợi:** Thông báo lỗi khi còn sản phẩm thuộc loại này
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 8: Quản lý Mặt hàng (Product)

#### TC-PRO-001: Danh sách sản phẩm - phân quyền
- **Kết quả mong đợi:**
  - Admin/Manager/Inventory: Thấy menu và xem được
  - Sales: Không thấy menu
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PRO-002: Tìm kiếm sản phẩm theo loại hàng và nhà cung cấp
- **Bước thực hiện:**
  1. Chọn loại hàng từ dropdown
  2. Chọn nhà cung cấp
  3. Nhấn tìm kiếm
- **Kết quả mong đợi:** Danh sách lọc đúng theo điều kiện
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PRO-003: Thêm sản phẩm
- **Dữ liệu:**
  - ProductName: "Sản phẩm Test"
  - Category: chọn 1 loại
  - Supplier: chọn 1 nhà cung cấp
  - Unit: "Cái"
  - Price: 50000
- **Kết quả mong đợi:** Sản phẩm mới tạo thành công
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PRO-004: Quản lý ảnh sản phẩm
- **Bước thực hiện:**
  1. Chọn sản phẩm → Thêm ảnh
  2. Upload file ảnh
- **Kết quả mong đợi:**
  - Ảnh hiển thị trong danh sách ảnh sản phẩm
  - Có thể sửa đổi thứ tự hiển thị
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PRO-005: Quản lý thuộc tính sản phẩm
- **Bước thực hiện:**
  1. Chọn sản phẩm → Thêm thuộc tính
  2. Nhập AttributeName: "Màu sắc", AttributeValue: "Đỏ"
- **Kết quả mong đợi:** Thuộc tính được lưu và hiển thị
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PRO-006: Nút xóa ẩn với Manager/Inventory
- **Dữ liệu:** Đăng nhập Manager hoặc Inventory
- **Kết quả mong đợi:** Không thấy nút Xóa sản phẩm
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 9: Quản lý Đơn hàng (Order)

#### TC-ORD-001: Danh sách đơn hàng - phân quyền xem
- **Kết quả mong đợi:**
  - Tất cả vai trò: Thấy menu "Quản lý đơn hàng"
  - Inventory: Chỉ thấy "Quản lý đơn hàng", không thấy "Lập đơn hàng"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-002: Lập đơn hàng mới - Sales có quyền
- **Dữ liệu:** Đăng nhập với tài khoản Sales
- **Kết quả mong đợi:** Thấy menu "Lập đơn hàng", truy cập được `/Order/Create`
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-003: Lập đơn hàng - thêm sản phẩm vào giỏ
- **Bước thực hiện:**
  1. Chọn khách hàng
  2. Thêm sản phẩm vào giỏ
  3. Điền địa chỉ giao hàng
  4. Lưu đơn hàng
- **Kết quả mong đợi:**
  - Đơn hàng được tạo với trạng thái "Đơn mới - Đang chờ duyệt"
  - Redirect đến trang chi tiết đơn hàng
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-004: Lập đơn hàng - chưa chọn khách hàng
- **Kết quả mong đợi:** Báo lỗi "Vui lòng chọn khách hàng"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-005: Lập đơn hàng - giỏ hàng trống
- **Kết quả mong đợi:** Báo lỗi "Giỏ hàng trống. Vui lòng thêm sản phẩm"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-006: Duyệt đơn hàng - Admin/Manager/Sales
- **Bước thực hiện:**
  1. Chọn đơn hàng có trạng thái "Mới - Đang chờ duyệt"
  2. Click Duyệt đơn hàng
- **Kết quả mong đợi:**
  - Đơn hàng chuyển sang trạng thái "Đã duyệt"
  - Cập nhật thời gian AcceptTime
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-007: Duyệt đơn hàng - Inventory không có quyền
- **Dữ liệu:** Đăng nhập Inventory, đơn hàng mới
- **Kết quả mong đợi:** Không thấy nút Duyệt trong menu xử lý
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-008: Từ chối đơn hàng
- **Bước thực hiện:**
  1. Chọn đơn hàng mới
  2. Click Từ chối đơn hàng
- **Kết quả mong đợi:**
  - Đơn hàng chuyển sang trạng thái "Bị từ chối"
  - Không hiển thị nút xử lý
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-009: Hủy đơn hàng
- **Bước thực hiện:**
  1. Chọn đơn hàng đã duyệt
  2. Click Hủy đơn hàng
- **Kết quả mong đợi:** Đơn hàng chuyển trạng thái "Đã hủy"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-010: Giao đơn hàng cho shipper
- **Bước thực hiện:**
  1. Đơn hàng đã duyệt
  2. Click Chuyển giao hàng
  3. Chọn người giao hàng
  4. Lưu
- **Kết quả mong đợi:**
  - Đơn hàng chuyển sang trạng thái "Đang giao"
  - Thông tin người giao hàng được cập nhật
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-011: Hoàn tất đơn hàng
- **Bước thực hiện:**
  1. Đơn hàng đang giao
  2. Click Hoàn tất đơn hàng
- **Kết quả mong đợi:**
  - Đơn hàng chuyển sang trạng thái "Hoàn tất"
  - Cập nhật FinishedTime
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-012: Xóa đơn hàng - Admin only
- **Dữ liệu:** Đơn hàng ở trạng thái Mới/Hoàn tất/Từ chối/Đã hủy
- **Kết quả mong đợi:**
  - Admin: Thấy nút Xóa
  - Manager/Sales/Inventory: Không thấy nút Xóa
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-ORD-013: Sửa/xóa mặt hàng trong đơn - phân quyền
- **Dữ liệu:** Đăng nhập Sales
- **Kết quả mong đợi:** Không thấy nút Sửa/Xóa mặt hàng trong chi tiết đơn hàng
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

### PHASE 10: Thông tin cá nhân & Đổi mật khẩu

#### TC-PROFILE-001: Xem thông tin cá nhân
- **Bước thực hiện:** Đăng nhập → User menu → Profile
- **Kết quả mong đợi:** Hiển thị thông tin nhân viên đang đăng nhập
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PROFILE-002: Cập nhật thông tin cá nhân
- **Bước thực hiện:** Sửa họ tên và lưu
- **Kết quả mong đợi:**
  - Thông tin cập nhật
  - Header hiển thị tên mới
  - Cookie claims được cập nhật
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PROFILE-003: Đổi mật khẩu cá nhân - thành công
- **Bước thực hiện:**
  1. Đổi mật khẩu: oldPassword đúng, newPassword >= 6 ký tự
  2. Lưu
- **Kết quả mong đợi:**
  - Thông báo thành công
  - Có thể đăng nhập với mật khẩu mới
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PROFILE-004: Đổi mật khẩu cá nhân - sai mật khẩu cũ
- **Kết quả mong đợi:** Báo lỗi "Mật khẩu cũ không đúng"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

#### TC-PROFILE-005: Đổi mật khẩu cá nhân - mật khẩu mới quá ngắn
- **Kết quả mong đợi:** Báo lỗi "Mật khẩu mới phải có ít nhất 6 ký tự"
- **Trạng thái:** [ ] PASSED [ ] FAILED [ ] BLOCKED

---

## 4. Bảng tổng hợp kết quả

| Phase | Tổng TC | PASSED | FAILED | BLOCKED |
|-------|---------|--------|--------|---------|
| 1. Authentication & Authorization | 10 | 0 | 0 | 0 |
| 2. Dashboard | 2 | 0 | 0 | 0 |
| 3. Employee | 10 | 0 | 0 | 0 |
| 4. Supplier | 5 | 0 | 0 | 0 |
| 5. Customer | 4 | 0 | 0 | 0 |
| 6. Shipper | 2 | 0 | 0 | 0 |
| 7. Category | 3 | 0 | 0 | 0 |
| 8. Product | 6 | 0 | 0 | 0 |
| 9. Order | 13 | 0 | 0 | 0 |
| 10. Profile & Password | 5 | 0 | 0 | 0 |
| **TỔNG CỘNG** | **60** | **0** | **0** | **0** |

---

## 5. Tài khoản test

Trước khi test, cần tạo các tài khoản với vai trò khác nhau trong bảng `Employees`:

```sql
-- Admin account
INSERT INTO Employees (FullName, Email, Password, RoleNames, IsWorking)
VALUES ('Admin Test', 'admin@tnp.vn', 'MD5_hash', 'Admin', 1);

-- Manager account
INSERT INTO Employees (FullName, Email, Password, RoleNames, IsWorking)
VALUES ('Manager Test', 'manager@tnp.vn', 'MD5_hash', 'Manager', 1);

-- Sales account
INSERT INTO Employees (FullName, Email, Password, RoleNames, IsWorking)
VALUES ('Sales Test', 'sales@tnp.vn', 'MD5_hash', 'Sales', 1);

-- Inventory account
INSERT INTO Employees (FullName, Email, Password, RoleNames, IsWorking)
VALUES ('Inventory Test', 'inventory@tnp.vn', 'MD5_hash', 'Inventory', 1);
```

> **Lưu ý:** Password cần hash MD5 trước khi lưu vào database.

---

## 6. Bug Report Template

| Trường | Mô tả |
|--------|--------|
| Bug ID | TC-XXX |
| Tiêu đề | Mô tả ngắn gọn bug |
| Mức độ | Critical / High / Medium / Low |
| Môi trường | Browser, OS |
| Steps to Reproduce | Các bước tái hiện bug |
| Expected Result | Kết quả mong đợi |
| Actual Result | Kết quả thực tế |
| Evidence | Ảnh chụp màn hình |
| Assigned To | Người phụ trách fix |
| Status | Open / In Progress / Fixed / Closed |
