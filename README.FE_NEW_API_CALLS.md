# README - FE New API Calls (Delta)

Tài liệu này chỉ tổng hợp **các API mới/bổ sung** mà FE thường chưa call đủ, kèm mẫu request để gắn nhanh.

## 1) Vì sao FE báo `Failed to load assigned questions`?

`GET /questions` chỉ trả lỗi khi có vấn đề request, không phải vì "không có data".

Các nguyên nhân thường gặp:
- Sai endpoint (gọi `/questions/assigned` thay vì `GET /questions?assignedTo=...`).
- `assignedTo` không phải GUID hợp lệ (backend bind query lỗi -> 400).
- Thiếu/mất `Authorization: Bearer <token>` -> 401/403.
- FE đang dùng nhầm base URL/proxy path.
- Token role không phù hợp hoặc token hết hạn.

Lưu ý: nếu chỉ "chưa có data", API vẫn trả `200` với mảng rỗng `[]`, không phải lỗi.

---

## 2) API mới FE nên call thêm

## 2.0 Lecturer registration + role approval flow (NEW)

### `POST /auth/register-lecturer`
- Service: AuthService
- Auth: không cần
- Mục tiêu: giảng viên tự đăng ký tài khoản, vào trạng thái chờ duyệt role

Body:
```json
{
  "fullName": "Minh Nguyen",
  "email": "minhnt01@swp.local",
  "password": "123456"
}
```

Kết quả:
- user tạo với `role = STUDENT`
- `requestedRole = TEACHER`

### `GET /auth/users/pending-role-requests`
- Service: AuthService
- Auth: `ADMIN`
- Dùng cho màn hình duyệt role.

### `PATCH /auth/users/{userId}/role`
- Service: AuthService
- Auth: `ADMIN`
- Duyệt/đổi role sang `TEACHER` hoặc `GVHD`.
- Khi đổi role thành công, `requestedRole` được clear tự động.

## 2.1 Assigned questions (teacher board)

### `POST /questions` (STUDENT hoặc GVHD)
- Service: QuestionService
- Auth: `STUDENT, GVHD`
- Rule mới:
  - `STUDENT` tạo question -> trạng thái `PENDING` như cũ.
  - `GVHD` tạo question -> backend tự set `ApprovedBy = gvhdId`, `AssignedTo = topic.lecturerId`, trạng thái `ASSIGNED`.

Body mẫu:
```json
{
  "title": "Nhờ giải thích use-case đăng ký đề tài",
  "content": "GVHD đăng bài test và auto gán cho giảng viên của topic",
  "topicId": "22222222-2222-2222-2222-222222222222",
  "semesterId": "33333333-3333-3333-3333-333333333333",
  "visibility": "PUBLIC"
}
```

### `GET /questions?assignedTo={teacherUserId}`
- Service: QuestionService
- Auth: required
- Dùng cho màn hình "câu hỏi được giao"

Ví dụ:
```http
GET /questions?assignedTo=11111111-1111-1111-1111-111111111111
Authorization: Bearer <token>
```

---

## 2.2 Aggregated topic thread (NEW)

### `GET /questions/topic-thread?topicId={topicId}&semesterId={semesterId}`
- Service: QuestionService
- Auth: required
- Trả về: `topic` + danh sách `questions[]`, mỗi question kèm `answers[]`
- Mục tiêu: tránh FE phải gọi N+1 (`questions` rồi lặp từng `answers?questionId=`)

Ví dụ:
```http
GET /questions/topic-thread?topicId=22222222-2222-2222-2222-222222222222&semesterId=33333333-3333-3333-3333-333333333333
Authorization: Bearer <token>
```

---

## 2.3 Batch answers by questionIds (NEW)

### `POST /answers/by-questions`
- Service: AnswerService
- Auth: required
- Body:
```json
{
  "questionIds": [
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
  ]
}
```
- Dùng khi FE đã có list question IDs và muốn kéo answers một lần.

---

## 2.4 Topic detail by id (NEW)

### `GET /topics/{id}`
- Service: QuestionService
- Auth: required
- Dùng cho trang detail/edit topic.

### `GET /topics/by-code/{code}`
- Service: QuestionService
- Auth: required
- Dùng business id theo kỳ (vd: `SWPSP26-08`) thay vì GUID.

---

## 2.7 Topic payload chuẩn (NEW fields)

`POST /topics` hiện nhận đầy đủ trường:

```json
{
  "code": "SWPSP26-08",
  "nameEn": "Smart Parking Management System",
  "nameVn": "Hệ thống quản lý bãi đỗ xe thông minh",
  "submittedBy": "MinhNT01",
  "responsibleBy": "MinhNT01",
  "context": "...",
  "problems": "...",
  "actors": "Driver, Parking Staff, Admin",
  "functionalRequirements": "Driver tìm chỗ trống; Đặt chỗ trước; Thanh toán online; Staff quản lý bãi xe",
  "references": null,
  "semesterId": "33333333-3333-3333-3333-333333333333",
  "lecturerId": "90000000-0000-0000-0000-000000000001"
}
```

---

## 2.5 Topic delete (NEW)

