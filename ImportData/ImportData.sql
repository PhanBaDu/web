-- ============================================
-- SCRIPT IMPORT DATA - LITE COMMERCE
-- Database: LiteCommerceDB
-- ============================================

USE LiteCommerceDB;
GO

-- Tắt constraints tạm thời
EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all';
GO

-- Xóa dữ liệu cũ
DELETE FROM OrderDetails;
DELETE FROM Orders;
DELETE FROM ProductPhotos;
DELETE FROM ProductAttributes;
DELETE FROM Products;
DELETE FROM Categories;
DELETE FROM Suppliers;
DELETE FROM Shippers;
DELETE FROM Employees;
DELETE FROM Customers;
GO

-- Reset Identity
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Customers', RESEED, 0);
DBCC CHECKIDENT ('Suppliers', RESEED, 0);
DBCC CHECKIDENT ('Shippers', RESEED, 0);
DBCC CHECKIDENT ('Employees', RESEED, 0);
GO

-- Bật lại constraints
EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT all';
GO

-- ============================================
-- 1. INSERT CATEGORIES (4 categories)
-- ============================================
SET IDENTITY_INSERT Categories ON;

INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (1, N'Bàn phím', N'Bàn phím máy tính - Bàn phím cơ, bàn phím không dây, bàn phím gaming');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (2, N'Điện thoại', N'Điện thoại smartphone - iPhone, Samsung, các hãng Android');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (3, N'Laptop', N'Máy tính xách tay - MacBook, Laptop gaming, Laptop văn phòng');
INSERT INTO Categories (CategoryID, CategoryName, Description) VALUES (4, N'Màn hình', N'Màn hình máy tính - Màn hình gaming, màn hình văn phòng, màn hình 4K');

SET IDENTITY_INSERT Categories OFF;
GO

-- ============================================
-- 2. INSERT SUPPLIERS (4 suppliers)
-- ============================================
SET IDENTITY_INSERT Suppliers ON;

INSERT INTO Suppliers (SupplierID, SupplierName, ContactName, Province, Address, Phone, Email)
VALUES (1, N'Apple Việt Nam', N'Nguyễn Văn Minh', N'Hồ Chí Minh', N'123 Nguyễn Huệ, Quận 1', N'0901234567', N'contact@apple-vietnam.com');

INSERT INTO Suppliers (SupplierID, SupplierName, ContactName, Province, Address, Phone, Email)
VALUES (2, N'Samsung Electronics Vietnam', N'Trần Thị Lan', N'Thành phố Hồ Chí Minh', N'456 Nguyễn Trãi, Quận 5', N'0902345678', N'contact@samsung-vn.com');

INSERT INTO Suppliers (SupplierID, SupplierName, ContactName, Province, Address, Phone, Email)
VALUES (3, N'ASUS Technology', N'Lê Hoàng Nam', N'Hồ Chí Minh', N'789 Điện Biên Phủ, Quận 3', N'0903456789', N'support@asus-vn.com');

INSERT INTO Suppliers (SupplierID, SupplierName, ContactName, Province, Address, Phone, Email)
VALUES (4, N'E-DRA Vietnam', N'Phạm Đức Anh', N'Hà Nội', N'101 Trần Hưng Đạo, Quận Hoàn Kiếm', N'0912345678', N'contact@edra-vn.com');

SET IDENTITY_INSERT Suppliers OFF;
GO

-- ============================================
-- 3. INSERT PRODUCTS (8 products với ảnh)
-- ============================================
SET IDENTITY_INSERT Products ON;

-- Bàn phím (CategoryID = 1)
INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (1, N'Bàn phím cơ E-DRA EK375 V2 Beta Blue Black',
N'Bàn phím cơ E-DRA EK375 V2 với switch Blue cho cảm giác bấm sắc nét, đèn RGB custom, keycap PBT double-shot chống mài mòn. Thiết kế compact 75%, layout TKL tiết kiệm không gian.',
4, 1, N'Chiếc', 1290000,
'Bàn phím/Bàn phím cơ E-DRA EK375 V2 Beta Blue Black/đen.png', 1);

INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (2, N'Bàn phím không dây Aula F75 Max Glacier Blue',
N'Bàn phím không dây Aula F75 Max Glacier Blue - Kết nối đa chế độ (2.4GHz, Bluetooth, USB-C), pin trâu, switch hot-swap, silencing foam.',
4, 1, N'Chiếc', 1590000,
'Bàn phím/Bàn phím không dây Aula F75 Max Glacier Blue/Glacier blue.png', 1);

