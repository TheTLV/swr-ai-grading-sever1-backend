# Kế hoạch & Trạng thái công việc (Plan & Status)

> Tài liệu này ghi lại **kế hoạch làm việc** và **trạng thái hiện tại** của từng task. Cập nhật liên tục khi làm việc.

---

## 1. Cấu trúc thư mục dự án

```
D:\FPT\2025_k9\DoAn\
│
├── doc/                              # Tài liệu & tài nguyên
│   ├── Phieu_FA26SE034.docx          # File đề bài gốc
│   ├── _docx_text.txt                # Đề bài dạng text (trích xuất từ docx)
│   ├── context.md                    # Bối cảnh tổng thể (✅ đã tạo)
│   ├── requirements.md                # Yêu cầu & phạm vi (✅ đã tạo)
│   ├── architecture.md               # Kiến trúc & DB (⏳ đang tạo)
│   ├── plan.md                      # Kế hoạch & trạng thái (✅ đã tạo)
│   └── decisions.md                  # Log quyết định (⏳ đang tạo)
│
└── Backend_Server1/                  # Backend C# .NET cho Server 1
    ├── README.md                      # (⏳ sẽ tạo sau)
    ├── Backend_Server1.sln            # (⏳ sẽ tạo sau)
    └── src/                          # Source code
        ├── Domain/                   # Entities, Enums, ValueObjects, Interfaces
        ├── Application/              # DTOs, Interfaces, Services, UseCases, Validators
        ├── Infrastructure/            # Persistence, Storage, External Services
        └── WebAPI/                   # Controllers, Middleware, Extensions, Program.cs
```

---

## 2. Các giai đoạn (Phases)

### ✅ Phase 0 — Chuẩn bị & Thu thập yêu cầu (2026-09-09)
**Trạng thái:** HOÀN THÀNH

| Task | Mô tả | Trạng thái | Ghi chú |
|---|---|---|---|
| T-00 | Đọc file đề bài Word | ✅ Done | Trích xuất qua PowerShell XML parsing |
| T-01 | Tổ chức thư mục (doc/, Backend_Server1/) | ✅ Done | |
| T-02 | Chuyển file đề vào doc/ | ✅ Done | |
| T-03 | Tạo doc/context.md | ✅ Done | |
| T-04 | Tạo doc/requirements.md | ✅ Done | |
| T-05 | Tạo doc/plan.md | ✅ Done | |
| T-06 | Tạo doc/decisions.md | ✅ Done | |
| T-07 | Tạo doc/architecture.md | ✅ Done | |
| T-08 | Tạo Backend_Server1/README.md | ✅ Done | |
| T-09 | Tạo .NET solution + 4 projects + NuGet | ✅ Done | Build OK: 0 errors, 0 warnings |

---

### ⏳ Phase 1 — Thiết kế & Setup (ngày dự kiến: sau Phase 0)

**Trạng thái:** CHƯA BẮT ĐẦU

> **Mục tiêu:** Hoàn thành thiết kế chi tiết (DB schema, API endpoints, architecture) và setup project .NET.

| Task | Mô tả | Trạng thái | Ghi chú |
|---|---|---|---|
| T-10 | Xác nhận lại câu hỏi nghiệp vụ (Q-01 → Q-10) | ⏳ Pending | Cần user trả lời |
| T-11 | Thiết kế chi tiết DB schema | ⏳ Pending | Dựa trên architecture.md |
| T-12 | Thiết kế chi tiết API endpoints (OpenAPI spec) | ⏳ Pending | |
| T-13 | Setup project .NET (dotnet new webapi) | ⏳ Pending | |
| T-14 | Setup Clean Architecture folder structure | ⏳ Pending | |
| T-15 | Setup Entity Framework Core + PostgreSQL | ⏳ Pending | |
| T-16 | Setup JWT Authentication | ⏳ Pending | |
| T-17 | Viết README.md cho Backend_Server1 | ⏳ Pending | |

---

### ⏳ Phase 2 — Authentication & Authorization (ngày dự kiến: sau Phase 1)

**Trạng thái:** CHƯA BẮT ĐẦU

> **Mục tiêu:** Module Register, Login, JWT, Role-based access.

| Task | Mô tả | Trạng thái | Ghi chú |
|---|---|---|---|
| T-20 | User Entity + DB migration | ⏳ Pending | |
| T-21 | Role Entity + Role mapping | ⏳ Pending | |
| T-22 | Auth DTOs (Register, Login, Token) | ⏳ Pending | |
| T-23 | Auth Service (register, login, token generation) | ⏳ Pending | |
| T-24 | Auth Controller + JWT middleware | ⏳ Pending | |
| T-25 | Role-based authorization (policy-based) | ⏳ Pending | |
| T-26 | Audit log middleware/service | ⏳ Pending | |
| T-27 | Unit tests cho Auth | ⏳ Pending | |

---

### ⏳ Phase 3 — Quản lý danh mục & Exam (ngày dự kiến: sau Phase 2)

**Trạng thái:** CHƯA BẮT ĐẦU

