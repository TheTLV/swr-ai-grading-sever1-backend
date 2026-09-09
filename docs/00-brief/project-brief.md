# 📋 Project Brief — Mô tả dự án từ nhiều góc nhìn

> **Mục đích:** Tài liệu này mô tả lại dự án **từ góc nhìn của các role khác nhau** để chắc chắn AI agent (và mọi người) hiểu đúng bức tranh tổng thể.
>
> **Ghi chú:** File này viết theo lối "đóng vai" — mỗi phần là một góc nhìn khác nhau, không phải đề xuất kỹ thuật mới.

---

## 1. Bức tranh 1 phút (Elevator Pitch)

> **Bạn là sinh viên SE.** Bài thi PE môn SWR (Software Requirements) là 1 file Word có text, bảng, sơ đồ, hình ảnh. Hàng trăm bài như vậy phải chấm tay → giảng viên mệt, không nhất quán.
>
> **Hệ thống này** giúp giảng viên chuẩn bị đề (upload, duyệt, đóng gói), sinh viên làm bài trên một server riêng (offline), bài nộp về được **AI tự động chấm** dựa trên rubric. Kết quả cuối cùng: giảng viên chỉ cần xem lại các ca AI chưa chắc chắn.

**Một dòng:** *AI-assisted grading system cho bài thi thực hành môn Software Requirements (SWR30x).*

---

## 2. Góc nhìn Solution Architect — "Hệ thống là gì?"

### 2.1. Bản chất bài toán

| Khía cạnh | Hiện tại | Mong muốn |
|---|---|---|
| Chuẩn bị đề | Giảng viên tạo file Word, gửi email, copy qua USB... | Web UI, có workflow duyệt, đóng gói tự động |
| Sinh viên thi | Mở file Word, làm bài, nộp lại qua LMS | Phòng thi riêng (offline), có giám sát |
| Chấm bài | Giảng viên mở hàng trăm file Word, đọc, đối chiếu rubric | AI đọc, tách text/diagram, đối chiếu rubric → điểm |
| Phúc khảo | Giảng viên chấm lại bằng tay | Xem lại điểm AI + override thủ công |

### 2.2. Tại sao 2 server?

**Server thi (Server 2) KHÔNG có Internet** vì:
- Chống sinh viên dùng ChatGPT/LLM trong phòng thi
- Chống copy từ Internet
- Bảo mật đề thi

→ Phải tách vật lý/Logic:
```
Server 1 (có Internet)         Server 2 (offline)
├ Giảng viên/Khảo thí          ├ Sinh viên thi
├ Chấm bài bằng AI             ├ Anti-cheating
└ Nhận bài từ Server 2          └ Nộp bài về Server 1
       ↑                                    │
       └──── Controlled link ──────────────┘
              (không phải Internet)
```

### 2.3. Các dòng nghiệp vụ chính

```
Flow 1 — Chuẩn bị & duyệt đề       [Server 1]   ← PHẠM VI BẠN (đợt này)
Flow 2 — Sinh viên thi             [Server 2]   ← Thành viên khác
Flow 3 — Chấm bài bằng AI          [Server 1]   ← Sau
```

### 2.4. Quyết định kiến trúc đã chốt