### `DELETE /topics/{id}`
- Service: QuestionService
- Auth: `TEACHER, GVHD, ADMIN`
- Rule:
  - `TEACHER`: chỉ xóa topic mình phụ trách.
  - `GVHD/ADMIN`: xóa được mọi topic.
  - Nếu topic đã có question -> backend trả lỗi nghiệp vụ.

---

## 2.6 Mark answered (nên dùng sau khi trả lời)

### `PATCH /questions/{id}/mark-answered`
- Service: QuestionService
- Auth: `TEACHER`
- Chỉ teacher được assign mới được mark.

Ghi chú:
- `POST /answers` hiện đã gọi luồng mark ở backend trước khi lưu answer.
- FE không bắt buộc gọi endpoint này riêng nếu chỉ đi qua `POST /answers`.

---

## 3) Cấu hình FE proxy/env (theo setup hiện tại)

```env
VITE_AUTH_API=/proxy/auth
VITE_QUESTION_API=/proxy/question
VITE_ANSWER_API=/proxy/answer
```

Checklist nhanh:
- Vite `server.proxy` phải map:
  - `/proxy/auth` -> AuthService
  - `/proxy/question` -> QuestionService
  - `/proxy/answer` -> AnswerService
- Mọi request protected phải có bearer token.

---

## 4) Luồng FE call đúng role (chuẩn)

### 4.1 Common bootstrap (mọi role)
1. `POST /auth/login` -> lưu `accessToken`, `userId`, `role`.
2. Gắn `Authorization: Bearer <accessToken>` cho toàn bộ request protected.
3. Màn hình topic list:
  - `GET /semesters?year=&month=` (optional filter)
  - `GET /topics?semesterId=&keyword=`

### 4.2 STUDENT flow
1. Xem topic/thread:
  - `GET /topics`
  - `GET /questions/topic-thread?topicId=...&semesterId=...`
2. Đăng câu hỏi:
  - `POST /questions` (role `STUDENT`) -> status `PENDING`.
3. Refresh thread:
  - `GET /questions/topic-thread?...`

### 4.3 GVHD flow
1. Quản lý học kỳ/topic:
  - `POST /semesters`
  - `POST /topics` (gán `lecturerId` đúng người phụ trách)
  - `PATCH /topics/{id}` (đổi lecturer khi cần)
2. Duyệt câu hỏi sinh viên:
  - `GET /questions?semesterId=...`
  - `PATCH /questions/{id}/approve`
3. Gán giảng viên trả lời:
  - Tự động theo topic: `PATCH /questions/{id}/assign-topic-lecturer`
  - Hoặc chỉ định thủ công: `PATCH /questions/{id}/assign`
4. GVHD đăng câu hỏi test nhanh:
  - `POST /questions` (role `GVHD`) -> auto `ASSIGNED` vào `topic.lecturerId`.

### 4.4 TEACHER flow
1. Lấy câu hỏi được giao:
  - `GET /questions?assignedTo=<teacherUserId>`
2. Xem thread theo topic:
  - `GET /questions/topic-thread?topicId=...&semesterId=...`
3. Trả lời:
  - `POST /answers` (role `TEACHER`)
4. Đồng bộ UI:
  - Reload `GET /questions/topic-thread?...` hoặc `GET /questions?assignedTo=...`

### 4.5 ADMIN flow
1. Duyệt yêu cầu lecturer:
  - `GET /auth/users/pending-role-requests`
  - `PATCH /auth/users/{userId}/role` (set `TEACHER` hoặc `GVHD`)
2. Hỗ trợ quản trị dữ liệu:
  - `DELETE /topics/{id}` (nếu cần, theo rule nghiệp vụ)

### 4.6 Rule FE để không sai role
- Chỉ hiển thị nút theo role từ token:
  - `STUDENT`: tạo question.
  - `GVHD`: approve/assign/create topic/semester.
  - `TEACHER`: create answer.
  - `ADMIN`: duyệt role.
- Không hard-code role ở FE route guard; luôn đọc từ claims/token login mới nhất.
- Sau mỗi action đổi trạng thái (`approve`, `assign`, `answer`) phải reload API nguồn thay vì mutate local state thủ công.

---

## 5) Axios snippets mẫu

```ts
// assigned questions
await apiQuestion.get('/questions', {
  params: { assignedTo: userId }
});

// topic thread
await apiQuestion.get('/questions/topic-thread', {
  params: { topicId, semesterId }
});

// batch answers (nếu chưa dùng topic-thread)
await apiAnswer.post('/answers/by-questions', {
  questionIds
});
```

---

## 6) Postman flow chạy thật (không seed data giả)

Import file:
- `SWPQA_Seed_And_RoleFlow.postman_collection.json`

Run theo thứ tự folder:
1. `01 - Lecturer Registration + Role Approval`
2. `02 - Create Semesters/Topics via APIs`
3. `03 - Ask / Approve / Assign / Answer`

Collection này tự lưu token/id qua `pm.collectionVariables` để chạy liên tục bằng Postman Runner. Hệ thống không còn auto-seed 10 topics/sample question trong startup.