> **Mục tiêu:** CRUD Semester, Subject, Subject Offering, Exam, Assign Lecturer.

| Task | Mô tả | Trạng thái | Ghi chú |
|---|---|---|---|
| T-30 | Semester Entity + CRUD | ⏳ Pending | |
| T-31 | Subject Entity + CRUD | ⏳ Pending | |
| T-32 | SubjectOffering Entity + CRUD | ⏳ Pending | |
| T-33 | Exam Entity + CRUD | ⏳ Pending | |
| T-34 | ExamLecturer (assignment) | ⏳ Pending | |
| T-35 | File upload infrastructure (local/Supabase) | ⏳ Pending | |
| T-36 | ExamFile Entity + upload | ⏳ Pending | |
| T-37 | Rubric Entity + upload (docx/xlsx) | ⏳ Pending | |
| T-38 | Exam state machine (Draft → Submitted → Reviewed → Locked → Packaged) | ⏳ Pending | |
| T-39 | Package + Transfer | ⏳ Pending | |
| T-40 | Unit tests cho các entity/service | ⏳ Pending | |

---

### ⏳ Phase 4 — Review, Lock, Package, Transfer (ngày dự kiến: sau Phase 3)

**Trạng thái:** CHƯA BẮT ĐẦU

| Task | Mô tả | Trạng thái | Ghi chú |
|---|---|---|---|
| T-50 | Review/Check flow (Khảo thí duyệt) | ⏳ Pending | |
| T-51 | Lock exam | ⏳ Pending | |
| T-52 | ExamPackage Entity + logic | ⏳ Pending | |
| T-53 | PackageTransfer Entity + mock endpoint | ⏳ Pending | |
| T-54 | Integration test cho end-to-end flow | ⏳ Pending | |

---

### ⏳ Phase 5 — Swagger & Documentation

**Trạng thái:** CHƯA BẮT ĐẦU

| Task | Mô tả | Trạng thái | Ghi chú |
|---|---|---|---|
| T-60 | Swagger UI configuration | ⏳ Pending | |
| T-61 | XML comment cho API endpoints | ⏳ Pending | |
| T-62 | API versioning | ⏳ Pending | |

---

### ⏳ Phase 6 — Extract & AI Grading Interface (DEFERRED)

**Trạng thái:** CHƯA BẮT ĐẦU — GIAI ĐOẠN SAU

| Task | Mô tả | Trạng thái | Ghi chú |
|---|---|---|---|
| T-70 | Define Extract interface (IPythonService) | ⏳ Pending | |
| T-71 | Mock Python AI service (stub) | ⏳ Pending | |
| T-72 | AI Grading orchestration service | ⏳ Pending | |

---

## 3. Milestones

| Milestone | Mô tả | Target | Trạng thái |
|---|---|---|---|
| **M0** | Tất cả file doc/ hoàn chỉnh | 2026-09-09 | ✅ Done |
| **M1** | .NET project chạy được, Swagger mở | TBD | ⏳ Pending |
| **M2** | Auth: Register/Login/JWT/RBAC | TBD | ⏳ Pending |
| **M3** | CRUD: Semester/Subject/SubjectOffering/Exam | TBD | ⏳ Pending |
| **M4** | File upload: Question/AnswerTemplate/Rubric | TBD | ⏳ Pending |
| **M5** | Exam state machine: Submit → Review → Lock → Package → Transfer | TBD | ⏳ Pending |
| **M6** | Audit log hoạt động đầy đủ | TBD | ⏳ Pending |
| **M7** | Swagger có đủ API + XML comment | TBD | ⏳ Pending |
| **M8** | Unit tests cover core services | TBD | ⏳ Pending |

---

## 4. Những việc cần làm NGAY (Next Actions)

### Trước khi bắt đầu Phase 1:

1. **[CRITICAL]** Trả lời các câu hỏi nghiệp vụ Q-01 → Q-10 trong `requirements.md` section 6
2. **[IMPORTANT]** Xác nhận stack: dùng .NET (đã quyết định) — nhưng cần chọn:
   - **Database:** PostgreSQL trực tiếp hay Supabase (PostgreSQL + Auth + Storage)?
   - **File storage:** Local filesystem hay Supabase Storage?
3. **[IMPORTANT]** Đặt tên project chính thức (thay vì "Backend_Server1" tạm)
4. **[OPTIONAL]** Xác nhận lại với GVHD về các role (Admin, Khảo thí, Lecturer)

---

## 5. Changelog

| Ngày | Người | Thay đổi |
|---|---|---|
| 2026-09-09 | AI | Tạo file doc/, tổ chức thư mục, tạo context.md, requirements.md, plan.md |
| 2026-09-09 | AI | Trích xuất nội dung file đề từ Phieu_FA26SE034.docx |
| 2026-09-09 | AI | Tạo decisions.md, architecture.md, README.md |
| 2026-09-09 | AI | Tạo .NET solution: 4 projects (Domain/Application/Infrastructure/WebAPI) + 2 test projects |
| 2026-09-09 | AI | Install packages: EF Core 8, JWT, FluentValidation, Swashbuckle, Moq — Build: 0 errors |