| Quyết định | Chọn | Lý do |
|---|---|---|
| Backend Server 1 | **ASP.NET Core (C#)** | User chọn thay Spring Boot |
| Database | **PostgreSQL** (Supabase) | Đề gợi ý; Supabase tiện cho dev |
| File storage | **Supabase Storage / Local** | Chưa chốt — chờ user |
| AI/CV module | **Python service riêng** (gọi qua HTTP) | Hệ sinh thái Python mạnh cho OCR/CV |
| Architecture pattern | **Clean Architecture** | Tách lớp rõ, dễ test |
| API style | **REST + Swagger/OpenAPI** | Đơn giản, đủ dùng |
| Auth | **JWT + BCrypt** | Chuẩn .NET |
| **User tables** | **3 bảng riêng** (Student / Lecturer / ExaminationOffice) | User xác nhận — không có bảng users chung |
| **Semester code** | **FPT style** (FA26 / SP27 / SU26) | User xác nhận |
| **Exam workflow** | **Draft→Submitted→AI_Validated→Under_Review→Approved→Locked→Packaged→Transferred** | User giải thích nghiệp vụ |

---

## 3. Góc nhìn Product Owner — "Người dùng là ai, họ cần gì?"

### 3.1. Personas (Ai sẽ dùng hệ thống?)

| Persona | Mục tiêu | Đau đớn hiện tại |
|---|---|---|
| **Admin** (Bạn quản trị hệ thống) | Quản lý user, role, config, xem audit log | Khó tổ chức nhiều role, khó truy vết |
| **Khảo thí / Examination Office** | Tổ chức kỳ thi (semester → subject → exam), phân công giảng viên, duyệt đề | Làm thủ công bằng Excel + email |
| **Lecturer** | Upload đề, answer template, rubric; submit; xem điểm AI; override | Mở hàng trăm file Word, chấm tay |
| **Student** | Login → xem đề → làm bài → nộp → xem điểm | Phải qua nhiều bước rườm rà |

> **Lưu ý:** Vai trò "Khảo thí" là do nhóm bạn **bổ sung** so với đề bài gốc — đề chỉ có Admin/Lecturer/Student. Bổ sung vì trong thực tế FPT, việc tổ chức thi do phòng khảo thí đảm nhận, không phải giảng viên. Cần xác nhận lại GVHD.

### 3.2. Câu chuyện người dùng tiêu biểu

**Lecturer Hương — Upload đề thi:**
> "Tôi vừa soạn xong đề PE cho SWR301. Tôi muốn upload đề, upload answer template, upload rubric, rồi submit để phòng khảo thí duyệt. Nếu bị reject, tôi sửa lại và submit lại."

**Khảo thí Minh — Tổ chức kỳ thi:**
> "Học kỳ Fall 2026 mở môn SWR301. Tôi tạo semester, tạo subject offering, tạo exam, assign giảng viên Hương. Khi Hương submit đề, tôi review. OK thì lock, đóng gói, gửi sang server thi."

**Admin Tuấn — Quản trị:**
> "Có giảng viên mới vào. Tôi tạo account, gán role Lecturer. Audit log có ghi lại thao tác này không?"

### 3.3. Phạm vi ĐỢT NÀY (Flow 1) — Bạn chỉ code phần này

✅ **Trong phạm vi:**
- Register / Login / Logout / JWT
- Quản lý User + Role
- CRUD Semester, Subject, Subject Offering, Exam
- Assign Lecturer
- Upload file đề + answer template + rubric
- Workflow duyệt: Submit → Review → Lock → Package → Transfer
- Audit log
- Swagger UI

❌ **Chưa làm đợt này:**
- Sinh viên thi (thuộc Server 2 — người khác làm)
- AI chấm bài (Flow sau)
- Anti-cheating (Server 2)
- Export Excel/PDF (Flow sau)
- Analytics dashboard (Flow sau)

---

## 4. Góc nhìn Backend Lead — "Code thế nào?"

### 4.1. Cấu trúc solution

```
Backend_Server1/
├── src/
│   ├── Domain/           ← Entities, Enums, Interfaces (KHÔNG phụ thuộc gì)
│   ├── Application/      ← Services, DTOs, Validators (→ Domain)
│   ├── Infrastructure/   ← EF Core, Storage, External clients (→ Application)
│   └── WebApi/           ← Controllers, Middleware, Program.cs (→ Application)
├── tests/
│   ├── Domain.UnitTests/
│   └── Application.UnitTests/
├── appsettings.json
├── .env / .env.example
└── Backend.Server1.slnx
```

### 4.2. Entities chính (đã có trong code)

| Entity | Mục đích | Status đang có |
|---|---|---|
| `User` | Tài khoản + role | ✅ Đã có |
| `Semester` | Học kỳ | ✅ Đã có |
| `Subject` | Môn học | ❓ Chưa thấy |
| `SubjectOffering` | Môn mở trong học kỳ | ❓ Chưa thấy |
| `Examination` | Kỳ thi cụ thể | ✅ Đã có |
| `Submission` | Lần nộp bộ đề | ✅ Đã có |
| `Grading` | Kết quả chấm (Flow sau) | ✅ Đã có |
| `ExamMaterial` | File đề/rubric/answer template | ✅ Đã có |

> Cần kiểm tra lại code hiện tại xem có đủ entities không.

### 4.3. Luồng nghiệp vụ chính — State Machine của Exam

```
                    ┌──────────────────────┐
                    │  Draft               │
                    │  (Khảo thí tạo)      │
                    └──────────┬───────────┘
                               │ Lecturer upload files + Submit
                               ▼
                    ┌──────────────────────┐
                    │  Submitted           │
                    └──────────┬───────────┘
                               │ Khảo thí mở review
                               ▼
                    ┌──────────────────────┐
                    │  UnderReview         │
                    └──────┬────────┬──────┘
                           │        │
                       Approve    Reject
                           │        │
                           ▼        ▼
                    ┌──────────┐  (Lecturer sửa + Submit lại)
                    │  Locked  │
                    └────┬─────┘
                         │ Khảo thí Package
                         ▼
                    ┌──────────────┐
                    │  Packaged    │
                    └──────┬───────┘
                           │ System Transfer sang Server 2
                           ▼
                    ┌──────────────┐
                    │  Transferred │
                    └──────────────┘
```

### 4.4. Auth đã làm

- ✅ JWT Bearer authentication
- ✅ BCrypt password hashing
- ✅ AuthSeeder: 3 account mẫu (Admin / Lecturer / Student) auto-create khi DB trống
- ✅ AuthOptions bind từ `appsettings.json`
- ✅ Các controller có `[Authorize]` phân quyền theo role

### 4.5. Còn phải làm (theo code)

| Phần | Trạng thái |
|---|---|
| Auth cơ bản | ✅ Done |
| CRUD User + gán Role | ❌ |
| CRUD Semester | ❌ |
| CRUD Subject | ❌ |
| CRUD SubjectOffering | ❌ |
| CRUD Examination | ❌ (entity có, controller có thể chưa đầy đủ) |
| Assign Lecturer | ❌ |
| Upload file (question/answer template/rubric) | ❌ |
| State machine Submit/Review/Lock | ❌ |
| Package + Transfer | ❌ |
| Audit log | ❌ |
| Swagger UI cho toàn bộ | 🟡 Một phần |

---

## 5. Góc nhìn DevOps — "Triển khai & vận hành"

### 5.1. Environment & Secrets

```
.env                       ← LOCAL ONLY, không commit
├── DATABASE_URL
├── JWT_SECRET             (≥ 32 ký tự)
├── JWT_ISSUER, JWT_AUDIENCE
├── JWT_EXPIRY_MINUTES
├── SUPABASE_URL + KEY     (nếu dùng)
├── STORAGE_TYPE           (local | supabase)
├── SERVER2_BASE_URL
└── AI_SERVICE_BASE_URL
```

### 5.2. Database

- **Dev local:** PostgreSQL (Supabase cloud, hoặc Docker local)
- **Schema source of truth:** Bảng trên Supabase (không tự ý sửa — phải qua review)
- **Migration:** EF Core, generate migration → **KHÔNG tự apply** lên Supabase → user tự apply

### 5.3. Deployment (khi đến lúc)

| Môi trường | Triển khai |
|---|---|
| Local dev | `dotnet run` + `npm run dev` |
| Staging | Docker → Cloud (Render/Railway/Fly.io) |
| Production | Azure App Service / AWS ECS — secret inject qua env vars |

### 5.4. Security checklist

- ✅ `.env` đã ignore trong `.gitignore`
- ✅ `appsettings.Development.json` đã ignore
- ✅ Password hash bằng BCrypt (không plain text)
- ⏳ JWT secret phải ≥ 32 ký tự random
- ⏳ HTTPS bắt buộc ở production
- ⏳ Rate limiting cho login endpoint
- ⏳ Audit log cho mọi thao tác nhạy cảm

---

## 6. Góc nhìn Tech Writer — "Tài liệu gì đã có?"

```
doc/
├── 01-assignment/                    ← Đề bài gốc (KHÔNG sửa)
│   ├── Phieu_FA26SE034.docx
│   └── _docx_text.txt                ← Bản text dễ đọc
│
├── 02-context/context.md             ✅ Bối cảnh tổng thể
├── 03-requirements/requirements.md    ✅ Yêu cầu & phạm vi (Flow 1)
├── 04-architecture/architecture.md    ✅ Kiến trúc & DB schema
├── 05-planning/plan.md               ✅ Plan & status
├── 06-decisions/decisions.md         ✅ Decision log
└── README.md                          ← Index cho doc/
```

---

## 7. Những câu hỏi đang chờ (Pending)

> Cần **bạn xác nhận** trước khi code tiếp:

### Nghiệp vụ (Q-01 → Q-10 trong `requirements.md` mục 6)

| ID | Câu hỏi |
|---|---|
| Q-01 | SubjectOffering cần lưu thêm thông tin gì (tín chỉ, mã lớp, sĩ số…)? |
| Q-02 | Assign Lecturer — 1 hay nhiều lecturer cho 1 exam? |
| Q-03 | Lecturer upload nhiều version đề cho 1 exam? |
| Q-04 | Lecturer có tự sửa sau khi bị reject, hay phải tạo submission mới? |
| Q-05 | Có cho un-lock exam không? |
| Q-06 | Rubric upload — Word hay Excel? Có template mẫu? |
| Q-07 | Cần track version của ExamSubmission? |
| Q-08 | Khảo thí có quyền upload thay Lecturer không? |
| Q-09 | 1 Lecturer phụ trách nhiều exam? |
| Q-10 | Flow 1 có cần API Student login không? |

### Quyết định kỹ thuật (D-006, D-007, D-008)

| ID | Câu hỏi |
|---|---|
| D-006 | PostgreSQL trực tiếp hay Supabase? |
| D-007 | File storage — Local hay Supabase Storage? |
| D-008 | Tên project chính thức (thay `Backend_Server1`)? |

### Với GVHD

| ID | Câu hỏi |
|---|---|
| P-004 | Xác nhận lại role "Khảo thí" (Examination Office) |

---

## 8. Tóm tắt 1 dòng cho AI

> **Tôi hiểu rằng:** Đây là hệ thống 2-server để hỗ trợ chấm bài thi thực hành môn SWR (Software Requirements) bằng AI. Server 1 (của bạn) làm Flow 1 — chuẩn bị đề + duyệt + đóng gói + chấm bài. Server 2 (người khác) làm Flow 2 — sinh viên thi. Backend dùng ASP.NET Core + Clean Architecture + PostgreSQL. Auth + JWT đã xong, còn phải làm CRUD danh mục + workflow duyệt đề + audit log + Swagger UI.

---

## Lịch sử thay đổi

| Ngày | Người | Thay đổi |
|---|---|---|
| 2026-09-09 | AI | Tạo file project-brief.md, mô tả dự án từ nhiều góc nhìn |