-- Điện thoại (CategoryID = 2)
INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (3, N'iPhone 17 Pro Max 256GB Chính hãng',
N'iPhone 17 Pro Max với chip A19 Pro, màn hình Super Retina XDR 6.9 inch, camera 48MP, pin trâu, hỗ trợ AI Apple Intelligence.',
1, 2, N'Chiếc', 39990000,
'Điện thoại/iPhone 17 Pro Max 256GB | Chính hãng/bạc.png', 1);

INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (4, N'Samsung Galaxy S26 Ultra 12GB 256GB',
N'Samsung Galaxy S26 Ultra với chip Snapdragon 8 Gen 4, màn hình Dynamic AMOLED 2X 6.8 inch, bút S Pen tích hợp, camera 200MP.',
2, 2, N'Chiếc', 32990000,
'Điện thoại/Samsung Galaxy S26 Ultra 12GB 256GB/đen classic.png', 1);

INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (5, N'iPhone 15 128GB Chính hãng VN',
N'iPhone 15 với chip A16 Bionic, màn hình Super Retina XDR 6.1 inch, camera 48MP, Dynamic Island, USB-C.',
1, 2, N'Chiếc', 19990000,
'Điện thoại/iPhone 15 128GB | Chính hãng VN/đen.png', 1);

-- Laptop (CategoryID = 3)
INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (6, N'Laptop ASUS ROG Strix G16 G614PH-S5101W',
N'Laptop gaming ASUS ROG Strix G16 với Intel Core i9-14900HX, NVIDIA RTX 4060 8GB, RAM 16GB DDR5, màn hình 16 inch 165Hz.',
3, 3, N'Chiếc', 44990000,
'Laptop/Laptop ASUS ROG Strix G16 G614PH-S5101W/xám 1.png', 1);

INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (7, N'MacBook Air M4 13 inch 2025 10CPU 8GPU 16GB 256GB',
N'MacBook Air M4 2025 - Chip M4 mới nhất, 10CPU/8GPU, 16GB RAM, 256GB SSD, màn hình Liquid Retina 13.6 inch, pin 18 giờ.',
1, 3, N'Chiếc', 32990000,
'Laptop/MacBook Air M4 13 inch 2025 10CPU 8GPU 16GB 256GB | Chính hãng Apple Việt Nam/bạc.png', 1);

INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (8, N'MacBook Air M3 15 inch 2024 8CPU 10GPU 16GB 256GB',
N'MacBook Air M3 15 inch - Chip M3, 8CPU/10GPU, 16GB RAM, 256GB SSD, màn hình Liquid Retina 15.3 inch, pin 18 giờ.',
1, 3, N'Chiếc', 28990000,
'Laptop/MacBook Air M3 15 inch 2024 8CPU 10GPU 16GB 256GB Sạc 35W | Chính hãng Apple Việt Nam/bạc.png', 1);

-- Màn hình (CategoryID = 4)
INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (9, N'Màn hình Gaming ASUS TUF VG27AQ5A 27 inch',
N'Màn hình gaming ASUS TUF Gaming VG27AQ5A - 27 inch, WQHD 2560x1440, IPS, 180Hz, 1ms MPRT, G-Sync compatible, HDR10.',
3, 4, N'Chiếc', 8990000,
'Màn hình/Màn hình Gaming ASUS TUF VG27AQ5A 27 inch/34 inch - QWHD VA - 180Hz.png', 1);

