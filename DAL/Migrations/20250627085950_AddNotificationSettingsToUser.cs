using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationSettingsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("623e5f8e-c32f-4666-86c3-db2879ae80f0"));

            migrationBuilder.AddColumn<string>(
                name: "NotificationSettingsJson",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("2d85df66-cdcb-428d-b1bd-59082fb0e8d6"), "5c58b6c7-89b1-4628-9c1e-70d7863c29e5", "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d85df66-cdcb-428d-b1bd-59082fb0e8d6"));

            migrationBuilder.DropColumn(
                name: "NotificationSettingsJson",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("623e5f8e-c32f-4666-86c3-db2879ae80f0"), "ed4d3b86-5c3f-444d-8489-ec512a67b314", "User", "USER" });
        }
    }
}
