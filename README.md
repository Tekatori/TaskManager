TaskManager
TaskManager là một ứng dụng quản lý dự án và công việc, cho phép người dùng tạo, theo dõi và quản lý các nhiệm vụ trong các dự án khác nhau. Ứng dụng được xây dựng với kiến trúc phân lớp, bao gồm các tầng:

TaskManager.BLL: Business Logic Layer – Xử lý các logic nghiệp vụ.

TaskManager.DAL: Data Access Layer – Quản lý truy cập dữ liệu.

TaskManager.Models: Định nghĩa các mô hình dữ liệu.

TaskManager: Giao diện người dùng và điểm khởi đầu của ứng dụng.

Tính năng
Quản lý người dùng với các vai trò khác nhau.

Tạo và quản lý dự án.

Tạo, cập nhật và theo dõi tiến độ công việc.

Thêm bình luận vào các công việc.

Phân quyền và phân công công việc cho người dùng.

Cấu trúc cơ sở dữ liệu
Cơ sở dữ liệu bao gồm các bảng chính:

Users: Lưu thông tin người dùng.

Projects: Lưu thông tin dự án.

Tasks: Lưu thông tin công việc.

Comments: Lưu bình luận liên quan đến công việc.

Chi tiết cấu trúc bảng có thể được tìm thấy trong tệp README.md hiện tại của kho lưu trữ.

Cài đặt
Clone kho lưu trữ:

bash
Sao chép
Chỉnh sửa
git clone https://github.com/Tekatori/TaskManager.git
Mở tệp TaskManager.sln bằng Visual Studio.

Khôi phục các gói NuGet cần thiết.

Cấu hình chuỗi kết nối đến cơ sở dữ liệu trong tệp cấu hình.

Chạy ứng dụng và bắt đầu sử dụng.

Đóng góp
Chúng tôi hoan nghênh mọi đóng góp từ cộng đồng. Nếu bạn muốn đóng góp, vui lòng fork kho lưu trữ, tạo nhánh mới và gửi pull request.

Giấy phép
Dự án này được cấp phép theo giấy phép MIT.

Nếu bạn cần thêm thông tin hoặc hỗ trợ, vui lòng liên hệ với chúng tôi qua email hoặc tạo issue trên GitHub.
