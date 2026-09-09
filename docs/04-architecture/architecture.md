# Kiến trúc & Database (Architecture & Database Design)

> Tài liệu này mô tả kiến trúc kỹ thuật chi tiết của **Server 1**, bao gồm sơ đồ kiến trúc, database schema, API endpoints và luồng dữ liệu.

---

## 1. Sơ đồ kiến trúc tổng thể

```
                                    ┌─────────────────────────────┐
                                    │      Client (Browser)        │
                                    │   ReactJS / SPA / Swagger   │
                                    └──────────────┬──────────────┘
                                                   │ HTTPS
                                                   ▼
┌─────────────────────────────────────────────────────────────────┐
│                         SERVER 1                                 │
│                                                               │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                    ASP.NET Core Web API                  │   │
│  │                  (Authentication / AuthZ)                │   │
│  │                      JWT / Cookie                         │   │
│  └────────────────────────────┬────────────────────────────┘   │
│                               │                                  │
│  ┌────────────┬──────────────┴───────────────┬──────────────┐  │
│  │             │                                │              │  │
│  ▼             ▼                                ▼              ▼  │
│ WebAPI     Infrastructure              Application           Domain │
│ Controllers  Persistence                 Services            Entities │
│ Middleware   EF Core                   UseCases             Enums  │
│ Extensions   Storage                   DTOs               ValueObj │
│              External                  Validators          Interfaces│
│              Services                                      │
│                                                            │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │              Python AI Service (Extract + Grading)        │   │
│  │           (Gọi qua HTTP từ Application Layer)             │   │
│  │                                                          │   │
│  │  Layout Analysis → OCR → Object Detection → Structured   │   │
│  └─────────────────────────────────────────────────────────┘   │
│                                                               │
└─────────────────────────────────────────────────────────────────┘
                      │                           ▲
             Controlled Link              Transfer Protocol
                      │                           │
                      ▼                           │
┌───────────────────────────────────────────────────────────────┐
│                         SERVER 2                               │
│              (Offline — Không có Internet)                     │
│    Exam Session → Anti-cheating → Student Submit → Transfer    │
└───────────────────────────────────────────────────────────────┘
```

---

## 2. Clean Architecture chi tiết