INSERT INTO Products (ProductID, ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
VALUES (10, N'Màn hình Gaming ASUS ROG Swift OLED PG34WCDM',
N'Màn hình ASUS ROG Swift OLED PG34WCDM - 34 inch UWQHD, OLED, 240Hz, 0.03ms, HDR400, USB-C 90W, RGB lighting.',
3, 4, N'Chiếc', 45990000,
'Màn hình/Màn hình Gaming ASUS ROG Swift OLED PG34WCDM/34 inch – UWQHD OLED – 240Hz.png', 1);

SET IDENTITY_INSERT Products OFF;
GO

-- ============================================
-- 4. INSERT EMPLOYEES (Admin + các role cần thiết)
-- ============================================
-- Password: 123123 (MD5: 4297f44b13955235245b2497399d7a93)

SET IDENTITY_INSERT Employees ON;

-- Admin - Full quyền
INSERT INTO Employees (EmployeeID, FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking, RoleNames)
VALUES (1, N'Phan Văn Badu', '1990-01-15', N'Thừa Thiên Huế', N'0912345678', N'admin@myshop.com',
N'4297f44b13955235245b2497399d7a93', N'nophoto.png', 1, N'Admin,Manager,Employee');

-- Quản lý kho (Inventory)
INSERT INTO Employees (EmployeeID, FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking, RoleNames)
VALUES (2, N'Nguyễn Thị Hoa', '1992-03-20', N'Hà Nội', N'0923456789', N'inventory@myshop.com',
N'4297f44b13955235245b2497399d7a93', N'nophoto.png', 1, N'Inventory');

-- Nhân viên bán hàng (Sales)
INSERT INTO Employees (EmployeeID, FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking, RoleNames)
VALUES (3, N'Trần Văn Minh', '1995-06-10', N'Đà Nẵng', N'0934567890', N'sales@myshop.com',
N'4297f44b13955235245b2497399d7a93', N'nophoto.png', 1, N'Sales');

-- Nhân viên giao hàng (Shipper)
INSERT INTO Employees (EmployeeID, FullName, BirthDate, Address, Phone, Email, Password, Photo, IsWorking, RoleNames)
VALUES (4, N'Lê Thị Mai', '1993-08-25', N'Hồ Chí Minh', N'0945678901', N'delivery@myshop.com',
N'4297f44b13955235245b2497399d7a93', N'nophoto.png', 1, N'Shipper');

SET IDENTITY_INSERT Employees OFF;
GO

-- ============================================
-- 5. INSERT SHIPPERS
-- ============================================
SET IDENTITY_INSERT Shippers ON;

INSERT INTO Shippers (ShipperID, ShipperName, Phone)
VALUES (1, N'Giao Hàng Nhanh (GHN)', N'1900 1234');

INSERT INTO Shippers (ShipperID, ShipperName, Phone)
VALUES (2, N'Viettel Post', N'1900 8095');

INSERT INTO Shippers (ShipperID, ShipperName, Phone)
VALUES (3, N'Giao Hàng Tiết Kiệm (GHTK)', N'1900 1235');

SET IDENTITY_INSERT Shippers OFF;
GO

-- ============================================
-- 6. INSERT PRODUCT PHOTOS (Ảnh bổ sung cho sản phẩm)
-- ============================================
SET IDENTITY_INSERT ProductPhotos ON;

-- iPhone 17 Pro Max (ProductID = 3)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (1, 3, N'Điện thoại/iPhone 17 Pro Max 256GB | Chính hãng/bạc.png', N'iPhone 17 Pro Max - Màu bạc', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (2, 3, N'Điện thoại/iPhone 17 Pro Max 256GB | Chính hãng/xanh đậm.png', N'iPhone 17 Pro Max - Màu xanh đậm', 2, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (3, 3, N'Điện thoại/iPhone 17 Pro Max 256GB | Chính hãng/cam vũ trụ.png', N'iPhone 17 Pro Max - Màu cam vũ trụ', 3, 0);

-- Samsung Galaxy S26 Ultra (ProductID = 4)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (4, 4, N'Điện thoại/Samsung Galaxy S26 Ultra 12GB 256GB/đen classic.png', N'Galaxy S26 Ultra - Đen', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (5, 4, N'Điện thoại/Samsung Galaxy S26 Ultra 12GB 256GB/trắng classic.png', N'Galaxy S26 Ultra - Trắng', 2, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (6, 4, N'Điện thoại/Samsung Galaxy S26 Ultra 12GB 256GB/tím cobalt.png', N'Galaxy S26 Ultra - Tím cobalt', 3, 0);

-- iPhone 15 (ProductID = 5)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (7, 5, N'Điện thoại/iPhone 15 128GB | Chính hãng VN/đen.png', N'iPhone 15 - Đen', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (8, 5, N'Điện thoại/iPhone 15 128GB | Chính hãng VN/xanh dương.png', N'iPhone 15 - Xanh dương', 2, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (9, 5, N'Điện thoại/iPhone 15 128GB | Chính hãng VN/hồng.png', N'iPhone 15 - Hồng', 3, 0);

-- ASUS ROG Strix G16 (ProductID = 6)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (10, 6, N'Laptop/Laptop ASUS ROG Strix G16 G614PH-S5101W/xám 1.png', N'ASUS ROG Strix G16 - Góc 1', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (11, 6, N'Laptop/Laptop ASUS ROG Strix G16 G614PH-S5101W/xám 2.png', N'ASUS ROG Strix G16 - Góc 2', 2, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (12, 6, N'Laptop/Laptop ASUS ROG Strix G16 G614PH-S5101W/xám 3.png', N'ASUS ROG Strix G16 - Bàn phím', 3, 0);

-- MacBook Air M4 (ProductID = 7)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (13, 7, N'Laptop/MacBook Air M4 13 inch 2025 10CPU 8GPU 16GB 256GB | Chính hãng Apple Việt Nam/bạc.png', N'MacBook Air M4 - Bạc', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (14, 7, N'Laptop/MacBook Air M4 13 inch 2025 10CPU 8GPU 16GB 256GB | Chính hãng Apple Việt Nam/xanh da trời.png', N'MacBook Air M4 - Xanh dương', 2, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (15, 7, N'Laptop/MacBook Air M4 13 inch 2025 10CPU 8GPU 16GB 256GB | Chính hãng Apple Việt Nam/vàng ánh sao.png', N'MacBook Air M4 - Vàng', 3, 0);

-- MacBook Air M3 15 inch (ProductID = 8)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (16, 8, N'Laptop/MacBook Air M3 15 inch 2024 8CPU 10GPU 16GB 256GB Sạc 35W | Chính hãng Apple Việt Nam/bạc.png', N'MacBook Air M3 15 - Bạc', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (17, 8, N'Laptop/MacBook Air M3 15 inch 2024 8CPU 10GPU 16GB 256GB Sạc 35W | Chính hãng Apple Việt Nam/trắng vàng.png', N'MacBook Air M3 15 - Vàng', 2, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (18, 8, N'Laptop/MacBook Air M3 15 inch 2024 8CPU 10GPU 16GB 256GB Sạc 35W | Chính hãng Apple Việt Nam/đen.png', N'MacBook Air M3 15 - Đen', 3, 0);

-- ASUS TUF Gaming Monitor (ProductID = 9)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (19, 9, N'Màn hình/Màn hình Gaming ASUS TUF VG27AQ5A 27 inch/34 inch - QWHD VA - 180Hz.png', N'ASUS TUF VG27AQ5A - Trước', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (20, 9, N'Màn hình/Màn hình Gaming ASUS TUF VG27AQ5A 27 inch/34 inch - WQHD VA - 250Hz.png', N'ASUS TUF VG27AQ5A - Sau', 2, 0);

-- ASUS ROG Swift OLED (ProductID = 10)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (21, 10, N'Màn hình/Màn hình Gaming ASUS ROG Swift OLED PG34WCDM/34 inch – UWQHD OLED – 240Hz.png', N'ASUS ROG Swift OLED - 34 inch', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (22, 10, N'Màn hình/Màn hình Gaming ASUS ROG Swift OLED PG34WCDM/49 inch – DQHD OLED – 144Hz.png', N'ASUS ROG Swift OLED - 49 inch', 2, 0);

-- Bàn phím E-DRA (ProductID = 1)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (23, 1, N'Bàn phím/Bàn phím cơ E-DRA EK375 V2 Beta Blue Black/đen.png', N'E-DRA EK375 V2 - Đen', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (24, 1, N'Bàn phím/Bàn phím cơ E-DRA EK375 V2 Beta Blue Black/đen đỏ.png', N'E-DRA EK375 V2 - Đen đỏ', 2, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (25, 1, N'Bàn phím/Bàn phím cơ E-DRA EK375 V2 Beta Blue Black/xanh đen.png', N'E-DRA EK375 V2 - Xanh đen', 3, 0);

-- Bàn phím Aula (ProductID = 2)
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (26, 2, N'Bàn phím/Bàn phím không dây Aula F75 Max Glacier Blue/Glacier blue.png', N'Aula F75 Max - Glacier Blue', 1, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (27, 2, N'Bàn phím/Bàn phím không dây Aula F75 Max Glacier Blue/Pro đen.png', N'Aula F75 Max - Pro đen', 2, 0);
INSERT INTO ProductPhotos (PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
VALUES (28, 2, N'Bàn phím/Bàn phím không dây Aula F75 Max Glacier Blue/Pro tím.png', N'Aula F75 Max - Pro tím', 3, 0);

SET IDENTITY_INSERT ProductPhotos OFF;
GO

-- ============================================
-- 7. INSERT PRODUCT ATTRIBUTES
-- ============================================
SET IDENTITY_INSERT ProductAttributes ON;

-- iPhone 17 Pro Max (ProductID = 3)
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (1, 3, N'Màn hình', N'6.9 inch Super Retina XDR, OLED, 2866 x 1320 pixels, ProMotion 120Hz', 1);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (2, 3, N'Chip', N'A19 Pro', 2);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (3, 3, N'RAM', N'8GB', 3);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (4, 3, N'Bộ nhớ', N'256GB', 4);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (5, 3, N'Camera', N'48MP + 48MP Ultra Wide + 12MP Telephoto 5x', 5);

-- Samsung Galaxy S26 Ultra (ProductID = 4)
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (6, 4, N'Màn hình', N'6.8 inch Dynamic AMOLED 2X, QHD+, 120Hz', 1);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (7, 4, N'Chip', N'Snapdragon 8 Gen 4', 2);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (8, 4, N'RAM', N'12GB', 3);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (9, 4, N'Bộ nhớ', N'256GB', 4);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (10, 4, N'Camera', N'200MP + 50MP + 12MP + 50MP 5x', 5);

-- MacBook Air M4 (ProductID = 7)
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (11, 7, N'Màn hình', N'13.6 inch Liquid Retina, 2560 x 1664 pixels', 1);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (12, 7, N'Chip', N'Apple M4, 10CPU/8GPU', 2);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (13, 7, N'RAM', N'16GB unified', 4);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (14, 7, N'Ổ cứng', N'256GB SSD', 5);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (15, 7, N'Pin', N'18 giờ', 6);

-- ASUS ROG Strix G16 (ProductID = 6)
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (16, 6, N'Màn hình', N'16 inch WUXGA, 1920 x 1200, 165Hz', 1);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (17, 6, N'CPU', N'Intel Core i9-14900HX', 2);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (18, 6, N'GPU', N'NVIDIA RTX 4060 8GB', 3);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (19, 6, N'RAM', N'16GB DDR5', 4);
INSERT INTO ProductAttributes (AttributeID, ProductID, AttributeName, AttributeValue, DisplayOrder)
VALUES (20, 6, N'Ổ cứng', N'1TB PCIe Gen4 SSD', 5);

SET IDENTITY_INSERT ProductAttributes OFF;
GO

-- ============================================
-- 8. INSERT CUSTOMERS (vài khách hàng mẫu)
-- ============================================
SET IDENTITY_INSERT Customers ON;

INSERT INTO Customers (CustomerID, CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
VALUES (1, N'Nguyễn Văn An', N'An', N'Hà Nội', N'123 Đường ABC, Quận 1', N'0901234567', N'an@gmail.com', N'4297f44b13955235245b2497399d7a93', 0);

INSERT INTO Customers (CustomerID, CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
VALUES (2, N'Trần Thị Bình', N'Bình', N'Đà Nẵng', N'456 Đường XYZ, Quận Hải Châu', N'0912345678', N'binh@yahoo.com', N'4297f44b13955235245b2497399d7a93', 0);

INSERT INTO Customers (CustomerID, CustomerName, ContactName, Province, Address, Phone, Email, Password, IsLocked)
VALUES (3, N'Lê Hoàng Nam', N'Nam', N'Hồ Chí Minh', N'789 Đường DEF, Quận Bình Thạnh', N'0923456789', N'nam@gmail.com', N'4297f44b13955235245b2497399d7a93', 0);

SET IDENTITY_INSERT Customers OFF;
GO

-- ============================================
-- THÔNG BÁO HOÀN THÀNH
-- ============================================
PRINT '============================================';
PRINT 'IMPORT DATA HOAN THANH!';
PRINT '============================================';
PRINT '';
PRINT '1. Categories: 4 records (Bàn phím, Điện thoại, Laptop, Màn hình)';
PRINT '2. Suppliers: 4 records (Apple, Samsung, ASUS, E-DRA)';
PRINT '3. Products: 10 records';
PRINT '4. ProductPhotos: 28 records';
PRINT '5. ProductAttributes: 20 records';
PRINT '6. Employees: 4 records (Admin, Inventory, Sales, Shipper)';
PRINT '7. Shippers: 3 records';
PRINT '8. Customers: 3 records (test accounts)';
PRINT '';
PRINT 'Password for all accounts: 123123';
PRINT '';
PRINT 'Employee Accounts:';
PRINT '  - admin@myshop.com (Admin, Manager, Employee)';
PRINT '  - inventory@myshop.com (Inventory)';
PRINT '  - sales@myshop.com (Sales)';
PRINT '  - delivery@myshop.com (Shipper)';
PRINT '';
PRINT 'Customer Accounts:';
PRINT '  - an@gmail.com';
PRINT '  - binh@yahoo.com';
PRINT '  - nam@gmail.com';
PRINT '';
PRINT '============================================';
GO
