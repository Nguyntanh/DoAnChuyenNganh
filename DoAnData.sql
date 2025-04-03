CREATE DATABASE DoAnData

USE DoAnData

-- 1. Tạo bảng Categories (Phân loại sản phẩm)
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL
);

-- 2. Tạo bảng Users (Người dùng)
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(256) NOT NULL, -- Mã hóa bằng ASP.NET Identity
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('customer', 'seller', 'admin')),
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('active', 'pending', 'inactive')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- 3. Tạo bảng Products (Sản phẩm)
CREATE TABLE Products (
    ProductId INT PRIMARY KEY,
    SellerId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    CategoryId INT NOT NULL,
    Stock INT NOT NULL CHECK (Stock >= 0),
    Description NVARCHAR(MAX),
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('pending', 'approved', 'rejected')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (SellerId) REFERENCES Users(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- 4. Tạo bảng Orders (Đơn hàng)
CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    SellerId INT NOT NULL,
    TotalPrice DECIMAL(18,2) NOT NULL CHECK (TotalPrice >= 0),
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('pending', 'confirmed', 'shipped', 'delivered', 'canceled')),
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    ShippingAddress NVARCHAR(255) NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Users(Id),
    FOREIGN KEY (SellerId) REFERENCES Users(Id)
);

-- 5. Tạo bảng OrderDetails (Chi tiết đơn hàng)
CREATE TABLE OrderDetails (
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
    PRIMARY KEY (OrderId, ProductId), -- Khóa chính composite
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

-- 6. Tạo bảng Reviews (Đánh giá sản phẩm)
CREATE TABLE Reviews (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    ProductId INT NOT NULL,
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(MAX),
    ReviewDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (CustomerId) REFERENCES Users(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);

CREATE TABLE Cart (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Cart_Users FOREIGN KEY (CustomerId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE TABLE CartItem (
    CartId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    CONSTRAINT PK_CartItem PRIMARY KEY (CartId, ProductId),
    CONSTRAINT FK_CartItem_Cart FOREIGN KEY (CartId) REFERENCES Cart(Id) ON DELETE CASCADE,
    CONSTRAINT FK_CartItem_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId) ON DELETE CASCADE
);

CREATE TABLE SupportTicket (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    OrderId INT NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'open' CHECK (Status IN ('open', 'resolved')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ResolvedAt DATETIME NULL,
    CONSTRAINT FK_SupportTicket_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SupportTicket_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id)
);

INSERT INTO Categories (Name)
VALUES 
(N'Sữa Rửa Mặt'),  -- Id = 1
(N'Nước Hoa'),     -- Id = 2
(N'Nước Tẩy Trang'), -- Id = 3
(N'Son'),          -- Id = 4
(N'Kem Chống Nắng'), -- Id = 5
(N'Phấn Phủ'),     -- Id = 6
(N'Tẩy Da Chết');  -- Id = 7

INSERT INTO Users (Username, Password, Email, Role, Status)
VALUES (N'seller1', N'hashedpassword', N'seller1@example.com', 'seller', 'active');

INSERT INTO Products (ProductId, SellerId, Name, Price, CategoryId, Stock, Description, Status)
VALUES 
(422208973, 1, N'Sữa Rửa Mặt', 367000.00, 1, 100, 
 N'Sữa Rửa Mặt Cerave Sạch Sâu là sản phẩm sữa rửa mặt đến từ thương hiệu mỹ phẩm Cerave của Mỹ, với sự kết hợp của ba Ceramides thiết yếu, Hyaluronic Acid sản phẩm giúp làm sạch và giữ ẩm cho làn da mà không ảnh hưởng đến hàng rào bảo vệ da mặt và cơ thể.', 
 'pending'),

(318900012, 1, N'Nước Hoa', 208000.00, 2, 100, 
 N'Nước Hoa Hồng Klairs Supple Preparation là dòng sản phẩm toner được thương hiệu Dear, Klairs thiết kế chuyên biệt dành cho làn da nhạy cảm. Với bảng thành phần chiết xuất từ thực vật và kết cấu lỏng nhẹ, thấm nhanh trên da, nước hoa hồng Klairs sẽ giúp cân bằng độ pH và cấp ẩm cho làn da hiệu quả, hỗ trợ cho các bước skincare tiếp theo đạt hiệu quả tối ưu.', 
 'pending'),

(200400003, 1, N'Nước Tẩy Trang', 336000.00, 3, 100, 
 N'Dành Cho Da Nhạy Cảm Bioderma Sensibio H2O là sản phẩm nước tẩy trang công nghệ Micellar đầu tiên trên thế giới, do thương hiệu dược mỹ phẩm Bioderma nổi tiếng của Pháp phát minh. Dung dịch giúp làm sạch sâu da và loại bỏ lớp trang điểm nhanh chóng mà không cần rửa lại bằng nước. Công thức dịu nhẹ, không kích ứng, không gây khô căng da, đặc biệt phù hợp với làn da nhạy cảm.', 
 'pending'),

(205100137, 1, N'Nước Tẩy Trang', 148000.00, 3, 100, 
 N'Nước Tẩy Trang L''Oréal là dòng sản phẩm tẩy trang dạng nước đến từ thương hiệu L''Oreal Paris, được ứng dụng công nghệ Micellar dịu nhẹ giúp làm sạch da, lấy đi bụi bẩn, dầu thừa và cặn trang điểm chỉ trong một bước, mang lại làn da thông thoáng, mềm mượt mà không hề khô căng.', 
 'pending'),

(422206303, 1, N'Son', 277000.00, 4, 100, 
 N'3CE Blur Water Tint là dòng son tint dạng nước đến từ thương hiệu 3CE của Hàn Quốc. Với chất son trơn mịn, mọng nước ngay từ lần thoa đầu tiên, Blur Water Tint sẽ mang đến cho bạn một làn môi mềm mại, căng mọng, đầy vẻ hờn dỗi nũng nịu.', 
 'pending'),

(422215428, 1, N'Son', 337000.00, 4, 100, 
 N'Son Lì 3CE Hazy Lip Clay Mịn Môi 4g là sản phẩm son môi đến từ thương hiệu 3CE - Hàn Quốc. Sản phẩm với kết cấu kem lì mềm mịn, hiệu ứng mờ lì che khuyết điểm hoàn hảo, mỏng nhẹ lướt dễ dàng trên môi giúp lớp trang điểm thêm lung linh.', 
 'pending'),

(253900006, 1, N'Kem Chống Nắng', 205000.00, 5, 100, 
 N'Kem Chống Nắng Skin1004 Cho Da Nhạy Cảm là sản phẩm kem chống nắng da mặt đến từ thương hiệu mỹ phẩm Skin1004 của Hàn Quốc, là kem chống nắng vật lý với chiết xuất rau má và chất kem mỏng nhẹ có màu giúp che phủ nhẹ khuyết điểm cho da. Công thức đa năng vừa chống nắng vừa đều màu da lại dưỡng ẩm chính là sản phẩm mà những cô nàng da mụn hay da dầu nhạy cảm cần vì không cần sử dụng quá nhiều bước lỉnh kỉnh.', 
 'pending'),

(422220412, 1, N'Phấn Phủ', 278000.00, 6, 100, 
 N'Phấn Phủ Carslan Black Magnetic Soft Mist Powder Dạng Nén 8g là sản phẩm phấn phủ đến từ thương hiệu Carslan - Trung Quốc, với công thức tiên tiến giúp che phủ lỗ chân lông, làm mờ các khuyết điểm cho lớp nền mịn lì tự nhiên và kiểm soát dầu hiệu quả suốt cả ngày dài. Thành phần an toàn, không chứa Talc hoặc các chất gây kích ứng, phù hợp với mọi loại da.', 
 'pending'),

(248700071, 1, N'Son', 316000.00, 4, 100, 
 N'Son Mịn Môi Naris CNC 3g. Son môi luôn là một món đồ làm đẹp không thể thiếu đối với nhiều cô nàng. Bởi lẽ chỉ cần chút son môi đã tạo vẻ đẹp quyến rũ, đầy sức sống cho khuôn mặt. Tuy nhiên, để chọn được 1 thỏi son chất lượng tốt, màu sắc đẹp và giá cả hợp lý không hề dễ dàng. Hasaki xin giới thiệu đến bạn Son Mịn Môi Naris CNC đến từ thương hiệu mỹ phẩm Nhật Bản, Naris Cosmetic với độ bám cao, dưỡng môi hiệu quả, khả năng giữ màu ổn mang đến cho bạn bờ môi mịn màng, chuẩn màu đầy quyến rũ.', 
 'pending'),

(308100015, 1, N'Tẩy Da Chết', 89000.00, 7, 100, 
 N'Tẩy Da Chết Toàn Thân Cocoon Cà Phê Đắk Lắk là dòng tẩy tế bào chết toàn thân từ thương hiệu mỹ phẩm Cocoon của Việt Nam là một trong những sản phẩm với thành phần tự nhiên có nguồn gốc trong nước như hạt cà phê Đắk Lắk nguyên chất xay nhuyễn, hòa quyện cùng bơ cacao Tiền Giang giúp loại bỏ tế bào chết hiệu quả, làm đều màu da, mang lại năng lượng, giúp da trở nên mềm mại và rạng rỡ giúp mang đến cho bạn sản phẩm thuần chay tốt nhất với niềm tự hào từ nguyên liệu thuần Việt.', 
 'pending');

 INSERT INTO Users (Username, Password, Email, Role, Status, CreatedAt)
VALUES 
-- Customers
(N'customer1', N'hashedpassword2', N'customer1@example.com', 'customer', 'active', '2025-01-02 10:00:00'), -- Id = 2
(N'customer2', N'hashedpassword3', N'customer2@example.com', 'customer', 'active', '2025-01-03 10:00:00'), -- Id = 3
(N'customer3', N'hashedpassword4', N'customer3@example.com', 'customer', 'active', '2025-01-04 10:00:00'), -- Id = 4
-- Admin
(N'admin1', N'hashedpassword5', N'admin1@example.com', 'admin', 'active', '2025-01-01 09:00:00'); -- Id = 5

INSERT INTO Orders (CustomerId, SellerId, TotalPrice, Status, OrderDate, ShippingAddress)
VALUES 
-- Đơn hàng 1: Customer 1 mua Sữa Rửa Mặt (367000) + Son (277000) = 644000
(2, 1, 644000.00, 'shipped', '2025-03-01 14:00:00', N'123 Đường Láng, Hà Nội'),
-- Đơn hàng 2: Customer 2 mua Nước Tẩy Trang (336000)
(3, 1, 336000.00, 'delivered', '2025-03-02 15:00:00', N'456 Nguyễn Trãi, TP.HCM'),
-- Đơn hàng 3: Customer 3 mua Kem Chống Nắng (205000) + Phấn Phủ (278000) + Tẩy Da Chết (89000) = 572000
(4, 1, 572000.00, 'pending', '2025-03-03 16:00:00', N'789 Lê Lợi, Đà Nẵng');

INSERT INTO OrderDetails (OrderId, ProductId, Quantity, Price)
VALUES 
-- Đơn hàng 1
(1, 422208973, 1, 367000.00), -- Sữa Rửa Mặt
(1, 422206303, 1, 277000.00), -- Son (3CE Blur Water Tint)
-- Đơn hàng 2
(2, 200400003, 1, 336000.00), -- Nước Tẩy Trang (Bioderma)
-- Đơn hàng 3
(3, 253900006, 1, 205000.00), -- Kem Chống Nắng
(3, 422220412, 1, 278000.00), -- Phấn Phủ
(3, 308100015, 1, 89000.00); -- Tẩy Da Chết

INSERT INTO Reviews (CustomerId, ProductId, Rating, Comment, ReviewDate)
VALUES 
-- Customer 1 đánh giá
(2, 422208973, 5, N'Rất tốt, làm sạch sâu và không khô da!', '2025-03-05 10:00:00'), -- Sữa Rửa Mặt
(2, 422206303, 4, N'Màu son đẹp, nhưng hơi nhanh phai.', '2025-03-05 10:05:00'), -- Son
-- Customer 2 đánh giá
(3, 200400003, 5, N'Tẩy trang rất sạch, không kích ứng da.', '2025-03-06 11:00:00'), -- Nước Tẩy Trang
-- Customer 3 đánh giá
(4, 253900006, 4, N'Chống nắng tốt, nhưng hơi bóng dầu.', '2025-03-07 12:00:00'); -- Kem Chống Nắng

INSERT INTO Cart (CustomerId, CreatedAt)
VALUES 
(2, '2025-03-29 09:00:00'), -- Giỏ hàng của customer1, hôm nay
(4, '2025-03-29 10:30:00'); -- Giỏ hàng của customer3, hôm nay

INSERT INTO CartItem (CartId, ProductId, Quantity)
VALUES 
(1, 318900012, 1), -- Giỏ hàng 1 (customer1): 1 Nước Hoa Klairs
(1, 422215428, 2), -- Giỏ hàng 1 (customer1): 2 Son Lì 3CE Hazy Lip Clay
(2, 248700071, 1), -- Giỏ hàng 2 (customer3): 1 Son Naris CNC
(2, 308100015, 3); -- Giỏ hàng 2 (customer3): 3 Tẩy Da Chết Cocoon

INSERT INTO SupportTicket (UserId, OrderId, Title, Description, Status, CreatedAt, ResolvedAt)
VALUES 
(2, 1, N'Giao hàng chậm', N'Đơn hàng #1 đã được đánh dấu shipped từ 01/03 nhưng đến nay 29/03 vẫn chưa nhận được.', 'open', '2025-03-29 08:00:00', NULL),
(3, 2, N'Sản phẩm tuyệt vời', N'Mình rất thích Nước Tẩy Trang Bioderma từ đơn hàng #2, có khuyến mãi nào không?', 'resolved', '2025-03-06 14:00:00', '2025-03-06 16:00:00'),
(4, 3, N'Hủy đơn hàng', N'Mình muốn hủy đơn hàng #3 vì chờ quá lâu chưa xác nhận.', 'open', '2025-03-29 11:00:00', NULL),
(1, 2, N'Hỏi về kiểm duyệt', N'Sản phẩm mới đăng từ 28/03 sao vẫn chưa được duyệt?', 'open', '2025-03-29 09:30:00', NULL);

SELECT * FROM Categories;
SELECT * FROM Users;
SELECT * FROM Orders;
SELECT * FROM OrderDetails;
SELECT * FROM Reviews;
SELECT * FROM Products;
SELECT * FROM Cart;
SELECT * FROM CartItem;
SELECT * FROM SupportTicket;

UPDATE Products
SET Status = 'approved'
WHERE ProductId = 248700071;

CREATE TABLE LoginRequest(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(256) NOT NULL,
	FOREIGN KEY (Username, Password) REFERENCES Users(Username, Password)
);