```
Backend_Server1/
│
├── src/
│   │
│   ├── 1.Domain/                          # ❌ Không phụ thuộc gì khác
│   │       │
│   │       ├── Entities/                  # User, Role, Semester, Subject,
│   │       │                               #   SubjectOffering, Exam,
│   │       │                               #   ExamSubmission, ExamFile,
│   │       │                               #   ExamPackage, PackageTransfer,
│   │       │                               #   AuditLog
│   │       │
│   │       ├── Enums/                     # ExamStatus, UserRole,
│   │       │                               #   ExamFileType, TransferStatus
│   │       │
│   │       ├── ValueObjects/              # Email, PasswordHash, FileMetadata,
│   │       │                               #   SuspicionScore
│   │       │
│   │       └── Interfaces/                 # IUserRepository, IExamRepository,
│   │                                       #   IStorageService, IAuditService,
│   │                                       #   IAIService, IUnitOfWork
│   │
│   ├── 2.Application/                    # ✅ Phụ thuộc Domain
│   │       │
│   │       ├── DTOs/                      # Request/Response DTOs cho từng use case
│   │       │       ├── Auth/
│   │       │       ├── Semester/
│   │       │       ├── Subject/
│   │       │       ├── Exam/
│   │       │       └── Transfer/
│   │       │
│   │       ├── Interfaces/                # IAuthService, IExamService,
│   │       │                               # IUserService, IPackageService
│   │       │
│   │       ├── Services/                  # Implement các interface
│   │       │       ├── AuthService.cs
│   │       │       ├── UserService.cs
│   │       │       ├── SemesterService.cs
│   │       │       ├── SubjectService.cs
│   │       │       ├── ExamService.cs
│   │       │       ├── PackageService.cs
│   │       │       ├── AuditService.cs
│   │       │       └── AIOrchestrationService.cs
│   │       │
│   │       ├── Validators/                # FluentValidation rules
│   │       │       ├── RegisterRequestValidator.cs
│   │       │       ├── CreateExamRequestValidator.cs
│   │       │       └── ...
│   │       │
│   │       └── Mappings/                  # AutoMapper profiles
│   │
│   ├── 3.Infrastructure/                  # ✅ Phụ thuộc Application + Domain
│   │       │
│   │       ├── Persistence/               # Entity Framework Core
│   │       │       │
│   │       │       ├── AppDbContext.cs     # DbContext chính
│   │       │       │
│   │       │       ├── Configurations/     # EF Fluent API configs
│   │       │       │       ├── UserConfiguration.cs
│   │       │       │       ├── ExamConfiguration.cs
│   │       │       │       ├── ExamSubmissionConfiguration.cs
│   │       │       │       └── ...
│   │       │       │
│   │       │       ├── Repositories/       # Implement Repository pattern
│   │       │       │       ├── UserRepository.cs
│   │       │       │       ├── ExamRepository.cs
│   │       │       │       └── ...
│   │       │       │
│   │       │       └── Migrations/         # EF Core migrations
│   │       │
│   │       ├── Storage/                    # File storage
│   │       │       ├── IStorageService.cs
│   │       │       ├── LocalStorageService.cs
│   │       │       └── SupabaseStorageService.cs
│   │       │
│   │       └── External/                   # External services
│   │               ├── IPythonAIService.cs
│   │               ├── PythonAIService.cs   # HTTP client gọi Python service
│   │               └── Server2Client.cs      # HTTP client gọi Server 2
│   │
│   └── 4.WebAPI/                          # ✅ Phụ thuộc tất cả
│           │
│           ├── Controllers/
│           │       ├── AuthController.cs
│           │       ├── UsersController.cs
│           │       ├── SemestersController.cs
│           │       ├── SubjectsController.cs
│           │       ├── ExamsController.cs
│           │       ├── ExamFilesController.cs
│           │       ├── PackagesController.cs
│           │       └── AuditController.cs
│           │
│           ├── Middleware/
│           │       ├── JwtMiddleware.cs
│           │       ├── AuditMiddleware.cs
│           │       └── ExceptionMiddleware.cs
│           │
│           ├── Extensions/
│           │       ├── ServiceCollectionExtensions.cs
│           │       ├── SwaggerExtensions.cs
│           │       └── JwtExtensions.cs
│           │
│           ├── Filters/
│           │       └── ValidateModelAttribute.cs
│           │
│           └── Program.cs                   # Entry point
│
├── tests/
│   ├── 1.Domain.UnitTests/
│   ├── 2.Application.UnitTests/
│   ├── 3.Infrastructure.IntegrationTests/
│   └── 4.WebAPI.IntegrationTests/
│
├── Backend_Server1.sln
├── README.md
└── .env.example
```

---

## 3. Database Schema (PostgreSQL)

### 3.1. Entity Relationship Diagram

