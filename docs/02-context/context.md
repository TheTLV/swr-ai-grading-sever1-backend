# Bối cảnh tổng thể đồ án

> **Tài liệu này ghi lại bức tranh toàn cảnh của đồ án để mọi thành viên trong nhóm và AI assistant đều cùng hiểu chung, tránh phải đọc lại file Word đề bài nhiều lần.**

---

## 1. Tên đồ án

- **Tiếng Anh:** Develop an AI-assisted system to support the grading of the Software Requirements (SWR30x) practical exam.
- **Tiếng Việt:** Xây dựng hệ thống hỗ trợ chấm thi thực hành môn Software Requirements (SWR30x) có hỗ trợ từ AI.

- **Mã đề tài:** FA26SE034
- **GVHD:** Thân Thị Ngọc Vân — `vanttn@fpt.edu.vn`
- **Thời gian:** 09/2026 → 03/2027
- **Chuyên ngành:** Software Engineer (SE)

---

## 2. Vấn đề cần giải quyết

Môn SWR (SWR30x) yêu cầu sinh viên tạo ra nhiều artifact: stakeholder analysis, user stories, use case, use case specification, business rules, acceptance criteria, product backlog, sprint plan, UML diagrams…

Bài thi thực hành (PE) thường là **file Word (.docx)** chứa **text + table + diagram + image**. Hiện tại:

- Giảng viên phải mở hàng trăm file thủ công
- Phải đọc, đối chiếu rubric, chấm thủ công → tốn thời gian, khó đảm bảo tính nhất quán
- LMS hiện tại không có khả năng chấm tự động cho artifact SE

AI (LLM, OCR, document understanding) mở ra cơ hội tự động hoá cả việc tạo artifact lẫn chấm bài.

---

## 3. Kiến trúc 2 Server

Đây là **đặc điểm rất quan trọng** của đồ án, quyết định ranh giới code.

> **Server phục vụ thi KHÔNG được kết nối Internet** (yêu cầu bảo mật/phòng thi).
> Do đó hệ thống được tách thành 2 server vật lý/Logic độc lập.

### Sơ đồ

```
                          INTERNET
                             │
                             ▼
┌──────────────────────────────────────────────────────────────┐
│ SERVER 1 — SERVER GIẢNG VIÊN / KHẢO THÍ (bạn phụ trách)     │
│                                                              │
│   Register/Login → Authn/Authz                               │
│        ↓                                                     │
│   Semester → Subject → Subject Offering → Exam               │
│        ↓                                                     │
│   Lecturer upload đề + Answer Template + Rubric              │
│        ↓                                                     │
│   Validate / Check / Review                                  │
│        ↓                                                     │
│   Submit → Lock → Package                                    │
│        ↓                                                     │
│   Gửi đề thi sang Server 2                                   │
│                                                              │
│   ← Nhận bài làm từ Server 2                                 │
│        ↓                                                     │
│   Extract → AI Grading + Rubric → Kết quả chấm               │
└──────────────────────────────────────────────────────────────┘
                             │
                  Kết nối có kiểm soát (không Internet)
                             │
                             ▼
┌──────────────────────────────────────────────────────────────┐
│ SERVER 2 — SERVER THI (phần khác của nhóm phụ trách)         │
│                                                              │
│   KHÔNG KẾT NỐI INTERNET                                     │
│                                                              │
│   Nhận đề thi từ Server 1                                    │
│        ↓                                                     │
│   Quản lý kỳ thi                                             │
│        ↓                                                     │
│   Student làm bài                                            │
│        ↓                                                     │
│   Anti-cheating                                              │
│        ↓                                                     │
│   Submit bài → Gửi bài về Server 1                           │
└──────────────────────────────────────────────────────────────┘
```

### Phân công

