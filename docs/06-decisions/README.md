# 📌 06-decisions/ — Lịch sử các quyết định

> **Mục đích:** Ghi lại **mọi quyết định quan trọng** đã đưa ra (chọn công nghệ, chọn pattern, chọn scope).
> Đọc khi muốn hiểu **tại sao** thiết kế lại như vậy.

## 📄 File trong thư mục này

| File | Mô tả |
|---|---|
| `decisions.md` | Tất cả quyết định với ID, lý do, hệ quả, status |

## 🔍 Cách đọc

Mỗi quyết định có format:

```markdown
### D-XXX | YYYY-MM-DD | [Tiêu đề]
**Quyết định:** ...
**Lý do:** ...
**Hệ quả:** ...
**Status:** ✅ Đã quyết định / ⏳ Chờ xác nhận
```

## 📋 Trạng thái các quyết định

- **✅ Đã quyết định:** D-001 (.NET), D-003 (2 server), D-004 (Python AI), D-005 (Clean Architecture), D-009 (Flow 1)
- **⏳ Chờ xác nhận:** D-002 (Role Khảo thí), D-006 (PostgreSQL/Supabase), D-007 (Storage), D-008 (Tên project), D-010 (State machine)
- **🔄 Pending:** P-001 → P-005 (xem trong decisions.md)
