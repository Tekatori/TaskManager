
# TaskManager

**TaskManager** là một ứng dụng web giúp bạn quản lý dự án và công việc hiệu quả. Dự án được xây dựng bằng ASP.NET Core MVC theo mô hình phân lớp (BLL, DAL, Models, MVC) và sử dụng **MySQL** làm hệ quản trị cơ sở dữ liệu.

## 💡 Tính năng

- Đăng nhập, đăng ký người dùng.
- Quản lý vai trò người dùng (role).
- Tạo và quản lý dự án.
- Tạo, sửa, xóa công việc (task) gắn với dự án.
- Giao nhiệm vụ cho người dùng.
- Giao diện Dashboard hiển thị thống kê tổng quan:
  - Số lượng dự án, nhiệm vụ, người dùng, bình luận, tệp đính kèm.
  - Các công việc gần đến deadline.
  - Người dùng mới được tạo gần đây.
- Bình luận vào công việc.
- Đính kèm tệp.

## 🧱 Kiến trúc dự án

- **TaskManager.Models**: Định nghĩa các mô hình (Entity).
- **TaskManager.DAL**: Truy cập dữ liệu, thao tác với MySQL qua EF Core.
- **TaskManager.BLL**: Chứa logic nghiệp vụ.
- **TaskManager (Web MVC)**: Giao diện người dùng và điều phối luồng xử lý.

## ⚙️ Cài đặt và chạy dự án

### 1. Clone dự án

```bash
git clone https://github.com/Tekatori/TaskManager.git
```

Mở file `TaskManager.sln` bằng **Visual Studio 2022** hoặc mới hơn.

### 2. Tạo cơ sở dữ liệu MySQL

Sử dụng MySQL Workbench hoặc dòng lệnh để tạo database:

```sql
CREATE DATABASE task_manager_db;
```

### 3. Cập nhật chuỗi kết nối

Trong file `appsettings.json` của project `TaskManager`, sửa lại như sau:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=task_manager_db;user=root;password=your_password"
}
```

> Thay `your_password` bằng mật khẩu thật của MySQL.

### 4. Tạo bảng trong CSDL

Nếu bạn đã có migration sẵn:

```bash
dotnet ef database update
```

Nếu chưa có, bạn có thể tạo migration đầu tiên:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

> Cài EF tool nếu cần:
```bash
dotnet tool install --global dotnet-ef
```

### 5. Chạy ứng dụng

Nhấn **F5** trên Visual Studio hoặc chạy lệnh:

```bash
dotnet run --project TaskManager
```

Ứng dụng sẽ chạy tại `https://localhost:5001` hoặc `http://localhost:5000`.

---

## 📌 Yêu cầu

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- MySQL Server 5.7+ / 8.0
- Visual Studio 2022 hoặc sử dụng CLI (.NET CLI)

---

## 📷 Demo (nếu có)

![Dashboard](wwwroot/img/dashboard.png)
![Calendar](wwwroot/img/calendar.png)
![Home](wwwroot/img/home.png)

---

## 🤝 Đóng góp

Chào mừng mọi đóng góp! Bạn có thể:

- Fork dự án
- Tạo nhánh mới: `git checkout -b chuc-nang-moi`
- Commit & Push: `git push origin chuc-nang-moi`
- Tạo Pull Request

---

## 📄 Giấy phép

Dự án này sử dụng giấy phép **MIT License**.
