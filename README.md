# Backend_Server1 — SWR30x AI-Assisted Grading System

> **⚠️ NOTE:** Tên project chính thức chưa được xác định. Đang dùng tên tạm `Backend_Server1`. Cần cập nhật sau khi user xác nhận.

**Server 1** — Giảng viên / Khảo thí — Server cho giảng viên tạo đề, khảo thí kiểm tra, và nhận bài chấm bằng AI.

---

## Mục lục

1. [Bối cảnh](#bối-cảnh)
2. [Công nghệ sử dụng](#công-nghệ-sử-dụng)
3. [Cấu trúc thư mục](#cấu-trúc-thư-mục)
4. [Cài đặt & Chạy](#cài-đặt--chạy)
5. [Cấu hình](#cấu-hình)
6. [API Documentation](#api-documentation)
7. [Database](#database)
8. [Các module](#các-module)
9. [Flow 1 — Phạm vi hiện tại](#flow-1--phạm-vi-hiện-tại)
10. [Tài liệu liên quan](#tài-liệu-liên-quan)

---

## Bối cảnh

Xem: [`../doc/context.md`](../doc/context.md)

**Tóm tắt:** Hệ thống chấm bài thực hành SWR30x có hỗ trợ AI, gồm 2 server:
- **Server 1** (repo này): Giảng viên + Khảo thí + AI grading
- **Server 2**: Sinh viên thi (offline, do thành viên khác phụ trách)

**Flow 1 hiện tại:** Register → Login → Semester → Subject → SubjectOffering → Exam → Assign Lecturer → Upload Files → Validate → Submit → Review → Lock → Package → Transfer

---

## Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Framework | ASP.NET Core 8.0 (C#) |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL |
| Authentication | JWT (Bearer Token) |
| API Docs | Swagger / OpenAPI 3.0 |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| File Storage | Local filesystem / Supabase Storage |
| AI Service | Python (gọi qua HTTP) |
| Architecture | Clean Architecture |
| Testing | xUnit + Moq |

---

## Cấu trúc thư mục

```
Backend_Server1/
├── src/
│   ├── 1.Domain/          # Entities, Enums, ValueObjects, Interfaces
│   ├── 2.Application/     # DTOs, Services, UseCases, Validators, Mappings
│   ├── 3.Infrastructure/  # EF Core, Repositories, Storage, External Services
│   └── 4.WebAPI/          # Controllers, Middleware, Extensions, Program.cs
├── tests/                  # Unit tests, Integration tests
├── doc/                    # (symlink hoặc reference sang ../doc/)
├── Backend_Server1.sln
├── README.md
└── .env.example
```

Xem chi tiết: [`../doc/architecture.md`](../doc/architecture.md#2-clean-architecture-chi-tiết)

---

## Cài đặt & Chạy

### Yêu cầu

- .NET 8.0 SDK trở lên
- PostgreSQL 14+ (hoặc Docker)
- (Tùy chọn) Supabase project cho storage

### Các bước

```bash
# 1. Clone / mở repo
cd D:\FPT\2025_k9\DoAn\Backend_Server1

# 2. Restore dependencies
dotnet restore

# 3. Cấu hình environment
cp .env.example .env
# Chỉnh sửa .env với database URL, JWT secret, ...

# 4. Tạo database (chạy migration)
dotnet ef database update --project src/3.Infrastructure
# Hoặc nếu dùng script SQL: chạy file SQL trong doc/migrations/

# 5. Chạy WebAPI
dotnet run --project src/4.WebAPI

# 6. Mở Swagger
# http://localhost:5000/swagger
```

### Chạy tests

```bash
dotnet test
```

---

## Cấu hình

Tất cả cấu hình qua biến môi trường hoặc `appsettings.json`. Xem `.env.example` và `appsettings.json` trong `src/4.WebAPI/`.

```bash
# Database
DATABASE_URL=postgresql://user:pass@host:5432/swrexam
DB_PROVIDER=postgresql

# JWT
JWT_SECRET=<your-secret-at-least-32-chars>
JWT_ISSUER=SwrGradingAPI
JWT_AUDIENCE=SwrGradingClient
JWT_EXPIRY_MINUTES=60

# Storage
STORAGE_TYPE=local  # hoặc supabase
STORAGE_LOCAL_PATH=./storage

# AI Service (Python)
AI_SERVICE_BASE_URL=http://localhost:8000

# Server 2 (offline exam)
SERVER2_BASE_URL=http://192.168.1.100:5000
```

---

## API Documentation

Sau khi chạy app, Swagger UI tại:
```
http://localhost:5000/swagger
```

Danh sách endpoint chính: Xem [`../doc/architecture.md#4-api-endpoints-openapi--flow-1`](../doc/architecture.md#4-api-endpoints-openapi--flow-1)

---

## Database

### ERD
Xem [`../doc/architecture.md#3-database-schema-postgresql`](../doc/architecture.md#3-database-schema-postgresql)

### Chạy migrations

```bash
# Tạo migration mới
dotnet ef migrations add <MigrationName> --project src/3.Infrastructure --startup-project src/4.WebAPI

# Apply migration
dotnet ef database update --project src/3.Infrastructure --startup-project src/4.WebAPI

# Rollback
dotnet ef database update <PreviousMigrationName> --project src/3.Infrastructure
```

### Seed data
Tạo tài khoản admin mặc định:
- Username: `admin`
- Email: `admin@swrexam.local`
- Password: `Admin123!` (nên đổi sau khi tạo)

---

## Các module

| Module | Mô tả | Trạng thái |
|---|---|---|
| **Auth** | Register, Login, JWT, RBAC | ⏳ Chưa bắt đầu |
| **User Management** | CRUD users, gán role | ⏳ Chưa bắt đầu |
| **Semester** | CRUD Semester | ⏳ Chưa bắt đầu |
| **Subject** | CRUD Subject | ⏳ Chưa bắt đầu |
| **Subject Offering** | CRUD SubjectOffering | ⏳ Chưa bắt đầu |
| **Exam** | CRUD Exam, state machine | ⏳ Chưa bắt đầu |
| **Exam Submission** | Upload files, submit, review | ⏳ Chưa bắt đầu |
| **Package & Transfer** | Đóng gói, transfer | ⏳ Chưa bắt đầu |
| **Audit Log** | Ghi log thao tác | ⏳ Chưa bắt đầu |
| **Extract** | Interface cho AI pipeline | ⏳ Chưa bắt đầu |
| **AI Grading** | Interface cho AI service | ⏳ Chưa bắt đầu |

---

## Flow 1 — Phạm vi hiện tại

Xem chi tiết: [`../doc/requirements.md#4-phạm-vi-scope--server-1--flow-1`](../doc/requirements.md#4-phạm-vi-scope--server-1--flow-1)

```
Register → Login → Auth → Semester → Subject → SubjectOffering →
Exam → Assign Lecturer → Upload Files → Submit → Review →
Lock → Package → Transfer to Server 2
```

### IN-SCOPE
- ✅ Authentication / Authorization (JWT + RBAC)
- ✅ Quản lý Semester / Subject / SubjectOffering
- ✅ Exam CRUD + Assign Lecturer
- ✅ Upload 3 loại file (Question, AnswerTemplate, Rubric)
- ✅ Exam state machine (Draft → Submitted → Reviewed → Locked → Packaged)
- ✅ Package & Transfer (mock/interface)
- ✅ Audit log
- ✅ Swagger/OpenAPI

### OUT-OF-SCOPE (Flow sau)
- ❌ Anti-cheating, activity monitoring
- ❌ Extract, OCR, AI grading
- ❌ Analytics, dashboard, charts
- ❌ Student login/nộp bài (Server 2)

---

## Tài liệu liên quan

| File | Mô tả |
|---|---|
| [`../doc/context.md`](../doc/context.md) | Bối cảnh tổng thể đồ án |
| [`../doc/requirements.md`](../doc/requirements.md) | Yêu cầu & phạm vi chi tiết |
| [`../doc/plan.md`](../doc/plan.md) | Kế hoạch & trạng thái task |
| [`../doc/decisions.md`](../doc/decisions.md) | Lịch sử các quyết định |
| [`../doc/architecture.md`](../doc/architecture.md) | Kiến trúc chi tiết & DB schema |
| [`../doc/Phieu_FA26SE034.docx`](../doc/Phieu_FA26SE034.docx) | Đề bài gốc |

---

## Liên hệ / Thông tin nhóm

*(Cập nhật sau khi user cung cấp)*
