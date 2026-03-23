# API FE Integration Detail (SWP Q&A)

Tài liệu này dành cho Frontend (React + Vite + shadcn) để call API nhanh, đúng field, đúng role, đúng cổng.

## 1) Service map + Port

## 1.1 Chạy từng service độc lập (launchSettings)

- AuthService:
  - HTTP: `http://localhost:5001`
  - HTTPS: `https://localhost:7096`
- QuestionService:
  - HTTP: `http://localhost:5028`
  - HTTPS: `https://localhost:7058`
- AnswerService:
  - HTTP: `http://localhost:5260`
  - HTTPS: `https://localhost:7145`

## 1.2 Chạy bằng AppHost (Aspire)

- Port có thể thay đổi theo lần chạy.
- Lấy URL thực tế trong Aspire dashboard resource của từng service.
- Khuyến nghị FE dùng `.env`:

```env
VITE_AUTH_API=http://localhost:5001
VITE_QUESTION_API=http://localhost:5028
VITE_ANSWER_API=http://localhost:5260
```

---

## 2) AuthService (`/auth`)

## 2.1 `POST /auth/register`

- Auth: Public
- Body:
  - `email: string`
  - `password: string`
- Response 200:
  - `accessToken: string`
  - `userId: guid`
  - `email: string`
  - `role: ADMIN | STUDENT | GVHD | TEACHER`

## 2.2 `POST /auth/login`

- Auth: Public
- Body:
  - `email: string`
  - `password: string`
- Response 200: giống register

## 2.3 `GET /auth/students`

- Auth: `GVHD, ADMIN`
- Response 200: `UserResponse[]`
  - `id: guid`
  - `email: string`
  - `role: string`

## 2.4 `GET /auth/teachers`

- Auth: `GVHD, ADMIN`
- Dùng để lấy `lecturerId` khi tạo topic.
- Response 200: `UserResponse[]`
  - `id: guid`
  - `email: string`
  - `role: string` (TEACHER)

## 2.5 `PATCH /auth/users/{userId}/role`

- Auth: `ADMIN`
- Body:
  - `role: ADMIN | STUDENT | GVHD | TEACHER`
- Response 200: `UserResponse`

---

## 3) QuestionService - Semester (`/semesters`)

## 3.1 `POST /semesters`

- Auth: `GVHD, ADMIN`
- Body:
  - `name: string` (VD: `SPRING`, `FALL`)
  - `year: number` (2000-2100)
  - `month: number` (1-12)
- Response 200: `SemesterResponse`
  - `id: guid`
  - `name: string`
  - `year: number`
  - `month: number`
  - `createdAt: datetime`

## 3.2 `GET /semesters?name=&year=&month=`

- Auth: Bất kỳ user đã login
- Query (optional):
  - `name: string`
  - `year: number`
  - `month: number`
- Response 200: `SemesterResponse[]`

---

## 4) QuestionService - Topic (`/topics`)

## 4.1 `POST /topics`

- Auth: `GVHD, ADMIN`
- Body:
  - `name: string`
  - `semesterId: guid`
  - `lecturerId: guid` (teacher phụ trách topic)
- Response 200: `TopicResponse`
  - `id: guid`
  - `name: string`
  - `semesterId: guid`
  - `lecturerId: guid`
  - `semesterName: string`
  - `year: number`
  - `month: number`
  - `createdAt: datetime`

## 4.2 `GET /topics?semesterId=&lecturerId=&year=&month=&keyword=`

- Auth: Bất kỳ user đã login
- Query (optional):
  - `semesterId: guid`
  - `lecturerId: guid`
  - `year: number`
  - `month: number`
  - `keyword: string` (search theo tên topic)
- Response 200: `TopicResponse[]`

## 4.3 `GET /topics/my`

- Auth: `TEACHER`
- Response 200: `TopicResponse[]` của chính giảng viên đăng nhập.

## 4.4 `PATCH /topics/{id}`

- Auth: `TEACHER, GVHD, ADMIN`
- Quy tắc:
  - `TEACHER`: chỉ update topic mình phụ trách, không được reassign lecturer.
  - `GVHD/ADMIN`: được đổi tên và đổi lecturer.
