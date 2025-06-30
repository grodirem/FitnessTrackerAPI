using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIntegrationSettingsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("2d85df66-cdcb-428d-b1bd-59082fb0e8d6"));

            migrationBuilder.AddColumn<string>(
                name: "IntegrationSettingsJson",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("bdfaf73f-cafb-40a2-9437-8122690fc06c"), "56c3e308-0d08-42e0-877f-693e484599c3", "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("bdfaf73f-cafb-40a2-9437-8122690fc06c"));

            migrationBuilder.DropColumn(
                name: "IntegrationSettingsJson",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("2d85df66-cdcb-428d-b1bd-59082fb0e8d6"), "5c58b6c7-89b1-4628-9c1e-70d7863c29e5", "User", "USER" });
        }
    }
}
