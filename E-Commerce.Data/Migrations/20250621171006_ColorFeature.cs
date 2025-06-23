using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Commerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ColorFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ProductColorImages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 6, 21, 20, 10, 5, 582, DateTimeKind.Local).AddTicks(8632), new Guid("173e5559-0095-486b-944a-63d1e22b63db") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 6, 21, 20, 10, 5, 585, DateTimeKind.Local).AddTicks(2421));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 6, 21, 20, 10, 5, 585, DateTimeKind.Local).AddTicks(3300));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "ProductColorImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 6, 21, 19, 52, 5, 974, DateTimeKind.Local).AddTicks(8557), new Guid("72baaf91-1df7-4505-bfb8-3526b7109dbd") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 6, 21, 19, 52, 5, 988, DateTimeKind.Local).AddTicks(2613));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 6, 21, 19, 52, 5, 988, DateTimeKind.Local).AddTicks(3616));
        }
    }
}
