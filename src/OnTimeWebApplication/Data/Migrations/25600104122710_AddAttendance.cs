using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnTimeWebApplication.Data.Migrations
{
    public partial class AddAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    SubjectId = table.Column<string>(maxLength: 7, nullable: false),
                    SubjectSection = table.Column<byte>(nullable: false),
                    StudentId = table.Column<string>(maxLength: 10, nullable: false),
                    AttendedTime = table.Column<DateTime>(nullable: false),
                    AttendState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => new { x.SubjectId, x.SubjectSection, x.StudentId, x.AttendedTime });
                    table.ForeignKey(
                        name: "FK_Attendance_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendance_Subjects_SubjectId_SubjectSection",
                        columns: x => new { x.SubjectId, x.SubjectSection },
                        principalTable: "Subjects",
                        principalColumns: new[] { "Id", "Section" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_StudentId",
                table: "Attendance",
                column: "StudentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");
        }
    }
}