| Server | Vai trò chính | Ai phụ trách |
|--------|--------------|--------------|
| **Server 1** | Giảng viên / Khảo thí — chuẩn bị đề, kiểm tra, đóng gói; nhận bài; extract; AI grading | **BẠN** (backend C#) |
| **Server 2** | Sinh viên thi, anti-cheating, nộp bài | Thành viên khác |

### Đặc điểm kết nối

- Server 2 **không có Internet** → không gọi được LLM API, OCR cloud, v.v.
- Server 1 (có Internet) sẽ gọi AI services khi cần
- Giữa 2 server có **kết nối có kiểm soát** (controlled link) để truyền package đề và bài làm

---

## 4. Phạm vi của bạn — Server 1 (Flow 1)

Flow 1 hiện tại đi từ **đầu vào** đến **đóng gói sẵn sàng chuyển sang Server 2**, cụ thể:

```
Register
   ↓
Login
   ↓
Authentication / Authorization
   ↓
Semester
   ↓
Subject
   ↓
Subject Offering
   ↓
Create Exam
   ↓
Assign Lecturer
   ↓
Lecturer
   ↓
Upload Exam File
   ├── Đề thi / Question
   ├── Answer Template
   └── Rubric
   ↓
Validate
   ↓
Submit
   ↓
Review / Check
   ↓
Lock
   ↓
Package
   ↓
Gửi sang Server 2  ← (điểm dừng của Flow 1)
```

> **Mô tả ngắn:** Flow 1 là quy trình chuẩn bị và kiểm tra đề thi trên Server 1, bắt đầu từ đăng ký/đăng nhập và kết thúc khi đề thi đã được kiểm tra, đóng gói và sẵn sàng gửi sang server thi.

Các flow tiếp theo (sẽ làm sau):
- Server 2: nhận đề → sinh viên thi → anti-cheating → submit bài
- Server 1: nhận bài → Extract → AI Grading → Score

---

## 5. Vai trò của AI trong đồ án

> **AI KHÔNG nằm trong Flow 1.** AI nằm ở giai đoạn chấm bài sau khi bài thi quay về Server 1.

### Vì sao không đơn giản `PDF → OCR → Text → AI chấm`?

Bài SWR chứa nhiều loại nội dung cần tách riêng:
- Text
- Table
- Use Case Diagram
- Sequence Diagram
- Class Diagram
- Hình ảnh
- Quan hệ giữa các thành phần

### Hướng xử lý đúng

```
PDF / Image
     ↓
Document / Layout Analysis
     ↓
┌───────────────┬─────────────────┐
│ TEXT          │ DIAGRAM / IMAGE │
│ ↓             │ ↓               │
│ PaddleOCR     │ YOLO / CV       │
│               │ ↓               │
│               │ Object Detection│
└───────┬───────┴────────┬────────┘
        ↓                ↓
      Text           Objects
        │                │
        └───────┬────────┘
                ↓
       Position / Geometry
                ↓
       Relationship Analysis
                ↓
    Structured Representation
                ↓
          AI Grading
                ↓
             Rubric
                ↓
              Score
```

### Nguyên tắc quan trọng

> **Extract không chấm điểm.** Extract chỉ biến bài làm từ dữ liệu hình ảnh/tài liệu thành dữ liệu có cấu trúc: **có gì, nội dung gì, nằm ở đâu và quan hệ với nhau như thế nào**.
>
> Sau đó **Grading** mới dùng dữ liệu đó để đối chiếu Rubric và quyết định điểm.

---

## 6. Stack công nghệ

### Server 1 (bạn phụ trách)
- **Backend:** ASP.NET Core (C#) — bạn đã chọn thay cho Spring Boot gợi ý trong đề
- **Database:** PostgreSQL (đề xuất Supabase)
- **Storage:** Supabase Storage (đề xuất)
- **API style:** REST + Swagger/OpenAPI
- **Realtime (nếu cần):** SignalR
- **AI/CV:** Tách thành Python service riêng, Server 1 gọi qua HTTP API
  - Lý do: hệ sinh thái Python phù hợp hơn cho xử lý document/CV

### Stack đề xuất (tham khảo từ đề)
Spring Boot | ReactJS | PostgreSQL | OpenAI API hoặc LLM tương đương | OCR Engine (Tesseract / Cloud OCR) | Apache POI | Mermaid/PlantUML | Docker | Redis | Elasticsearch (optional)

> **Lưu ý:** Bạn chọn giữ .NET thay vì Spring Boot — đã được ghi nhận trong `decisions.md`.

---

## 7. Clean Architecture định hướng cho Server 1

```
Server1/
├── src/
│   ├── Domain/           # Entities, Enums, ValueObjects, Interfaces
│   ├── Application/      # DTOs, Interfaces, Services, UseCases, Validators
│   ├── Infrastructure/   # Persistence (DbContext, Configurations, Migrations), Storage, Services
│   └── WebAPI/           # Controllers, Middleware, Extensions, Program.cs
└── tests/
    ├── UnitTests/
    ├── IntegrationTests/
    └── ApiTests/
```

- **Swagger/OpenAPI:** nằm trong `WebAPI` để test API thủ công
- **Automated tests:** UnitTests / IntegrationTests / ApiTests

---

## 8. Glossary (thuật ngữ)

| Thuật ngữ | Ý nghĩa |
|---|---|
| **PE** | Practical Examination — bài thi thực hành |
| **SWR30x** | Môn Software Requirements tại FPTU |
| **Server 1** | Server giảng viên/khảo thí (có Internet, chạy AI) |
| **Server 2** | Server thi (offline) |
| **Subject** | Môn học |
| **Subject Offering** | Môn học mở trong một học kỳ cụ thể |
| **Exam** | Đề/kỳ thi cụ thể của một SubjectOffering |
| **ExamSubmission** | Lần giảng viên nộp bộ đề |
| **ExamFile** | Các file của bộ đề (đề, rubric, answer template) |
| **ExamPackage** | Package đã đóng gói để chuyển sang Server 2 |
| **PackageTransfer** | Bản ghi việc chuyển package giữa 2 server |
| **Extract** | Biến bài làm (ảnh/PDF) → dữ liệu có cấu trúc (text + object + quan hệ) |
| **Grading** | Đối chiếu dữ liệu có cấu trúc với rubric → điểm |

---

## 9. Tài liệu liên quan

- **Đề bài gốc:** `doc/Phieu_FA26SE034.docx`
- **Đề bài dạng text:** `doc/_docx_text.txt`
- **Yêu cầu & phạm vi chi tiết:** `doc/requirements.md`
- **Kiến trúc & DB:** `doc/architecture.md`
- **Kế hoạch & trạng thái:** `doc/plan.md`
- **Lịch sử quyết định:** `doc/decisions.md`
