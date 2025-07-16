using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ColorDeneme1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 7, 7, 16, 34, 23, 520, DateTimeKind.Local).AddTicks(4615), new Guid("7f3f6938-bce6-4a46-a616-7da461bda6cc") });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 7, 7, 16, 34, 23, 521, DateTimeKind.Local).AddTicks(3779), new Guid("3aeea76e-806f-4698-9352-7be8d7e9f08b") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 7, 7, 16, 34, 23, 520, DateTimeKind.Local).AddTicks(6950));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 7, 7, 16, 34, 23, 520, DateTimeKind.Local).AddTicks(6962));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 7, 7, 16, 24, 19, 575, DateTimeKind.Local).AddTicks(1390), new Guid("9a3a63f5-9ceb-440f-ab33-9d92922b4971") });

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 7, 7, 16, 24, 19, 576, DateTimeKind.Local).AddTicks(299), new Guid("97e14e8a-7c9f-44cb-a37e-012cc5000c9a") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 7, 7, 16, 24, 19, 575, DateTimeKind.Local).AddTicks(3577));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 7, 7, 16, 24, 19, 575, DateTimeKind.Local).AddTicks(3588));
        }
    }
}
