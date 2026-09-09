# 📚 doc/ — Thư mục tài liệu dự án

> **Mục đích:** Lưu trữ toàn bộ tài liệu của dự án SWR30x AI-Assisted Grading System (Server 1) được tổ chức **theo mục đích sử dụng** để dễ tìm kiếm.

> ⚠️ **Quy ước đặt tên:** Mỗi thư mục bắt đầu bằng **số thứ tự 2 chữ số + dấu gạch ngang** để giữ thứ tự ưu tiên và tránh hệ thống xếp lộn xộn.

---

## 🗂 Cấu trúc

```
doc/
│
├── 📌 README.md                    ← File này — index tổng
│
├── 01-assignment/                  ← Đề bài gốc & nguồn yêu cầu từ GVHD
│   ├── README.md
│   ├── Phieu_FA26SE034.docx         ← Phiếu đề tài gốc từ giảng viên
│   └── _docx_text.txt               ← Đề bài trích ra dạng text (dễ đọc hơn Word)
│
├── 02-context/                     ← Bối cảnh tổng thể & thuật ngữ
│   ├── README.md
│   └── context.md                   ← Bức tranh toàn cảnh đồ án (2 server, Flow 1, AI role, stack)
│
├── 03-requirements/                ← Yêu cầu & phạm vi
│   ├── README.md
│   └── requirements.md             ← Functional/Non-functional, Use cases, IN/OUT scope, Acceptance criteria
│
├── 04-architecture/               ← Kiến trúc & thiết kế kỹ thuật
│   ├── README.md
│   ├── architecture.md             ← Sơ đồ, Clean Architecture, DB schema, API endpoints, interfaces
│   └── (sẽ thêm: ERD.png, sequence-diagrams/, deployment-diagrams/ ở phase sau)
│
├── 05-planning/                   ← Kế hoạch & theo dõi tiến độ
│   ├── README.md
│   └── plan.md                    ← Phases, milestones, task list, changelog, next actions
│
└── 06-decisions/                  ← Lịch sử các quyết định
    ├── README.md
    └── decisions.md               ← Mỗi quyết định có ID, lý do, hệ quả — giúp hiểu vì sao thiết kế như vậy
```

---

## 🧭 Tôi cần gì thì vào đâu?

| Tôi muốn… | Vào thư mục |
|---|---|
| Đọc đề bài gốc từ giảng viên | `01-assignment/` |
| Hiểu tổng quan đồ án là gì, 2 server là gì, AI ở đâu | `02-context/` |
| Biết phải làm những gì, scope bao nhiêu, use case nào | `03-requirements/` |
| Xem kiến trúc code, DB schema, API endpoints | `04-architecture/` |
| Xem task nào đang làm, làm đến đâu rồi | `05-planning/` |
| Tại sao chọn .NET thay vì Spring Boot? Tại sao có role "Khảo thí"? | `06-decisions/` |

---

## 📝 Quy tắc khi thêm file mới

1. **File docx/pdf gốc từ GVHD** → `01-assignment/`
2. **File mô tả kiến trúc** → `04-architecture/`
3. **File quyết định mới** → thêm vào `06-decisions/decisions.md`
4. **Task mới / cập nhật tiến độ** → cập nhật `05-planning/plan.md`
5. **Đặt tên file theo mục đích, không theo ngày** (ví dụ `auth-design.md`, không phải `2026-09-09.md`)

---

## 🔗 Tài liệu liên quan

- Backend code: [`../Backend_Server1/`](../Backend_Server1/)
- Đề bài đầy đủ: [`01-assignment/Phieu_FA26SE034.docx`](01-assignment/Phieu_FA26SE034.docx)
