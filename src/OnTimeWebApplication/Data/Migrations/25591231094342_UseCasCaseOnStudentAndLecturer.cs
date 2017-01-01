using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnTimeWebApplication.Data.Migrations
{
    public partial class UseCasCaseOnStudentAndLecturer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lecturer_AspNetUsers_AccountId",
                table: "Lecturer");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_AccountId",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturer_AspNetUsers_AccountId",
                table: "Lecturer",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_AccountId",
                table: "Students",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lecturer_AspNetUsers_AccountId",
                table: "Lecturer");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_AccountId",
                table: "Students");

            migrationBuilder.AddForeignKey(
                name: "FK_Lecturer_AspNetUsers_AccountId",
                table: "Lecturer",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_AccountId",
                table: "Students",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