```
┌──────────────┐       ┌──────────────────┐       ┌─────────────────┐
│     ROLE      │       │       USER        │       │    AUDIT_LOG     │
│──────────────│       │─────────────────│       │─────────────────│
│ id (PK)      │◄──────│ role_id (FK)     │       │ id (PK)         │
│ name         │       │ id (PK)          │       │ user_id (FK)    │
│ description  │       │ username         │       │ action          │
│ permissions  │       │ email            │       │ entity_type     │
│ created_at   │       │ password_hash    │       │ entity_id       │
└──────────────┘       │ is_active        │       │ old_value       │
                       │ created_at       │       │ new_value       │
                       │ updated_at       │       │ ip_address      │
                       └────────┬─────────┘       │ created_at      │
                                │                  └─────────────────┘
                                │ user_id
                                │
        ┌───────────────────────┼───────────────────────┐
        │                       │                       │
        ▼                       ▼                       ▼
┌──────────────┐       ┌──────────────────┐    ┌──────────────────┐
│   SEMESTER    │       │ SUBJECT_OFFERING  │    │  EXAM_LECTURER   │
│──────────────│       │─────────────────│    │─────────────────│
│ id (PK)      │       │ id (PK)          │    │ exam_id (FK)    │
│ name         │◄──────│ semester_id (FK) │    │ user_id (FK)    │
│ code         │       │ subject_id (FK)  │    │ assigned_at     │
│ start_date   │       │ lecturer_id (FK) │    │ assigned_by (FK)│
│ end_date     │       │ start_date       │    └──────────────────┘
│ is_active    │       │ end_date         │
└──────────────┘       │ max_students     │
                       │ room             │
        ┌──────────────┘ status            │
        │ subject_id                        │
        ▼                                   │
┌──────────────┐                           │
│   SUBJECT     │                           │
│──────────────│                           │
│ id (PK)      │                           │
│ code         │                           │
│ name         │                           │
│ description  │                           │
│ credits      │                           │
└──────────────┘                           │
                                          │
                                          ▼
                                 ┌──────────────────┐
                                 │       EXAM       │
                                 │─────────────────│
                                 │ id (PK)          │
                                 │ subject_offering │──┐
                                 │   _id (FK)       │  │
                                 │ title            │  │
                                 │ description      │  │
                                 │ duration_minutes │  │
                                 │ scheduled_at     │  │
                                 │ status           │  │
                                 │ created_by (FK)  │  │
                                 │ created_at       │  │
                                 │ updated_at       │  │
                                 └────────┬─────────┘  │
                                          │ exam_id           │
                    ┌─────────────────────┼─────────────────┐
                    │                     │                   │
                    ▼                     ▼                   ▼
         ┌──────────────────┐  ┌──────────────────┐  ┌─────────────────┐
         │  EXAM_SUBMISSION │  │   EXAM_PACKAGE   │  │ EXAM_CONFIG     │
         │─────────────────│  │─────────────────│  │─────────────────│
         │ id (PK)          │  │ id (PK)          │  │ id (PK)         │
         │ exam_id (FK)     │──│ exam_id (FK)     │  │ exam_id (FK)    │
         │ submitted_by (FK)│  │ package_hash     │  │ setting_key     │
         │ status           │  │ status           │  │ setting_value    │
         │ submitted_at     │  │ created_at       │  └─────────────────┘
         │ reviewed_by (FK) │  │ locked_at        │
         │ reviewed_at      │  │ transferred_at   │
         │ review_notes     │  │ transfer_dest    │
         └────────┬─────────┘  └────────┬─────────┘
                  │ exam_submission_id             │
                  ▼                                    ▼
         ┌──────────────────┐              ┌──────────────────┐
         │   EXAM_FILE      │              │ PACKAGE_TRANSFER  │
         │─────────────────│              │─────────────────│
         │ id (PK)          │              │ id (PK)          │
         │ exam_submission  │              │ exam_package_id  │
         │   _id (FK)       │              │ from_server      │
         │ file_type        │              │ to_server        │
         │ original_name    │              │ status           │
         │ stored_path      │              │ initiated_by (FK)│
         │ file_size        │              │ initiated_at      │
         │ mime_type        │              │ completed_at      │
         │ uploaded_by (FK) │              │ notes             │
         │ uploaded_at      │              └──────────────────┘
         │ hash_sha256      │
         └──────────────────┘
```

### 3.2. Table Definitions

