# Yêu cầu & Phạm vi (Requirements & Scope)

> Tài liệu này mô tả **đầy đủ yêu cầu** của đồ án (lấy từ đề bài + từ chat với ChatGPT) và **phạm vi Server 1 / Flow 1** mà bạn phụ trách.

---

## 1. Yêu cầu chức năng (Functional Requirements)

> Trích từ đề bài + trao đổi với ChatGPT. Các mục **đánh dấu [F1]** thuộc **Flow 1 (phạm vi bạn làm)**. Các mục khác thuộc flow sau hoặc Server 2 — ghi nhận để tham khảo, chưa code ngay.

### 1.1. Administrator
- **[ ]** Quản lý user accounts & roles **[F1 — Authentication/Authorization]**
- **[ ]** Cấu hình examination settings/duration **[F1 — Exam Configuration]**
- **[ ]** Cấu hình examination schedules **[F1 — Exam Configuration]**
- **[ ]** Cấu hình examination rooms **[F1 — Exam Configuration]**
- **[ ]** Cấu hình grading policies **[F1 — Exam Configuration]**
- **[ ]** Cấu hình plagiarism detection policies **[F1 — Exam Configuration]**
- **[ ]** Cấu hình system security policies **[F1 — Exam Configuration]**
- **[ ]** Monitor overall system **[F1]**
- **[ ]** View audit logs **[F1]**
- **[ ]** Generate system reports
- **[ ]** Export examination statistics
- **[ ]** Statistics on the number of submission reviewed for re-evaluation

### 1.2. Lecturer (phần thuộc Flow 1)
- **[ ]** Tạo kỳ thi thực hành (PE) **[F1]**
- **[ ]** Upload đề thi (.docx) **[F1]**
- **[ ]** Upload answer template (.docx) **[F1]**
- **[ ]** Upload grading rubric (.docx / .xlsx) **[F1]**
- **[ ]** Publish đề thi **[F1]**
- **[ ]** Cấu hình AI grading model
- **[ ]** Grade, ReGrade khi sinh viên phúc khảo
- **[ ]** Approve final grades
- **[ ]** Start AI grading
- **[ ]** Review AI-generated scores
- **[ ]** Review AI explanations
- **[ ]** Override AI scores
- **[ ]** Add manual comments
- **[ ]** Compare AI scores với manual scores

### 1.3. Lecturer (phần KHÔNG thuộc Flow 1 — flow sau)
- Upload student submissions
- Review AI-generated scores
- Manually adjust grades
- Approve final scores
- Analyze examination statistics
- Export reports

### 1.4. Student
- **[ ]** Log in securely **[F1]**
- **[ ]** View examination information **[F1 — phần view cơ bản]**
- **[ ]** Download examination files
- **[ ]** Download answer templates
- **[ ]** Upload submissions
- **[ ]** Track submission status
- **[ ]** Receive submission confirmation
- **[ ]** View examination results
- **[ ]** View lecturer feedback
- **[ ]** View AI feedback (nếu được phép)

### 1.5. Examination Monitoring (flow sau / Server 2)
- Monitor students during examinations
- Review suspicious activities
- View cheating alerts
- Review examination logs
- Replay examination timelines

### 1.6. Analytics (flow sau)
- Generate grading statistics
- Analyze score distributions
- View AI grading accuracy
- Export reports
- Highlight failed students
- Highlight students receiving zero scores

### 1.7. Examination Environment Configuration (flow sau)
- Approved/blocked software
- Approved IDEs/browsers/AI tools
- Internet access policies
- Clipboard / File / External device permissions
- Screen recording policies

### 1.8. Student Activity Monitoring (flow sau)
- Application launch / switching history
- Browser history
- Active window changes
- File open / modify history
- Clipboard usage
- Screenshot events
- Multiple monitor detection
- USB insertion/removal
- Network changes
- Login/logout history
- Submission timestamps

### 1.9. Cheating Detection (flow sau)
- Switch to unauthorized apps
- Excessive app switching
- Prohibited websites
- Copy/paste from external
- Frequent clipboard activity
- Simultaneous logins
- Abnormal idle periods
- Suspicious file access
- Multiple submission attempts
- Unusual editing patterns
- **Suspicion Score** cho mỗi sinh viên

### 1.10. OCR Processing (flow sau — khi nhận bài)
- Extract text from images
- Separate images into preview panel
- Highlight extracted text
- Compare extracted text vs original image

### 1.11. AI Explainability (flow sau)
- Score justification
- Rubric mapping
- Missing requirements
- Suggested improvements
- Confidence score
- Supporting evidence

### 1.12. Examination Analytics (flow sau)
- Score distribution charts
- AI vs lecturer score comparisons
- Rubric achievement analysis
- Submission time analysis
- Student performance analysis
- Suspicious behavior statistics
- AI grading accuracy reports
- Examination quality reports

