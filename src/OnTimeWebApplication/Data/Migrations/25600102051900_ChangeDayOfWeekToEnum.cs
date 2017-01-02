using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnTimeWebApplication.Data.Migrations
{
    public partial class ChangeDayOfWeekToEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DayOfWeek",
                table: "SubjectTimes",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DayOfWeek",
                table: "SubjectTimes",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