```sql
-- =============================================
-- ROLES & USERS
-- =============================================

CREATE TABLE roles (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name          VARCHAR(50)  NOT NULL UNIQUE,
    description   TEXT,
    permissions   JSONB       DEFAULT '[]',
    created_at    TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE users (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    role_id         UUID NOT NULL REFERENCES roles(id),
    username        VARCHAR(100) NOT NULL UNIQUE,
    email           VARCHAR(255) NOT NULL UNIQUE,
    password_hash   VARCHAR(255) NOT NULL,
    full_name       VARCHAR(200),
    is_active       BOOLEAN DEFAULT TRUE,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_users_role_id ON users(role_id);
CREATE INDEX idx_users_email ON users(email);

-- =============================================
-- SEMESTER & SUBJECT
-- =============================================

CREATE TABLE semesters (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name        VARCHAR(200) NOT NULL,      -- e.g. "Summer 2026"
    code        VARCHAR(50)  NOT NULL UNIQUE, -- e.g. "FA26"
    start_date  DATE         NOT NULL,
    end_date    DATE         NOT NULL,
    is_active   BOOLEAN DEFAULT TRUE,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE subjects (
    id          UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code        VARCHAR(50)  NOT NULL UNIQUE,  -- e.g. "SWR301"
    name        VARCHAR(255) NOT NULL,
    description TEXT,
    credits     INT DEFAULT 3,
    created_at  TIMESTAMPTZ DEFAULT NOW()
);

-- =============================================
-- SUBJECT OFFERING (môn mở trong học kỳ)
-- =============================================

CREATE TABLE subject_offerings (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    semester_id     UUID NOT NULL REFERENCES semesters(id) ON DELETE CASCADE,
    subject_id      UUID NOT NULL REFERENCES subjects(id) ON DELETE CASCADE,
    lecturer_id     UUID REFERENCES users(id),
    start_date      DATE,
    end_date        DATE,
    max_students    INT DEFAULT 50,
    room            VARCHAR(100),
    status          VARCHAR(20) DEFAULT 'active', -- active | closed
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(semester_id, subject_id)
);

CREATE INDEX idx_so_semester ON subject_offerings(semester_id);
CREATE INDEX idx_so_subject ON subject_offerings(subject_id);
CREATE INDEX idx_so_lecturer ON subject_offerings(lecturer_id);

-- =============================================
-- EXAM
-- =============================================

CREATE TABLE exams (
    id                UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    subject_offering_id  UUID NOT NULL REFERENCES subject_offerings(id) ON DELETE CASCADE,
    title             VARCHAR(255) NOT NULL,
    description       TEXT,
    duration_minutes  INT DEFAULT 90,
    scheduled_at      TIMESTAMPTZ,
    status            VARCHAR(30) DEFAULT 'draft',
    -- Status: draft | submitted | under_review | locked | packaged | transferred
    created_by        UUID REFERENCES users(id),
    created_at        TIMESTAMPTZ DEFAULT NOW(),
    updated_at        TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_exams_subject_offering ON exams(subject_offering_id);
CREATE INDEX idx_exams_status ON exams(status);
CREATE INDEX idx_exams_created_by ON exams(created_by);

-- =============================================
-- EXAM_LECTURER (phân công giảng viên chấm thi)
-- =============================================

CREATE TABLE exam_lecturers (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id       UUID NOT NULL REFERENCES exams(id) ON DELETE CASCADE,
    user_id       UUID NOT NULL REFERENCES users(id),
    assigned_by   UUID REFERENCES users(id),
    assigned_at   TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(exam_id, user_id)
);

CREATE INDEX idx_el_exam ON exam_lecturers(exam_id);
CREATE INDEX idx_el_user ON exam_lecturers(user_id);

-- =============================================
-- EXAM_SUBMISSION (lần nộp bộ đề của giảng viên)
-- =============================================

CREATE TABLE exam_submissions (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id         UUID NOT NULL REFERENCES exams(id) ON DELETE CASCADE,
    submitted_by    UUID NOT NULL REFERENCES users(id),
    status          VARCHAR(20) DEFAULT 'draft',
    -- Submission status: draft | submitted | under_review | approved | rejected | locked
    submitted_at    TIMESTAMPTZ,
    reviewed_by     UUID REFERENCES users(id),
    reviewed_at     TIMESTAMPTZ,
    review_notes    TEXT,
    created_at      TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_es_exam ON exam_submissions(exam_id);
CREATE INDEX idx_es_submitted_by ON exam_submissions(submitted_by);
CREATE INDEX idx_es_status ON exam_submissions(status);

-- =============================================
-- EXAM_FILE (file trong bộ đề)
-- =============================================

CREATE TABLE exam_files (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_submission_id  UUID NOT NULL REFERENCES exam_submissions(id) ON DELETE CASCADE,
    file_type           VARCHAR(30) NOT NULL,
    -- file_type: question | answer_template | rubric
    original_name       VARCHAR(255) NOT NULL,
    stored_path         VARCHAR(500) NOT NULL,
    file_size           BIGINT,
    mime_type           VARCHAR(100),
    hash_sha256         VARCHAR(64),
    uploaded_by         UUID REFERENCES users(id),
    uploaded_at         TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_ef_submission ON exam_files(exam_submission_id);
CREATE INDEX idx_ef_type ON exam_files(file_type);

-- =============================================
-- EXAM_PACKAGE (package đóng gói)
-- =============================================

CREATE TABLE exam_packages (
    id              UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_id         UUID NOT NULL REFERENCES exams(id),
    package_hash    VARCHAR(64) UNIQUE,  -- SHA256 hash của package
    status          VARCHAR(20) DEFAULT 'pending',
    -- status: pending | ready | transferred | failed
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    locked_at       TIMESTAMPTZ,
    transferred_at TIMESTAMPTZ,
    locked_by       UUID REFERENCES users(id)
);

CREATE INDEX idx_ep_exam ON exam_packages(exam_id);
CREATE INDEX idx_ep_status ON exam_packages(status);

-- =============================================
-- PACKAGE_TRANSFER (bản ghi chuyển package)
-- =============================================

CREATE TABLE package_transfers (
    id                  UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    exam_package_id     UUID NOT NULL REFERENCES exam_packages(id),
    from_server_id      VARCHAR(50) DEFAULT 'SERVER1',
    to_server_id        VARCHAR(50) DEFAULT 'SERVER2',
    status              VARCHAR(20) DEFAULT 'pending',
    -- status: pending | in_progress | completed | failed
    initiated_by        UUID REFERENCES users(id),
    initiated_at        TIMESTAMPTZ DEFAULT NOW(),
    completed_at        TIMESTAMPTZ,
    transfer_url        VARCHAR(500),
    notes               TEXT
);

CREATE INDEX idx_pt_package ON package_transfers(exam_package_id);
CREATE INDEX idx_pt_status ON package_transfers(status);

-- =============================================
-- AUDIT_LOG
-- =============================================

CREATE TABLE audit_logs (
    id            UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id       UUID REFERENCES users(id),
    action        VARCHAR(100) NOT NULL,
    entity_type   VARCHAR(100),
    entity_id     UUID,
    old_value     JSONB,
    new_value     JSONB,
    ip_address    VARCHAR(45),
    user_agent    TEXT,
    created_at    TIMESTAMPTZ DEFAULT NOW()
);

CREATE INDEX idx_al_user ON audit_logs(user_id);
CREATE INDEX idx_al_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX idx_al_created_at ON audit_logs(created_at);
CREATE INDEX idx_al_action ON audit_logs(action);
```

