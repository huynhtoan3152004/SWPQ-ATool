# FE Role-Based Flow (Student/GVHD/Topic Lecturer)

Tài liệu này mô tả đúng luồng mới:
- Student đăng nhập -> lọc topic -> hỏi theo topic.
- Câu hỏi vào trạng thái chờ GVHD.
- GVHD duyệt và có thể chuyển cho giảng viên phụ trách topic.
- Topic Lecturer trả lời.

---

## 1) Vai trò và màn hình

## 1.1 Student
1. Login.
2. Trang hỏi đáp:
   - Bộ lọc semester/year/month/keyword.
   - Danh sách topic theo filter.
3. Click topic -> form đặt câu hỏi theo topic.
4. Theo dõi danh sách câu hỏi + trả lời của topic.

## 1.2 GVHD
1. Login.
2. Màn danh sách câu hỏi chờ xử lý (`PENDING`).
3. Duyệt câu hỏi (`APPROVED`).
4. Chuyển câu hỏi cho giảng viên phụ trách topic (`ASSIGNED`).

## 1.3 Topic Lecturer (TEACHER)
1. Login.
2. Xem câu hỏi được giao (`assignedTo = myUserId`, status `ASSIGNED`).
3. Trả lời câu hỏi (`POST /answers`) -> hệ thống tự mark answered.

---

## 2) API call theo từng bước

## 2.1 Student - login + lọc topic
- `POST /auth/login`
- `GET /semesters?year=&month=&name=`
- `GET /topics?semesterId=&year=&month=&keyword=`

## 2.2 Student - hỏi theo topic
- `POST /questions`

Body:
```json
{
  "title": "...",
  "content": "...",
  "topicId": "guid",
  "semesterId": "guid",
  "visibility": "PUBLIC"
}
```

Luồng mới sau khi tạo:
- `status = PENDING`
- `assignedTo = null`

## 2.3 Student/GVHD/Teacher - xem thread topic
- `GET /questions/topic-thread?topicId={topicId}&semesterId={semesterId}`

## 2.4 GVHD - duyệt và chuyển
- `PATCH /questions/{id}/approve`
- `PATCH /questions/{id}/assign-topic-lecturer`

Hoặc chuyển thủ công:
- `PATCH /questions/{id}/assign`
- body `{ "teacherId": "..." }`

## 2.5 Teacher - trả lời
- `POST /answers`

Body:
```json
{
  "questionId": "guid",
  "content": "..."
}
```

---

## 3) FE chỉ nên lấy field nào (tối ưu UX)

## 3.1 Topic list item
Từ `TopicResponse` chỉ dùng:
- `id`
- `name`
- `semesterName`
- `year`
- `month`

Ẩn khỏi UI (nhưng vẫn giữ trong state khi cần):
- `lecturerId`
- `createdAt`

## 3.2 Question card
Từ `QuestionResponse` dùng:
- `id`
- `title`
- `content`
- `status`
- `createdAt`

Chỉ dùng nội bộ (không cần hiện user):
- `topicId`
- `semesterId`
- `askedBy`
- `approvedBy`
- `assignedTo`

## 3.3 Answer item
Từ `TopicAnswerResponse` dùng:
- `id`
- `content`
- `createdAt`

Ẩn khỏi UI nếu không cần:
- `teacherId`
- `questionId`

---

## 4) Mapping status cho FE

- `PENDING`: chờ GVHD duyệt.
- `APPROVED`: GVHD đã duyệt, chưa giao lecturer.
- `ASSIGNED`: đã giao cho lecturer phụ trách topic.
- `ANSWERED`: đã có câu trả lời.

Gợi ý button theo role:
- Student: chỉ tạo câu hỏi, xem thread.
- GVHD: `Approve`, `Assign to Topic Lecturer`.
- Teacher: `Answer` khi status `ASSIGNED` và assignedTo là chính mình.

---

## 5) Proxy/env FE

```env
VITE_AUTH_API=/proxy/auth
VITE_QUESTION_API=/proxy/question
VITE_ANSWER_API=/proxy/answer
```

---

## 6) Checklist nếu FE còn báo lỗi

1. Có gửi `Authorization: Bearer <token>` cho endpoint protected.
2. `topicId`, `semesterId`, `questionId` là GUID hợp lệ.
3. Không gọi endpoint cũ kiểu `/questions/assigned`.
4. Dùng đúng endpoint mới: `/questions/topic-thread`, `/questions/{id}/assign-topic-lecturer`.
