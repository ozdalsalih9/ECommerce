using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class last : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 6, 18, 12, 55, 2, 919, DateTimeKind.Local).AddTicks(2503), new Guid("b92e4178-35ae-4099-8a90-b4318e63c4ca") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 6, 18, 12, 55, 2, 921, DateTimeKind.Local).AddTicks(6888));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 6, 18, 12, 55, 2, 921, DateTimeKind.Local).AddTicks(7769));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 6, 18, 12, 25, 11, 850, DateTimeKind.Local).AddTicks(1244), new Guid("ecd1d985-a09f-4870-bd83-a9335289cf76") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 6, 18, 12, 25, 11, 852, DateTimeKind.Local).AddTicks(5421));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 6, 18, 12, 25, 11, 852, DateTimeKind.Local).AddTicks(6446));
        }
    }
}
