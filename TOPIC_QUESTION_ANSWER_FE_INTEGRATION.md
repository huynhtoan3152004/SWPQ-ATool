# Topic - Question - Answer Integration Guide (Frontend)

## 1) Mục tiêu nghiệp vụ

Flow cần triển khai:
1. Người dùng chọn kỳ (semester).
2. Hệ thống hiển thị danh sách topic của kỳ đó.
3. Người dùng nhấp vào một topic.
4. Hệ thống hiển thị toàn bộ câu hỏi và câu trả lời của topic đó để quản lý.

---

## 2) API cần dùng

### 2.1 Lấy danh sách topic theo kỳ

**Endpoint**
- `GET /topics?semesterId={semesterId}`

**Auth**
- Required (`Bearer token`)

**Response 200**
```json
[
  {
    "id": "22222222-2222-2222-2222-222222222222",
    "name": "SWP Architecture",
    "semesterId": "33333333-3333-3333-3333-333333333333",
    "lecturerId": "11111111-1111-1111-1111-111111111111",
    "semesterName": "Spring",
    "year": 2026,
    "month": 3,
    "createdAt": "2026-03-01T08:00:00Z"
  }
]
```

---

### 2.2 Lấy toàn bộ câu hỏi + câu trả lời theo topic trong kỳ (API tổng hợp cho FE)

**Endpoint (NEW)**
- `GET /questions/topic-thread?topicId={topicId}&semesterId={semesterId}`

**Auth**
- Required (`Bearer token`)

**Response 200**
```json
{
  "topic": {
    "id": "22222222-2222-2222-2222-222222222222",
    "name": "SWP Architecture",
    "semesterId": "33333333-3333-3333-3333-333333333333",
    "lecturerId": "11111111-1111-1111-1111-111111111111",
    "semesterName": "Spring",
    "year": 2026,
    "month": 3,
    "createdAt": "2026-03-01T08:00:00Z"
  },
  "questions": [
    {
      "question": {
        "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
        "title": "How to deploy?",
        "content": "Need deployment guideline",
        "askedBy": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
        "topicId": "22222222-2222-2222-2222-222222222222",
        "semesterId": "33333333-3333-3333-3333-333333333333",
        "visibility": "PUBLIC",
        "status": "ANSWERED",
        "approvedBy": "cccccccc-cccc-cccc-cccc-cccccccccccc",
        "assignedTo": "11111111-1111-1111-1111-111111111111",
        "createdAt": "2026-03-05T02:30:00Z"
      },
      "answers": [
        {
          "id": "dddddddd-dddd-dddd-dddd-dddddddddddd",
          "questionId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
          "teacherId": "11111111-1111-1111-1111-111111111111",
          "content": "Use AppHost + postgres reference...",
          "createdAt": "2026-03-05T04:00:00Z"
        }
      ]
    }
  ]
}
```

**Response 400**
```json
{ "message": "topicId and semesterId are required." }
```

**Response 404**
- Topic không thuộc kỳ tương ứng hoặc không tồn tại.

---

## 3) API quản lý câu hỏi / trả lời liên quan

### 3.1 Tạo câu hỏi
- `POST /questions`
- Role: `STUDENT`

### 3.2 Duyệt câu hỏi
- `PATCH /questions/{id}/approve`
- Role: `GVHD`

### 3.3 Phân công câu hỏi
- `PATCH /questions/{id}/assign`
- Role: `GVHD`

### 3.4 Đánh dấu đã trả lời
- `PATCH /questions/{id}/mark-answered`
- Role: `TEACHER`

### 3.5 Tạo câu trả lời
- `POST /answers`
- Role: `TEACHER`

---

## 4) FE orchestration đề xuất

## Screen A: Semester -> Topics
1. Gọi `GET /topics?semesterId={selectedSemesterId}`.
2. Render danh sách topic.
3. Khi user click topic: lưu `topicId` + `semesterId` và điều hướng sang màn B.

## Screen B: Topic Detail (Question Management)
1. Gọi `GET /questions/topic-thread?topicId={topicId}&semesterId={semesterId}`.
2. Dùng `response.topic` để render header.
3. Dùng `response.questions[]` để render list (mỗi item gồm `question` + `answers[]`).
4. Sau các hành động (approve/assign/answer), reload lại endpoint `topic-thread` để đồng bộ state.

---

## 5) Trạng thái nghiệp vụ cần map UI

`question.status`:
- `PENDING`: chờ duyệt
- `APPROVED`: đã duyệt
- `ASSIGNED`: đã phân công
- `ANSWERED`: đã có trả lời

Gợi ý UX:
- Chỉ hiển thị action theo role + status hợp lệ.
- Disable button khi request đang chạy.
- Sau khi thành công, toast + refresh `topic-thread`.

---

## 6) Lưu ý kỹ thuật quan trọng

1. Tất cả endpoint trên đều cần `Authorization: Bearer <token>`.
2. `topicId` và `semesterId` phải đồng nhất (topic thuộc semester được chọn).
3. API `topic-thread` đã tối ưu để FE không phải gọi N+1 cho answers từng question.
4. Nếu không có answers, `answers` sẽ là mảng rỗng (`[]`), không phải `null`.