---

## 4. API Endpoints (OpenAPI — Flow 1)

### 4.1. Authentication

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| POST | `/api/v1/auth/register` | Đăng ký tài khoản | Public |
| POST | `/api/v1/auth/login` | Đăng nhập, nhận JWT | Public |
| POST | `/api/v1/auth/logout` | Đăng xuất | Authenticated |
| GET | `/api/v1/auth/me` | Lấy thông tin user hiện tại | Authenticated |

### 4.2. Users (Admin)

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/users` | Danh sách users (phân trang) | Admin |
| GET | `/api/v1/users/{id}` | Chi tiết user | Admin |
| POST | `/api/v1/users` | Tạo user | Admin |
| PUT | `/api/v1/users/{id}` | Cập nhật user | Admin |
| DELETE | `/api/v1/users/{id}` | Xóa user | Admin |
| PUT | `/api/v1/users/{id}/role` | Gán role cho user | Admin |

### 4.3. Semesters

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/semesters` | Danh sách học kỳ | Authenticated |
| GET | `/api/v1/semesters/{id}` | Chi tiết học kỳ | Authenticated |
| POST | `/api/v1/semesters` | Tạo học kỳ | Admin |
| PUT | `/api/v1/semesters/{id}` | Cập nhật học kỳ | Admin |
| DELETE | `/api/v1/semesters/{id}` | Xóa học kỳ | Admin |

