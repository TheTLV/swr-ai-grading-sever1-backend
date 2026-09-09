# Lịch sử các quyết định (Decision Log)

> Tài liệu này ghi lại **mọi quyết định quan trọng** đã được đưa ra trong quá trình lên kế hoạch và phát triển. Mỗi quyết định có ID, ngày, nội dung, lý do và hệ quả.

---

## Quyết định đã đưa ra

### D-001 | 2026-09-09 | Chọn .NET thay vì Spring Boot
**Quyết định:** Dùng ASP.NET Core (C#) cho backend thay vì Spring Boot (Java) như gợi ý trong đề bài.

**Lý do:** User đã chọn .NET (theo phản hồi trong chat đầu tiên với AI).

**Hệ quả:** Kiến trúc, library, ORM sẽ dùng stack .NET. Cần tự thiết kế nhiều hơn thay vì dùng templates Spring Boot có sẵn.

**Status:** ✅ Đã quyết định

---

### D-002 | 2026-09-09 | Bổ sung vai trò "Khảo thí" (Examination Office)
**Quyết định:** Bổ sung vai trò "Khảo thí" (Examination Office) ngoài Admin và Lecturer như trong đề bài.

**Lý do:** Trong thực tế FPT, việc tạo semester/subject/subject offering thường do phòng khảo thí thực hiện, không phải giảng viên. Lecturer chỉ phụ trách upload đề và chấm bài.

**Hệ quả:** Cần thêm role "ExaminationOffice" trong hệ thống. Cần xác nhận lại với GVHD.

**Status:** ⏳ Chờ xác nhận GVHD

---

### D-003 | 2026-09-09 | Kiến trúc 2 Server bắt buộc
**Quyết định:** Hệ thống phải chia thành 2 server vật lý/logic riêng biệt:
- Server 1: Giảng viên/Khảo thí (có Internet, chạy AI)
- Server 2: Server thi (không có Internet)

**Lý do:** Yêu cầu bảo mật phòng thi — server thi không được kết nối Internet để tránh gian lận.

**Hệ quả:**
- Server 2 cần design sao cho offline hoàn toàn
- AI grading chỉ chạy trên Server 1
- Package transfer giữa 2 server cần kiến trúc riêng

**Status:** ✅ Đã quyết định (từ bối cảnh đồ án)

---

### D-004 | 2026-09-09 | AI Service tách riêng (Python)
**Quyết định:** Phần AI/CV (Extract, OCR, Grading) chạy trên **Python service riêng**, Server 1 gọi qua HTTP API.

**Lý do:** Hệ sinh thái Python phù hợp hơn cho xử lý document/computer vision (PaddleOCR, OpenCV, YOLO, LangChain…). Gọi từ C# qua HTTP đơn giản và linh hoạt.

**Hệ quả:**
- Server 1 định nghĩa interface `IAIService` / `IPythonService`
- Python service implement riêng
- Trong giai đoạn dev có thể mock/stub
- Cần thiết kế API contract giữa C# và Python

**Status:** ✅ Đã quyết định

---

### D-005 | 2026-09-09 | Clean Architecture cho Server 1
**Quyết định:** Áp dụng Clean Architecture (onion/ports-and-adapters) cho Server 1.

**Cấu trúc:**
```
src/
├── Domain/        # Entities, Enums, ValueObjects, Interfaces (no dependencies)
├── Application/   # DTOs, Services, UseCases, Validators (references Domain)
├── Infrastructure/# Persistence, Storage, External Services (references Application)
└── WebAPI/        # Controllers, Middleware, Extensions (references Application)
```

**Lý do:**
- Phù hợp cho dự án lớn (Capstone)
- Dễ test, dễ mở rộng
- Tách biệt rõ ràng giữa business logic và infrastructure

**Hệ quả:** Cần setup project với 4 projects con trong solution.

**Status:** ✅ Đã quyết định

---

### D-006 | 2026-09-09 | Database = PostgreSQL (Supabase hoặc trực tiếp)
**Quyết định:** Sử dụng **PostgreSQL** làm database cho Server 1.

**Lựa chọn cụ thể:** Chưa chọn giữa:
- A) PostgreSQL trực tiếp (self-hosted)
- B) Supabase (PostgreSQL + Auth + Storage)
- C) Supabase (PostgreSQL + Storage, tự implement Auth)

