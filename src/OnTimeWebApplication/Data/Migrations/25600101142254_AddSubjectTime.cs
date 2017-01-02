using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnTimeWebApplication.Data.Migrations
{
    public partial class AddSubjectTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lecturer_AspNetUsers_AccountId",
                table: "Lecturer");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Lecturer_LecturerId",
                table: "Subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecturer",
                table: "Lecturer");

            migrationBuilder.RenameTable(
                name: "Lecturer",
                newName: "Lecturers");

            migrationBuilder.RenameIndex(
                name: "IX_Lecturer_AccountId",
                table: "Lecturers",
                newName: "IX_Lecturers_AccountId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AbsentTime",
                table: "Subjects",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LateTime",
                table: "Subjects",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "UseComeAbsent",
                table: "Subjects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecturers",
                table: "Lecturers",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "SubjectTimes",
                columns: table => new
                {
                    SubjectId = table.Column<string>(maxLength: 7, nullable: false),
                    Section = table.Column<byte>(nullable: false),
                    DayOfWeek = table.Column<string>(maxLength: 3, nullable: false),
                    End = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTimes", x => new { x.SubjectId, x.Section, x.DayOfWeek });
                    table.ForeignKey(
                        name: "FK_SubjectTimes_Subjects_SubjectId_Section",
                        columns: x => new { x.SubjectId, x.Section },
                        principalTable: "Subjects",
                        principalColumns: new[] { "Id", "Section" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturers_AspNetUsers_AccountId",
                table: "Lecturers",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Lecturers_LecturerId",
                table: "Subjects",
                column: "LecturerId",
                principalTable: "Lecturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lecturers_AspNetUsers_AccountId",
                table: "Lecturers");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Lecturers_LecturerId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "SubjectTimes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Lecturers",
                table: "Lecturers");

            migrationBuilder.DropColumn(
                name: "AbsentTime",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "LateTime",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "UseComeAbsent",
                table: "Subjects");

            migrationBuilder.RenameTable(
                name: "Lecturers",
                newName: "Lecturer");

            migrationBuilder.RenameIndex(
                name: "IX_Lecturers_AccountId",
                table: "Lecturer",
                newName: "IX_Lecturer_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Lecturer",
                table: "Lecturer",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturer_AspNetUsers_AccountId",
                table: "Lecturer",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Lecturer_LecturerId",
                table: "Subjects",
                column: "LecturerId",
                principalTable: "Lecturer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