### 1.13. Export Functions (flow sau)
- Grade reports
- AI grading reports
- Examination logs
- Cheating reports
- Audit reports
- Statistical reports
- Format: Excel / PDF / Docx

---

## 2. Yêu cầu phi chức năng (Non-functional Requirements)

### Performance
> Nền tảng phải chấm được **hàng trăm bài nộp** với AI-assisted analysis trong thời gian phản hồi chấp nhận được.

### Scalability
> Hỗ trợ nhiều course/exam/user đồng thời, với AI services dạng modular có thể mở rộng trong tương lai.

### Security
> Authentication, role-based authorization, secure document storage, audit logging, data privacy cho examination materials và student submissions.

### Operational
- **Server 2 không có Internet** — đây là constraint bắt buộc, ảnh hưởng thiết kế
- **AI services** chạy ở Server 1 hoặc tách sang Python service cùng Server 1
- **Kết nối 2 server:** controlled link, không phải Internet công khai

---

## 3. Use cases nghiệp vụ (theo flow nghiệp vụ)

### Flow 1 — Chuẩn bị đề (Server 1)

| # | Actor | Use case | Mô tả ngắn |
|---|---|---|---|
| UC-01 | Guest | Register | Tạo tài khoản mới |
| UC-02 | User | Login | Đăng nhập |
| UC-03 | User | Logout | Đăng xuất |
| UC-04 | User | View profile | Xem thông tin cá nhân |
| UC-05 | Admin | Manage users | CRUD user, gán role |
| UC-06 | Admin | Manage roles | Tạo/sửa role & permissions |
| UC-07 | Admin | Configure system settings | Cấu hình policies chung |
| UC-08 | Admin | View audit logs | Xem lịch sử thao tác |
| UC-09 | Khảo thí | Create semester | Tạo học kỳ |
| UC-10 | Khảo thí | Create subject | Tạo môn học |
| UC-11 | Khảo thí | Create subject offering | Mở môn trong học kỳ |
| UC-12 | Khảo thí | Create exam | Tạo kỳ thi cho subject offering |
| UC-13 | Khảo thí | Assign lecturer | Gán giảng viên cho exam |
| UC-14 | Lecturer | Login | Đăng nhập (tái sử dụng UC-02) |
| UC-15 | Lecturer | Upload exam question | Upload file đề |
| UC-16 | Lecturer | Upload answer template | Upload file answer template |
| UC-17 | Lecturer | Upload rubric | Upload file rubric |
| UC-18 | Lecturer | Submit exam | Nộp bộ đề để xét duyệt |
| UC-19 | Khảo thí | Review / check exam | Kiểm tra bộ đề |
| UC-20 | Khảo thí | Lock exam | Khoá bộ đề |
| UC-21 | System | Package exam | Đóng gói thành exam package |
| UC-22 | System | Transfer package to Server 2 | Gửi package sang Server 2 |
| UC-23 | Admin/User | View audit logs | Xem log (cùng UC-08) |

> Use case `UC-01 → UC-22` chính là **Flow 1** mà bạn phải code backend.

### Flow 2 — Sinh viên thi (Server 2 — không phải phần bạn)
- Receive exam
- Exam session
- Student access
- Anti-cheating
- Student submit
- Return submission

### Flow 3 — Chấm bài (Server 1 — bạn làm nhưng là flow SAU)
- Receive submission
- Extract (Layout → OCR/CV → Structured)
- AI Grading vs Rubric
- Score + Explainability

---

## 4. Phạm vi (Scope) — Server 1 / Flow 1

### IN-SCOPE (làm trong đợt này)
1. **Authentication / Authorization**
   - Register, Login, Logout, JWT/session
   - Role-based: Admin, Khảo thí (Examination Office), Lecturer (cơ bản)
   - Audit log cho mọi hành động quan trọng
2. **Quản lý danh mục**
   - Semester
   - Subject
   - Subject Offering
3. **Quản lý Exam**
   - Create exam
   - Assign lecturer
4. **Quản lý Exam Submission (bộ đề)**
   - Upload 3 loại file: Question (.docx) | Answer Template (.docx) | Rubric (.docx/.xlsx)
   - Validate cơ bản (file type, size, định dạng)
   - Submit (chuyển trạng thái Submitted)
   - Review / Check (Khảo thí xét duyệt)
   - Lock (khoá bộ đề — không sửa được)
   - Package (đóng gói thành ExamPackage)
5. **Transfer**
   - Gửi package sang Server 2 (qua controlled link — ghi nhận, có thể mock/stub giai đoạn đầu)
6. **Audit log**
   - Ghi log cho các thao tác chính (CRUD, submit, lock, transfer)
