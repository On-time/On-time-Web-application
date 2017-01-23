using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnTimeWebApplication.Data.Migrations
{
    public partial class ChangeTelToAndroidId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tel",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Tel",
                table: "Lecturers");

            migrationBuilder.AddColumn<string>(
                name: "AndroidId",
                table: "Students",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AndroidId",
                table: "Lecturers",
                maxLength: 16,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AndroidId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "AndroidId",
                table: "Lecturers");

            migrationBuilder.AddColumn<string>(
                name: "Tel",
                table: "Students",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tel",
                table: "Lecturers",
                maxLength: 10,
                nullable: true);
        }
    }
}
