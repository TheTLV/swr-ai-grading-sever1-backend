# 🏗 04-architecture/ — Kiến trúc & Thiết kế kỹ thuật

> **Mục đích:** Sơ đồ kiến trúc, cấu trúc project, database schema (ERD + SQL), API endpoints, interfaces, enums, environment variables.
> Đọc khi muốn **code đúng thiết kế** hoặc muốn hiểu **tại sao code lại tổ chức như vậy**.

## 📄 File trong thư mục này

| File | Mô tả |
|---|---|
| `architecture.md` | Tài liệu kiến trúc chính |

## 🔍 Mục lục của `architecture.md`

1. Sơ đồ kiến trúc tổng thể (Server 1 ↔ Server 2 ↔ AI Service ↔ Client)
2. Clean Architecture chi tiết — Domain, Application, Infrastructure, WebAPI
3. Database Schema — ERD + PostgreSQL DDL (10 tables)
4. API Endpoints (OpenAPI cho Flow 1) — 11 nhóm endpoint
5. Luồng dữ liệu (Data Flow) — File upload, Exam state transition
6. Các interface quan trọng — IStorageService, IAIService, IServer2Client
7. Enums quan trọng — ExamStatus, ExamFileType, UserRole, TransferStatus
8. Environment variables — Database, JWT, Storage, AI, Server 2

## 🔗 Liên kết

- Code: [`../../Backend_Server1/`](../../Backend_Server1/)
- Database cấu hình thật: xem `Backend_Server1/src/4.WebAPI/appsettings.Development.json`
