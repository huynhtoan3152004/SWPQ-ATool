# FRONTEND UI FLOW (React + Vite + shadcn)

Tài liệu này để bạn đưa cho AI dựng nhanh giao diện MVP cho hệ thống SWP Q&A microservices.

## 1) Luồng domain chốt

1. GVHD/ADMIN tạo `Semester` theo kỳ + năm + tháng.
2. GVHD/ADMIN tạo `Topic` thuộc `Semester` và gán `lecturerId`.
3. Student chọn topic để đặt câu hỏi.
4. Câu hỏi tự động gán cho lecturer của topic (không cần assign tay).
5. Đúng lecturer đó trả lời và quản lý câu hỏi theo topic.

## 2) API cần FE gọi

## 2.1 Auth

- `POST /auth/login`
- `POST /auth/register`
- `GET /auth/students` (GVHD, ADMIN)
- `GET /auth/teachers` (GVHD, ADMIN)
- `PATCH /auth/users/{userId}/role` (ADMIN)

## 2.2 Semester (QuestionService)

- `POST /semesters` (GVHD, ADMIN)
  - body: `{ name, year, month }`
- `GET /semesters?name=&year=&month=`

## 2.3 Topic (QuestionService)

- `POST /topics` (GVHD, ADMIN)
  - body: `{ name, semesterId, lecturerId }`
- `GET /topics?semesterId=&lecturerId=&year=&month=&keyword=`
- `GET /topics/my` (TEACHER)
- `PATCH /topics/{id}` (TEACHER/GVHD/ADMIN)
  - body: `{ name?, lecturerId? }`

## 2.4 Question

- `POST /questions` (STUDENT)
  - body: `{ title, content, topicId, semesterId, visibility }`
- `GET /questions?topicId=&semesterId=&assignedTo=&visibility=&year=&month=`
- `GET /questions/{id}`
- `PATCH /questions/{id}/approve` (GVHD)
- `PATCH /questions/{id}/assign` (GVHD)

## 2.5 Answer

- `POST /answers` (TEACHER)
- `GET /answers?questionId=`

## 3) FE gọi nhiều service bằng cổng nào?

Không hard-code port. Dùng `.env`:

```env
VITE_AUTH_API=http://localhost:5101
VITE_QUESTION_API=http://localhost:5102
VITE_ANSWER_API=http://localhost:5103
```

Khi chạy Aspire, xem port thực tế trong dashboard rồi cập nhật env.

## 4) UI pages cần có (MVP)

1. Login Page.
2. Semester/Topic Management (GVHD/ADMIN, có gán lecturer).
3. Question List + Filter (topic/semester/year/month/visibility).
4. Create Question Form (Student).
5. Lecturer Question Board (lọc theo `assignedTo` hoặc `topics/my`).
6. Teacher Answer Form.
7. Admin Role Management.

## 5) Prompt ngắn để đưa AI build FE

```txt
Build a React + Vite + shadcn/ui MVP frontend for a SWP Q&A microservices app.

Requirements:
1) Implement pages: Login, Semester/Topic Management, Question List, Create Question, Lecturer question board, Teacher answer, Admin role management.
2) Use role-based routing: STUDENT, GVHD, TEACHER, ADMIN.
3) Use env base URLs:
   - VITE_AUTH_API
   - VITE_QUESTION_API
   - VITE_ANSWER_API
4) Implement API calls:
  - Auth: POST /auth/login, POST /auth/register, GET /auth/students, GET /auth/teachers, PATCH /auth/users/{userId}/role
   - Semester: POST /semesters, GET /semesters
  - Topic: POST /topics, GET /topics, GET /topics/my, PATCH /topics/{id}
  - Questions: POST /questions, GET /questions, GET /questions/{id}, PATCH /questions/{id}/approve, PATCH /questions/{id}/assign
   - Answers: POST /answers, GET /answers?questionId=
5) Add filter UI for questions/topics by semester/year/month.
6) Keep UI simple and clean, no extra features.
7) Add reusable API client with JWT Bearer token and 401 handling.
```