**Lý do:** Đề bài gợi ý PostgreSQL. Supabase cung cấp thêm Auth và Storage tiện lợi.

**Hệ quả:** Cần quyết định trước khi setup.

**Status:** ⏳ Chưa quyết định — cần user xác nhận

---

### D-007 | 2026-09-09 | File Storage = Supabase Storage hoặc Local
**Quyết định:** Chưa quyết định — 2 lựa chọn:
- A) Supabase Storage (cloud, tiện)
- B) Local filesystem (đơn giản, dev nhanh)

**Lý do:** File đề thi (docx) và bài nộp cần lưu trữ. Supabase Storage tiện hơn nhưng phụ thuộc Internet. Local filesystem đơn giản hơn cho dev.

**Hệ quả:** Ảnh hưởng đến Infrastructure layer implementation.

**Status:** ⏳ Chưa quyết định — cần user xác nhận

---

### D-008 | 2026-09-09 | Tên project tạm thời
**Quyết định:** Đặt tên tạm thời cho backend repo là `Backend_Server1`.

**Lý do:** User chưa cung cấp tên chính thức cho dự án.

**Hệ quả:** Cần đổi tên thành tên chính thức (ví dụ: `SwrGrading.API`, `SWR30x-Grading-System`, v.v.)

**Status:** ⏳ Tên tạm — cần user cung cấp tên chính thức

---

### D-009 | 2026-09-09 | Flow 1 = Register → Package Transfer
**Quyết định:** Flow 1 (phạm vi công việc hiện tại) bao gồm:
```
Register → Login → Auth → Semester → Subject → SubjectOffering →
Exam → Assign Lecturer → Upload Files → Submit → Review →
Lock → Package → Transfer to Server 2
```

**Lý do:** Đây là phần đầu của quy trình tổng thể. AI grading là flow sau.

**Hệ quả:** Tập trung hoàn thành phần này trước, sau đó mới sang Extract/AI Grading.

**Status:** ✅ Đã quyết định

---

### D-010 | 2026-09-09 | State Machine cho Exam (cũ)
**Quyết định:** Exam có các trạng thái:
```
Draft → Submitted → UnderReview → Locked → Packaged → Transferred
```

**Lý do:** Phản ánh đúng nghiệp vụ: Lecturer tạo đề (Draft) → Nộp (Submitted) → Khảo thí duyệt (UnderReview) → Khoá (Locked) → Đóng gói (Packaged) → Gửi sang Server 2 (Transferred).

**Hệ quả:**
- Cần validate state transition (không cho sửa khi đã Locked)
- Mỗi transition có audit log riêng
- Unlock có thể cho phép hoặc không — cần xác nhận thêm

**Status:** ⏳ ĐÃ THAY THẾ bởi D-011 (xem bên dưới)

---

### D-011 | 2026-09-09 | ExamSubmissionStatus mới (có AI Validation)
**Quyết định:** Thay thế `ExaminationStatus` cũ bằng `ExamSubmissionStatus` mới:
```
Draft → Submitted → AI_Validated → Under_Review →
  ├─ Approved → Locked → Packaged → Transferred
  └─ Rejected → (Lecturer sửa + submit lại → Submitted)
```

**Lý do:** User giải thích rõ ràng nghiệp vụ:
1. Lecturer upload + submit đề
2. AI tự kiểm tra rubric (tổng điểm = 10? format đúng không?)
3. Khảo thí xem lại (lần 2 — vì sau khi Lock sẽ mã hóa, Server 2 không mở được)
4. Lock → Nén + Mã hóa → Gửi Server 2

**AI Validation kiểm tra:**
- Tổng điểm rubric = 10?
- Đề đúng số câu/mục như rubric?
- File đọc được không?
- Answer template match với đề?

**Hệ quả:**
- Phải tạo enum `ExamSubmissionStatus` mới trong code
- Bỏ `ExaminationStatus` (hoặc giữ lại cho Exam session ở Server 2)
- Bước `AI_Validated` là tự động — không cần human action