7. **Swagger/OpenAPI** để test thủ công

### OUT-OF-SCOPE (chưa làm trong đợt này)
- Anti-cheating, activity monitoring, suspicion score
- Student login/nộp bài (thuộc Server 2)
- Extract / OCR / AI grading / scoring
- Analytics, dashboard, charts
- Export báo cáo Excel/PDF/Docx
- AI model configuration / explainability
- Notification, email
- Realtime (SignalR) — có thể thêm sau nếu cần

### DEFERRED (làm sau nhưng đã ghi nhận)
- Extract pipeline (PDF/Image → Structured) — chỉ định nghĩa interface/IPython service call
- AI grading engine — chỉ định nghĩa interface
- Re-grade flow (sinh viên phúc khảo)

---

## 5. Actors của hệ thống

| Actor | Mô tả | Quyền trong Flow 1 |
|---|---|---|
| **Admin** | Quản trị hệ thống | Full access: user, role, config, audit |
| **Examination Office (Khảo thí)** | Phòng khảo thí — tổ chức thi | Semester, Subject, Subject Offering, Exam, Assign Lecturer, Review, Lock, Package, Transfer |
| **Lecturer** | Giảng viên | Upload file đề/answer template/rubric cho exam được phân công, Submit |
| **Student** | Sinh viên | (Flow 1 chỉ cần login + view thông tin cơ bản; nộp bài là flow sau) |

> **Lưu ý về role:** Trong đề bài có "Administrator" và "Lecturer". Khi trao đổi với ChatGPT, bạn bổ sung thêm vai trò "Khảo thí" (Examination Office). Cần xác nhận lại với GVHD nếu chưa rõ. Xem `decisions.md` mục D-002.

---

## 6. Một số nghiệp vụ cần làm rõ thêm

Mục này ghi lại các **câu hỏi nghiệp vụ còn mơ hồ** cần bạn/giảng viên xác nhận trước khi code:

- **[Q-01]** SubjectOffering cần lưu thêm thông tin gì? (số tín chỉ, mã lớp, sĩ số tối đa, ngày bắt đầu/kết thúc…)
- **[Q-02]** "Assign Lecturer" cho phép một hay nhiều lecturer cho một exam?
- **[Q-03]** Lecturer có được upload nhiều bộ file khác nhau cho cùng một exam không (nhiều version đề)?
- **[Q-04]** Workflow Review/Check: Lecturer có thể tự sửa sau khi Submit và bị từ chối? Hay phải tạo submission mới?
- **[Q-05]** "Lock" có phải trạng thái cuối cùng trước khi Package? Có cho phép un-lock không?
- **[Q-06]** Rubric upload: file Word hay Excel? Có template mẫu nào không?
- **[Q-07]** Có cần lưu version cho ExamSubmission không (track lịch sử chỉnh sửa)?
- **[Q-08]** Khảo thí có quyền upload file thay Lecturer không, hay Lecturer là người duy nhất upload?
- **[Q-09]** Một Lecturer có được phụ trách nhiều exam khác nhau không?
- **[Q-10]** Trong Flow 1 có cần thiết kế API cho Student login không, hay Student chỉ xuất hiện ở Flow sau?

---

## 7. Acceptance criteria cho Flow 1 (phiên bản đầu)

Khi hoàn thành Flow 1, các tiêu chí tối thiểu:

1. ✅ User có thể register và login bằng email + password (hoặc username + password).
2. ✅ Hệ thống có 3 role chính: Admin, Khảo thí, Lecturer. Student có thể không active trong Flow 1.
3. ✅ JWT/session token được cấp sau login và được verify trên mọi request bảo vệ.
4. ✅ Admin có thể CRUD user và gán role.
5. ✅ Khảo thí có thể tạo Semester → Subject → Subject Offering → Exam và Assign Lecturer.
6. ✅ Lecturer được gán có thể upload 3 file cho exam của mình: Question, Answer Template, Rubric.
7. ✅ Submit → Review → Lock → Package hoạt động đúng trạng thái (state machine).
8. ✅ Audit log ghi nhận được tất cả thao tác quan trọng (CRUD entity + state transition).
9. ✅ Swagger UI chạy được và expose đủ các endpoint của Flow 1.
10. ✅ Backend chạy được trên local với PostgreSQL (hoặc InMemoryDb cho giai đoạn dev).

---

## 8. Out of Flow 1 nhưng vẫn thuộc Server 1 (để tham khảo)

Các module này **KHÔNG làm** trong Flow 1 nhưng vẫn thuộc Server 1, nên cần dành chỗ trong kiến trúc:
- Extract Orchestration
- AI Grading Integration (gọi Python service)
- Submission/Review (nhận bài từ Server 2)
- Analytics, Reports, Export

Xem chi tiết trong `architecture.md`.