- Body:
  - `name?: string`
  - `lecturerId?: guid`
- Response 200: `TopicResponse`

---

## 5) QuestionService - Question (`/questions`)

## 5.1 `POST /questions`

- Auth: `STUDENT`
- Body:
  - `title: string`
  - `content: string`
  - `topicId: guid`
  - `semesterId: guid`
  - `visibility: PUBLIC | GROUP`
- Logic backend:
  - Validate `topicId` thuộc `semesterId`.
  - Tự gán `assignedTo = topic.lecturerId`.
  - Trạng thái ban đầu: `ASSIGNED`.
- Response 200: `QuestionResponse`
  - `id: guid`
  - `title: string`
  - `content: string`
  - `askedBy: guid`
  - `topicId: guid`
  - `semesterId: guid`
  - `visibility: PUBLIC | GROUP`
  - `status: PENDING | APPROVED | ASSIGNED | ANSWERED`
  - `approvedBy: guid | null`
  - `assignedTo: guid | null`
  - `createdAt: datetime`

## 5.2 `GET /questions?topicId=&semesterId=&assignedTo=&visibility=&year=&month=`

- Auth: Bất kỳ user đã login
- Query (optional):
  - `topicId: guid`
  - `semesterId: guid`
  - `assignedTo: guid` (lọc theo lecturer phụ trách)
  - `visibility: PUBLIC | GROUP`
  - `year: number`
  - `month: number`
- Response 200: `QuestionResponse[]`

## 5.3 `GET /questions/{id}`

- Auth: Bất kỳ user đã login
- Response 200: `QuestionResponse`

## 5.4 `PATCH /questions/{id}/approve`

- Auth: `GVHD`
- Response 200: `QuestionResponse`
- Note: dùng cho luồng cũ/manual workflow.

## 5.5 `PATCH /questions/{id}/assign`

- Auth: `GVHD`
- Body:
  - `teacherId: guid`
- Response 200: `QuestionResponse`
- Note: dùng khi cần reassign ngoài luồng topic-owner.

## 5.6 `PATCH /questions/{id}/mark-answered`

- Auth: `TEACHER`
- Rule: chỉ giảng viên được `assignedTo` mới mark được.
- Response 200: `QuestionResponse`

---

## 6) AnswerService (`/answers`)

## 6.1 `POST /answers`

- Auth: `TEACHER`
- Body:
  - `questionId: guid`
  - `content: string`
- Logic backend:
  - Gọi `QuestionService` để `mark-answered` trước.
  - Chỉ success nếu đúng teacher được assign.
- Response 200: `AnswerResponse`
  - `id: guid`
  - `questionId: guid`
  - `teacherId: guid`
  - `content: string`
  - `createdAt: datetime`

## 6.2 `GET /answers?questionId=`

- Auth: Bất kỳ user đã login
- Query:
  - `questionId: guid` (required)
- Response 200: `AnswerResponse[]`

---

## 7) Mapping UI -> API khuyến nghị

1. Topic Management page:
   - Load teachers: `GET /auth/teachers`
   - Load semesters: `GET /semesters`
   - Create topic: `POST /topics`
2. Student ask page:
   - Load topics theo semester: `GET /topics?semesterId=...`
   - Create question: `POST /questions`
3. Lecturer dashboard:
   - Load my topics: `GET /topics/my`
   - Load my questions: `GET /questions?assignedTo=<myUserId>`
   - Answer: `POST /answers`
4. GVHD dashboard:
   - Create semester/topic, reassign topic lecturer (`PATCH /topics/{id}`)
   - Optional legacy approve/assign question.

---

## 8) Lưu ý triển khai FE

- Luôn gửi `Authorization: Bearer <token>` với endpoint protected.
- Parse `role` từ login response để route theo quyền.
- Dùng `userId` từ login response để lọc `assignedTo` cho dashboard giảng viên.
- Có thể ưu tiên chạy qua AppHost; khi đó update `.env` theo URL thực trong dashboard.
