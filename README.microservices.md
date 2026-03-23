# SWP Q&A Microservices (Aspire + .NET 8)

## 1) Kiến trúc đã setup

- `AuthService`: đăng ký, đăng nhập, phát JWT.
- `QuestionService`: quản lý câu hỏi + workflow trạng thái.
- `AnswerService`: lưu câu trả lời + gọi REST sang `QuestionService` để cập nhật `ANSWERED`.
- `SWPQ&ATool.AppHost`: orchestration toàn bộ services + PostgreSQL resource.

Mỗi service có:

- `Controllers`
- `Services`
- `Repositories`
- `Entities`
- `Data (DbContext + Seed)`

## 2) Workflow trạng thái

`PENDING -> APPROVED -> ASSIGNED -> ANSWERED`

- Student tạo câu hỏi: `PENDING`
- GVHD duyệt: `APPROVED`
- GVHD assign teacher: `ASSIGNED`
- Teacher trả lời (qua `AnswerService`): `ANSWERED`

## 3) Tài khoản seed mặc định

Được seed trong `AuthService`:

- Admin: `admin@swp.local / 123456`
- Student: `student1@swp.local / 123456`
- GVHD: `gvhd1@swp.local / 123456`
- Teacher: `teacher1@swp.local / 123456`

## 4) Chạy bằng AppHost (khuyến nghị)

### Điều kiện

- .NET SDK 8
- Docker Desktop đang chạy (vì Aspire sẽ dùng container cho Postgres)

### Lệnh chạy

```powershell
dotnet run --project .\SWPQ&ATool.AppHost\SWPQ&ATool.AppHost.csproj
```

Sau khi chạy, terminal sẽ in link dashboard Aspire dạng:

- `https://localhost:<port>`

Vào dashboard để mở từng service và Swagger của từng service.

### Quản lý DB trực quan

- Trong Aspire dashboard sẽ có thêm resource `postgres` và `pgadmin`.
- Bạn có thể mở `pgadmin` trực tiếp từ dashboard để xem schema/table/data.

## 4.1) Xem DB bằng DBeaver

Bạn hoàn toàn có thể dùng DBeaver để xem dữ liệu khi chạy qua AppHost.

### Bước 1: chạy AppHost

```powershell
dotnet run --project .\SWPQ&ATool.AppHost\SWPQ&ATool.AppHost.csproj
```

### Bước 2: lấy port Postgres đang map ra máy

```powershell
docker ps
```

Tìm container postgres của Aspire, ví dụ cột PORTS có dạng:

- `0.0.0.0:54329->5432/tcp`

=> Port để vào DBeaver là `54329`.

### Bước 3: cấu hình connection trong DBeaver

- Cách chắc chắn nhất: mở Aspire Dashboard -> resource `postgres` -> copy `Connection String`.
- Dán vào DBeaver (hoặc tách ra các field Host/Port/Database/User/Password).
- Đổi `Database` thành một trong các DB cần xem:
  - `authdb`
  - `questiondb`
  - `answerdb`

## 4.2) Các database hiện có

- `authdb`
- `questiondb`
- `answerdb`

Mỗi service sở hữu DB riêng theo đúng microservice boundary.

## 5) API chính

### AuthService

- `POST /auth/register`
- `POST /auth/login`
- `GET /auth/students` (role `GVHD`, `ADMIN`)
- `GET /auth/teachers` (role `GVHD`, `ADMIN`)
- `PATCH /auth/users/{userId}/role` (role `ADMIN`)

### QuestionService

- `POST /questions` (role `STUDENT`) -> tự gán cho giảng viên phụ trách topic
- `GET /questions?topicId=&semesterId=&assignedTo=&visibility=&year=&month=`
- `GET /questions/{id}`
- `PATCH /questions/{id}/approve` (role `GVHD`)
- `PATCH /questions/{id}/assign` (role `GVHD`)
- `PATCH /questions/{id}/mark-answered` (role `TEACHER`, dùng nội bộ qua AnswerService)

### Semester APIs (QuestionService)

- `POST /semesters` (role `GVHD`, `ADMIN`)
- `GET /semesters?name=&year=&month=`

### Topic APIs (QuestionService)

- `POST /topics` (role `GVHD`, `ADMIN`)
- `GET /topics?semesterId=&lecturerId=&year=&month=&keyword=`
- `GET /topics/my` (role `TEACHER`)
- `PATCH /topics/{id}` (role `TEACHER`, `GVHD`, `ADMIN`)

### AnswerService

- `POST /answers` (role `TEACHER`)
- `GET /answers?questionId=<guid>`

## 6) Demo flow nhanh trên Swagger

1. Login `GVHD` -> gọi `GET /auth/teachers` để lấy lecturer id.
2. Tạo semester: `POST /semesters`.
3. Tạo topic có lecturer: `POST /topics`.
4. Login `Student` -> tạo câu hỏi vào topic: `POST /questions`.
5. Login `Teacher` (lecturer đã gán) -> trả lời: `POST /answers`.
6. Gọi `GET /questions/{id}` để xác nhận status `ANSWERED`.

## 7) Database: có cần tạo thủ công trên máy không?

- Nếu chạy qua AppHost + Docker: **không cần** tự tạo DB thủ công. Aspire tạo resource Postgres và inject connection string cho service.
- Nếu chạy từng service độc lập không qua AppHost: cần có PostgreSQL local và tạo DB theo connection string mặc định trong từng `appsettings.json`.

## 8) Giao tiếp service-service

- Hiện tại dùng REST đơn giản:
  - `AnswerService` -> `QuestionService` endpoint `PATCH /questions/{id}/mark-answered`
- URL cấu hình tại `AnswerService/appsettings.json`:
  - `Services:QuestionServiceBaseUrl`
