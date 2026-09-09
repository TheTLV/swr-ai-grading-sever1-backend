# Enum & Trạng thái — Ghi nhận thảo luận

> **File này** ghi lại **các quyết định về enum, trạng thái, và cấu trúc kỳ thi** sau khi trao đổi với user.
> Cập nhật liên tục. File thuộc `doc/` → **PUSH lên git** (cùng team).

---

## 1. User Roles — 3 bảng riêng

> **⚠️ 2026-09-09 — CHƯA SỬA CODE, CHỈ GHI NHẬN**

User xác nhận: **không có bảng `users` chung**, mà chia thành **3 bảng riêng**:

| Bảng | Mô tả | Vai trò |
|---|---|---|
| `students` | Thông tin sinh viên | Student |
| `lecturers` | Thông tin giảng viên | Lecturer |
| `examination_offices` | Thông tin nhân viên phòng khảo thí | ExaminationOffice |

**Tác động:**
- Entity `User` hiện tại trong code sẽ bị thay bằng 3 entity riêng
- Auth/Seeder cần sửa lại (mỗi bảng có username/password riêng)
- Quyền truy cập API phân biệt theo role
- **Hiện tại: CHƯA SỬA** — chờ giai đoạn sau

**Tạm thời giữ nguyên:** Entity `User` trong code vẫn dùng enum `UserRoleEnum` để compile.

**Cần hỏi thêm:**
- [ ] 3 bảng này có trường chung nào cần tách ra bảng `accounts` (username, password_hash, email) không?
- [ ] Sinh viên có lớp học (class) không? Nếu có thì thuộc bảng nào?
- [ ] Lecturer có thuộc khoa/phòng ban nào không?

---

## 2. Semester Code Format — FPT Academic Style

### 2.1. Mã kỳ theo FPT

**Format:** `{Block}{Year2Digits}`

| Block | Ý nghĩa | Tháng bắt đầu | Độ dài |
|---|---|---|---|
| `FA` | Fall (Học kỳ chính) | Tháng 9 | ~10 tuần |
| `SP` | Spring (Học kỳ chính) | Tháng 1 | ~10 tuần |
| `SU` | Summer (Học kỳ ngắn) | Tháng 6 | ~5 tuần |

**Ví dụ:**
- `FA26` = Fall 2026 (09/2026 – 12/2026)
- `SP27` = Spring 2027 (01/2027 – 05/2027)
- `SU26` = Summer 2026 (06/2026 – 08/2026)

### 2.2. Validation rules

```
[FA|SP|SU][0-9]{2}   // VD: FA26, SP27, SU26
- Không phân biệt hoa/thường (lưu uppercase)
- Year phải trong khoảng: 2025 → 2035
- Code phải UNIQUE
- Độ dài: 4 ký tự
```

### 2.3. Mối quan hệ thời gian

```
FA = Fall (09 → 12)
SP = Spring (01 → 05)
SU = Summer (06 → 08)

1 năm học FPT = FA(09-12) + SP(01-05) + SU(06-08)
```

### 2.4. Đề xuất cấu trúc Semester entity (chưa sửa, ghi nhận)

```csharp
public class Semester
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(4)]
    [RegularExpression(@"^(FA|SP|SU)\d{2}$",
        ErrorMessage = "Code phải theo format: FA26, SP27, SU26")]
    public string Code { get; set; }        // VD: "FA26"

    [Required]
    [MaxLength(200)]
    public string Name { get; set; }         // VD: "Fall 2026"

    public DateTime StartDate { get; set; }  // VD: 2026-09-07
    public DateTime EndDate { get; set; }    // VD: 2026-12-13

    // Validation tự động:
    // - FA: StartDate phải trong [08-01, 09-30]
    // - SP: StartDate phải trong [12-01, 01-31]
    // - SU: StartDate phải trong [05-01, 06-30]
    // - EndDate - StartDate: FA/SP ≈ 70-100 ngày (10 tuần), SU ≈ 35-50 ngày (5 tuần)

    public SemesterStatus Status { get; set; } // Upcoming | Active | Closed
}
```

---

## 3. Examination Status — Theo nghiệp vụ thực tế

### 3.1. Mô tả nghiệp vụ (bạn giải thích)

```
Lecturer upload đề
    ↓
Lecturer submit bộ đề
    ↓
[AI tự động validate] ← KIỂM TRA RUBRIC
    - Tổng điểm có = 10 không?
    - Đề có đúng format không?
    - Rubric có đủ mục không?
    ↓
Khảo thí xem lại (lần 2)
    ↓
Lock → Nén + Mã hóa → Gửi sang Server 2
```

> **Quan trọng:** Khảo thí Server 2 **không mở được file** sau khi mã hóa → vì bảo mật đề thi.
> Nên AI validate là bước kiểm tra cuối cùng trước khi khóa.