### 4.4. Subjects

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/subjects` | Danh sách môn học | Authenticated |
| GET | `/api/v1/subjects/{id}` | Chi tiết môn học | Authenticated |
| POST | `/api/v1/subjects` | Tạo môn học | Admin |
| PUT | `/api/v1/subjects/{id}` | Cập nhật môn học | Admin |
| DELETE | `/api/v1/subjects/{id}` | Xóa môn học | Admin |

### 4.5. Subject Offerings

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/subject-offerings` | Danh sách (filter by semester/subject) | Authenticated |
| GET | `/api/v1/subject-offerings/{id}` | Chi tiết | Authenticated |
| POST | `/api/v1/subject-offerings` | Tạo subject offering | Admin, ExaminationOffice |
| PUT | `/api/v1/subject-offerings/{id}` | Cập nhật | Admin, ExaminationOffice |
| DELETE | `/api/v1/subject-offerings/{id}` | Xóa | Admin |

### 4.6. Exams

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/exams` | Danh sách exam (filter) | Authenticated |
| GET | `/api/v1/exams/{id}` | Chi tiết exam + submissions | Authenticated |
| POST | `/api/v1/exams` | Tạo exam | Admin, ExaminationOffice |
| PUT | `/api/v1/exams/{id}` | Cập nhật exam | Admin, ExaminationOffice |
| DELETE | `/api/v1/exams/{id}` | Xóa exam (chỉ khi draft) | Admin |
| GET | `/api/v1/exams/{id}/status` | Lấy trạng thái exam | Authenticated |
| PUT | `/api/v1/exams/{id}/status` | Cập nhật trạng thái (submit/lock/package) | Authenticated |

### 4.7. ExamLecturers (Assign)

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/exams/{id}/lecturers` | Danh sách lecturer được gán | Authenticated |
| POST | `/api/v1/exams/{id}/lecturers` | Gán lecturer cho exam | Admin, ExaminationOffice |
| DELETE | `/api/v1/exams/{id}/lecturers/{userId}` | Xóa assignment | Admin, ExaminationOffice |

### 4.8. Exam Submissions & Files

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/exams/{id}/submissions` | Danh sách submissions của exam | Authenticated |
| GET | `/api/v1/exams/{id}/submissions/{subId}` | Chi tiết submission + files | Authenticated |
| POST | `/api/v1/exams/{id}/submissions` | Tạo submission mới (draft) | Lecturer |
| PUT | `/api/v1/exams/{id}/submissions/{subId}` | Cập nhật submission (thêm file…) | Lecturer |
| POST | `/api/v1/exams/{id}/submissions/{subId}/submit` | Submit bộ đề | Lecturer |
| POST | `/api/v1/exams/{id}/submissions/{subId}/review` | Review (approve/reject) | Admin, ExaminationOffice |
| POST | `/api/v1/exams/{id}/submissions/{subId}/lock` | Lock submission | Admin, ExaminationOffice |

### 4.9. File Upload/Download

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| POST | `/api/v1/submissions/{subId}/files` | Upload file (multipart) | Lecturer |
| GET | `/api/v1/files/{fileId}` | Download file | Authenticated |
| GET | `/api/v1/files/{fileId}/preview` | Preview file (docx → PDF preview) | Authenticated |
| DELETE | `/api/v1/files/{fileId}` | Xóa file | Lecturer |

### 4.10. Package & Transfer

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/exams/{id}/package` | Lấy thông tin package | Authenticated |
| POST | `/api/v1/exams/{id}/package` | Tạo package (đóng gói) | Admin, ExaminationOffice |
| POST | `/api/v1/exams/{id}/package/lock` | Lock package | Admin, ExaminationOffice |
| POST | `/api/v1/exams/{id}/package/transfer` | Bắt đầu transfer sang Server 2 | Admin |
| GET | `/api/v1/exams/{id}/transfers` | Lịch sử transfer | Admin |

### 4.11. Audit