**Status:** ✅ Đã quyết định (ghi nhận trong `doc/00-brief/enums-and-statuses.md`)

---

### D-012 | 2026-09-09 | Không có bảng `users` chung — 3 bảng riêng
**Quyết định:** Hệ thống **không có bảng `users` chung**, mà chia thành 3 bảng riêng:
- `students` — thông tin sinh viên
- `lecturers` — thông tin giảng viên
- `examination_offices` — thông tin nhân viên phòng khảo thí

**Lý do:** User xác nhận đây là cấu trúc DB thực tế của team (không phải 1 bảng users + enum role).

**Hệ quả:**
- Entity `User` trong code sẽ bị xoá/thay thế
- Auth/Seeder phải sửa lại cho 3 bảng
- API permission phân biệt theo role
- **Hiện tại: CHƯA SỬA** — ghi nhận, sẽ refactor sau

**Cần hỏi thêm:**
- [ ] Có bảng `accounts` chung cho username/password không?
- [ ] Student có thuộc lớp (class) không?
- [ ] Lecturer có thuộc khoa/phòng ban không?

**Status:** ✅ Ghi nhận — **CHƯA IMPLEMENT**

---

### D-013 | 2026-09-09 | Semester Code Format — FPT Academic Style
**Quyết định:** Mã kỳ học theo format FPT:
```
{Block}{Year2Digits}   // VD: FA26, SP27, SU26
```

| Block | Ý nghĩa | Tháng | Độ dài |
|---|---|---|---|
| `FA` | Fall | 09 | ~10 tuần |
| `SP` | Spring | 01 | ~10 tuần |
| `SU` | Summer | 06 | ~5 tuần |

**Validation:**
- Format regex: `^(FA|SP|SU)\d{2}$`
- Không phân biệt hoa/thường (lưu uppercase)
- Năm trong khoảng: 2025 → 2035
- Code UNIQUE

**Hệ quả:**
- Semester entity cần field `Code` (4 ký tự)
- Validation kiểm tra ngày hợp lệ:
  - FA: startDate trong [08-01, 09-30]
  - SP: startDate trong [12-01, 01-31]
  - SU: startDate trong [05-01, 06-30]
  - EndDate - StartDate: FA/SP ≈ 70-100 ngày, SU ≈ 35-50 ngày

**Status:** ✅ Đã quyết định

---

## Quyết định đang chờ (Pending)

| ID | Chủ đề | Các lựa chọn | Cần gì |
|---|---|---|---|
| P-001 | Tên chính thức dự án | User cung cấp | User input |
| P-002 | Database hosting | PostgreSQL vs Supabase | User input |
| P-003 | File storage | Local vs Supabase Storage | User input |
| P-004 | Xác nhận role "Khảo thí" với GVHD | Có/Không | User hỏi GVHD |
| P-005 | Câu hỏi nghiệp vụ Q-01 → Q-10 | Xem requirements.md section 6 | User trả lời |
| P-006 | User table refactor | 3 bảng Student/Lecturer/ExaminationOffice + accounts chung? | User xác nhận |
| P-007 | Student có lớp (class) không? | Có/Không | User xác nhận |
| P-008 | Lecturer có thuộc khoa/phòng ban? | Có/Không | User xác nhận |
| P-009 | AI_Validated có tách thành 2 trạng thái con? | AI_OK / AI_Warning | User xác nhận |
| P-010 | Locked có cho phép Unlock? | Có/Không | User xác nhận |
| P-011 | Rejected → sửa submission cũ hay tạo mới? | Sửa cũ / Tạo mới | User xác nhận |

---

## Mẫu ghi nhận quyết định mới

Khi có quyết định mới, thêm vào đây theo format:

```markdown
### D-XXX | YYYY-MM-DD | [Tiêu đề ngắn]
**Quyết định:** [Mô tả quyết định]
**Lý do:** [Tại sao chọn như vậy]
**Hệ quả:** [Ảnh hưởng gì đến design/code]
**Status:** ✅/⏳ Đã quyết định / Chờ xác nhận
```