### 3.2. Đề xuất trạng thái — PHIÊN BẢN MỚI

> **Thay thế hoàn toàn `ExaminationStatus` cũ** trong code.

```csharp
/// <summary>
/// Trạng thái của bộ đề thi (bộ file) sau khi Lecturer upload và submit.
/// Đây là trạng thái của ExamSubmission/ExamPackage, không phải Exam session.
/// </summary>
public enum ExamSubmissionStatus
{
    /// <summary>Draft — Lecturer đang tạo, chưa submit.</summary>
    Draft = 0,

    /// <summary>Submitted — Lecturer đã submit, chờ AI validate.</summary>
    Submitted = 1,

    /// <summary>AI_Validated — AI đã kiểm tra rubric, kết quả OK hoặc có cảnh báo.</summary>
    AI_Validated = 2,

    /// <summary>Under_Review — Đang được Khảo thí kiểm tra thủ công.</summary>
    Under_Review = 3,

    /// <summary>Approved — Khảo thí duyệt, sẵn sàng đóng gói.</summary>
    Approved = 4,

    /// <summary>Rejected — Khảo thí từ chối, có ghi chú, Lecturer sửa lại.</summary>
    Rejected = 5,

    /// <summary>Locked — Bộ đề đã bị khóa, không cho sửa.</summary>
    Locked = 6,

    /// <summary>Packaged — Đã nén và đóng gói.</summary>
    Packaged = 7,

    /// <summary>Transferred — Đã gửi sang Server 2.</summary>
    Transferred = 8
}
```

### 3.3. Các bước chuyển trạng thái

```
Draft
  │
  │ Lecturer submit
  ▼
Submitted
  │
  │ AI validate (tự động)
  ▼
AI_Validated ──⚠️ Cảnh báo──→ [AI_Warning] (cờ hiệu, vẫn đi tiếp)
  │
  │ Khảo thí mở review
  ▼
Under_Review
  │
  ├─ Khảo thí duyệt ─→ Approved
  │                      │
  │                      │ Khóa để đóng gói
  │                      ▼
  │                    Locked
  │                      │
  │                      │ Nén + mã hóa
  │                      ▼
  │                    Packaged
  │                      │
  │                      │ Gửi sang Server 2
  │                      ▼
  │                    Transferred ✓
  │
  └─ Khảo thí từ chối ─→ Rejected
                               │
                               │ Lecturer sửa + submit lại
                               ▼
                             Submitted (lại từ đầu)
```

### 3.4. AI Validation — Chi tiết kiểm tra gì?

```
AI Validate thực hiện:
1. Tổng điểm rubric = 10?
2. Đề có đúng số câu/mục như rubric không?
3. File đề (.docx) đọc được không?
4. Answer template có match với đề không?
5. Có suspicious content không?

→ Kết quả: { passed: bool, warnings: string[], score_sum: int }
```

### 3.5. So sánh với enum cũ

| Enum cũ | Thay thế bằng | Ghi chú |
|---|---|---|
| `ExaminationStatus.Draft` | `ExamSubmissionStatus.Draft` | |
| `ExaminationStatus.Ongoing` | *(không dùng trong Flow 1)* | Dành cho Exam session (Server 2) |
| `ExaminationStatus.Closed` | *(không dùng trong Flow 1)* | Dành cho Exam session |
| `ExaminationStatus.Completed` | `Transferred` | Đã gửi = hoàn thành |
| *(không có)* | `AI_Validated` | **MỚI** — bước AI kiểm tra |
| *(không có)* | `Under_Review` | **MỚI** |
| *(không có)* | `Locked` | **MỚI** |
| *(không có)* | `Packaged` | **MỚI** |

---

## 4. Các câu hỏi cần xác nhận thêm

| # | Câu hỏi | Trạng thái |
|---|---|---|
| Q-A | 3 bảng Student/Lecturer/ExaminationOffice có trường chung (accounts) không? | ⏳ Chưa hỏi |
| Q-B | Sinh viên có thuộc lớp học (class) không? | ⏳ Chưa hỏi |
| Q-C | Lecturer có thuộc khoa/phòng ban không? | ⏳ Chưa hỏi |
| Q-D | Trạng thái AI_Validated có cần tách thành 2 trạng thái con (AI_OK, AI_Warning) không? | ⏳ Chưa hỏi |
| Q-E | Locked có cho phép Unlock không? | ⏳ Chưa hỏi |
| Q-F | Khi Rejected, Lecturer có được sửa submission cũ hay phải tạo submission mới? | ⏳ Chưa hỏi |

---

## Lịch sử thay đổi

| Ngày | Người | Thay đổi |
|---|---|---|
| 2026-09-09 | AI | Tạo file enums-and-statuses.md — ghi nhận thảo luận về UserRole (3 bảng), Semester code format (FA26), ExamSubmissionStatus mới |
