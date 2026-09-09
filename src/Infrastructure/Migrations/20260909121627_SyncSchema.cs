using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "semesters",
                columns: table => new
                {
                    semester_id = table.Column<Guid>(type: "uuid", nullable: false),
                    semester_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_semesters", x => x.semester_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    cccd = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Discriminator = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    lecturer_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    student_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    major = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "examinations",
                columns: table => new
                {
                    examination_id = table.Column<Guid>(type: "uuid", nullable: false),
                    examination_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    before_time_minutes = table.Column<int>(type: "integer", nullable: false),
                    note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    semester_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_examinations", x => x.examination_id);
                    table.ForeignKey(
                        name: "FK_examinations_semesters_semester_id",
                        column: x => x.semester_id,
                        principalTable: "semesters",
                        principalColumn: "semester_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "submissions",
                columns: table => new
                {
                    submission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    submission_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    folder = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    lecturer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_submissions", x => x.submission_id);
                    table.ForeignKey(
                        name: "FK_submissions_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_submissions_users_lecturer_id",
                        column: x => x.lecturer_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exam_materials",
                columns: table => new
                {
                    exam_material_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exam_material_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    file_question_docs = table.Column<string>(type: "text", nullable: true),
                    file_answer_rubric = table.Column<string>(type: "text", nullable: true),
                    file_answer_template = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_by_id = table.Column<Guid>(type: "uuid", nullable: false),
                    examination_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exam_materials", x => x.exam_material_id);
                    table.ForeignKey(
                        name: "FK_exam_materials_examinations_examination_id",
                        column: x => x.examination_id,
                        principalTable: "examinations",
                        principalColumn: "examination_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_exam_materials_users_create_by_id",
                        column: x => x.create_by_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "gradings",
                columns: table => new
                {
                    grading_id = table.Column<Guid>(type: "uuid", nullable: false),
                    grading_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ai_score = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    lecturer_score = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    ai_logs = table.Column<string>(type: "text", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    update_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    final_score = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    submission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gradings", x => x.grading_id);
                    table.ForeignKey(
                        name: "FK_gradings_submissions_submission_id",
                        column: x => x.submission_id,
                        principalTable: "submissions",
                        principalColumn: "submission_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_gradings_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_exam_materials_create_by_id",
                table: "exam_materials",
                column: "create_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_exam_materials_exam_material_code",
                table: "exam_materials",
                column: "exam_material_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_exam_materials_examination_id",
                table: "exam_materials",
                column: "examination_id");

            migrationBuilder.CreateIndex(
                name: "IX_examinations_examination_code",
                table: "examinations",
                column: "examination_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_examinations_semester_id",
                table: "examinations",
                column: "semester_id");

            migrationBuilder.CreateIndex(
                name: "IX_gradings_grading_code",
                table: "gradings",
                column: "grading_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gradings_submission_id",
                table: "gradings",
                column: "submission_id");

            migrationBuilder.CreateIndex(
                name: "IX_gradings_UserId",
                table: "gradings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_semesters_semester_code",
                table: "semesters",
                column: "semester_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_submissions_lecturer_id",
                table: "submissions",
                column: "lecturer_id");

            migrationBuilder.CreateIndex(
                name: "IX_submissions_UserId",
                table: "submissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_cccd",
                table: "users",
                column: "cccd");

            migrationBuilder.CreateIndex(
                name: "IX_users_lecturer_code",
                table: "users",
                column: "lecturer_code");

            migrationBuilder.CreateIndex(
                name: "IX_users_student_code",
                table: "users",
                column: "student_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exam_materials");

            migrationBuilder.DropTable(
                name: "gradings");

            migrationBuilder.DropTable(
                name: "examinations");

            migrationBuilder.DropTable(
                name: "submissions");

            migrationBuilder.DropTable(
                name: "semesters");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