| Method | Endpoint | Mô tả | Role |
|---|---|---|---|
| GET | `/api/v1/audit-logs` | Danh sách audit log (phân trang, filter) | Admin |
| GET | `/api/v1/audit-logs/{entityType}/{entityId}` | Log của một entity cụ thể | Admin |

---

## 5. Luồng dữ liệu (Data Flow)

### 5.1. Upload đề thi (Question File)

```
Lecturer
   │
   │ POST /api/v1/submissions/{subId}/files  (multipart/form-data)
   ▼
WebAPI/ExamFilesController
   │
   │ Validate file (extension, size, MIME)
   ▼
FileStorageService.SaveAsync(file, "exam-files/{examId}/{submissionId}/")
   │
   │ Lưu file vào disk / Supabase Storage
   │ Tính SHA256 hash
   ▼
ExamFileRepository.Add(new ExamFile { ... })
   │
   │ UnitOfWork.SaveChangesAsync()
   ▼
AuditService.Log("EXAM_FILE_UPLOADED", examFile)
   │
   ▼
Response: 201 Created { fileId, fileName, fileType, ... }
```

### 5.2. Exam State Transition

```
Lecturer tạo Exam
   │
   │ Status = Draft
   ▼
Lecturer upload files
   │
   │ POST /submit → Status = Submitted
   ▼
Khảo thí Review
   │
   │ POST /review {action: "approve"} → Status = UnderReview → Locked
   │ POST /review {action: "reject"} → Status = Rejected (Lecturer sửa lại)
   ▼
Khảo thí Lock
   │
   │ POST /lock → Status = Locked (không cho sửa)
   ▼
Package
   │
   │ POST /package → Status = Packaged
   ▼
Transfer
   │
   │ POST /transfer → Status = Transferred
```

---

## 6. Các interface quan trọng

### 6.1. IStorageService

```csharp
public interface IStorageService
{
    Task<string> SaveAsync(Stream file, string path, string contentType);
    Task<Stream> GetAsync(string path);
    Task DeleteAsync(string path);
    Task<string> GetPresignedUrlAsync(string path, TimeSpan expiry);
    Task<bool> ExistsAsync(string path);
}
```

### 6.2. IAIService

```csharp
public interface IAIService
{
    // Trong giai đoạn sau — Flow 1 chỉ cần định nghĩa interface
    Task<ExtractResult> ExtractAsync(Stream documentStream, string fileType);
    Task<GradingResult> GradeAsync(ExtractResult structured, RubricData rubric);
}
```

### 6.3. IServer2Client

```csharp
public interface IServer2Client
{
    Task<TransferResult> SendPackageAsync(ExamPackage package);
    Task<PingResponse> PingAsync();
}
```

---

## 7. Enums quan trọng

```csharp
// ExamStatus
public enum ExamStatus
{
    Draft = 0,
    Submitted = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Locked = 5,
    Packaged = 6,
    Transferred = 7
}

// ExamFileType
public enum ExamFileType
{
    Question = 0,
    AnswerTemplate = 1,
    Rubric = 2
}

// UserRole
public enum UserRole
{
    Admin = 0,
    ExaminationOffice = 1,
    Lecturer = 2,
    Student = 3
}

// TransferStatus
public enum TransferStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3
}
```

---

## 8. Environment variables

```bash
# Database
DATABASE_URL=postgresql://user:pass@host:5432/swrexam
DB_PROVIDER=postgresql

# JWT
JWT_SECRET=your-super-secret-key-at-least-32-chars
JWT_ISSUER=SwrGradingAPI
JWT_AUDIENCE=SwrGradingClient
JWT_EXPIRY_MINUTES=60

# Storage
STORAGE_TYPE=local  # hoặc "supabase"
STORAGE_LOCAL_PATH=D:/FPT/storage
SUPABASE_URL=https://xxx.supabase.co
SUPABASE_KEY=xxx
SUPABASE_BUCKET=exam-files

# Server 2
SERVER2_BASE_URL=http://192.168.1.100:5000
SERVER2_API_KEY=xxx

# AI Service
AI_SERVICE_BASE_URL=http://localhost:8000
AI_SERVICE_API_KEY=xxx

# App
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://0.0.0.0:5000
```
