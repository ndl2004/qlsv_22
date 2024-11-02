use qlsv_2
go

CREATE TABLE login (
    id INT PRIMARY KEY IDENTITY(1,1), -- Khóa chính tự động tăng
    taikhoan NVARCHAR(255) NOT NULL,  -- Tên tài khoản không được null
    matkhau NVARCHAR(255) NOT NULL     -- Mật khẩu được lưu dưới dạng chuỗi
);

go
use qlsv_2
go
CREATE TABLE Students (
    MSSV INT PRIMARY KEY,
    HoTen NVARCHAR(100),
    NgaySinh DATE,
    GioiTinh BIT, -- 1 cho Nam, 0 cho Nữ
    Nganh NVARCHAR(100),
    DiaChi NVARCHAR(255)
);

INSERT INTO Students (MSSV, HoTen, NgaySinh, GioiTinh, Nganh, DiaChi,MaLop) VALUES 
('2122110147', N'Nguyễn Đình Lợi', '2004-12-12', 1, N'Công nghệ thông tin', N'Số 1, Đường A, Quận B, TP.HCM'),
('2122110148', N'Trần Thị B', '2000-02-20', 0, N'Ngôn ngữ Anh', N'Số 2, Đường C, Quận D, TP.HCM'),
('2122110149', N'Phạm Văn C', '2000-03-30', 1, N'Cơ khí', N'Số 3, Đường E, Quận F, TP.HCM'),
('2122110150', N'Lê Thị D', '2000-04-25', 0, N'Quản trị kinh doanh', N'Số 4, Đường G, Quận H, TP.HCM'),
('2122110151', N'Vũ Văn E', '2000-05-10', 1, N'Sinh học', N'Số 5, Đường I, Quận J, TP.HCM');

INSERT INTO Students (MSSV, HoTen, NgaySinh, GioiTinh, Nganh, DiaChi, MaLop) 
VALUES (2122110152, N'Nguyễn Văn F', '2000-06-15', 1, N'Khoa học máy tính', N'Số 6, Đường H, Quận K, TP.HCM', N'CCQ2211D');

INSERT INTO login (taikhoan, matkhau) VALUES ('loi', 'loi1');


-- Xem dữ liệu trong bảng areas
SELECT * FROM Students;
GO
SELECT * FROM login;
GO




