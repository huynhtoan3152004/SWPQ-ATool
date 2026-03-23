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

## 5) API chính

### AuthService

- `POST /auth/register`
- `POST /auth/login`

### QuestionService

- `POST /questions` (role `STUDENT`)
- `GET /questions`
- `GET /questions/{id}`
- `PATCH /questions/{id}/approve` (role `GVHD`)
- `PATCH /questions/{id}/assign` (role `GVHD`)
- `PATCH /questions/{id}/mark-answered` (role `TEACHER`, dùng nội bộ qua AnswerService)

### AnswerService

- `POST /answers` (role `TEACHER`)
- `GET /answers?questionId=<guid>`

## 6) Demo flow nhanh trên Swagger

1. `POST /auth/login` bằng Student -> lấy token.
2. Dùng token Student gọi `POST /questions`.
3. Login GVHD -> token GVHD -> gọi approve + assign.
4. Login Teacher -> token Teacher -> gọi `POST /answers`.
5. Gọi `GET /questions/{id}` để xác nhận status `ANSWERED`.

## 7) Database: có cần tạo thủ công trên máy không?

- Nếu chạy qua AppHost + Docker: **không cần** tự tạo DB thủ công. Aspire tạo resource Postgres và inject connection string cho service.
- Nếu chạy từng service độc lập không qua AppHost: cần có PostgreSQL local và tạo DB theo connection string mặc định trong từng `appsettings.json`.

## 8) Giao tiếp service-service

- Hiện tại dùng REST đơn giản:
  - `AnswerService` -> `QuestionService` endpoint `PATCH /questions/{id}/mark-answered`
- URL cấu hình tại `AnswerService/appsettings.json`:
  - `Services:QuestionServiceBaseUrl`
