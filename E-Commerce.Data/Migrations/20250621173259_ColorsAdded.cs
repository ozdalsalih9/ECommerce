using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace E_Commerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class ColorsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreateDate", "UserGuid" },
                values: new object[] { new DateTime(2025, 6, 21, 20, 32, 59, 224, DateTimeKind.Local).AddTicks(290), new Guid("f4e87935-694d-4506-becd-7b7a5a69ff1f") });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreateTime",
                value: new DateTime(2025, 6, 21, 20, 32, 59, 226, DateTimeKind.Local).AddTicks(4545));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreateTime",
                value: new DateTime(2025, 6, 21, 20, 32, 59, 226, DateTimeKind.Local).AddTicks(5452));

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "Id", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "#FF0000", true, "Kırmızı" },
                    { 2, "#0000FF", true, "Mavi" },
                    { 3, "#008000", true, "Yeşil" },
                    { 4, "#FFFDD0", true, "Krem" },
                    { 5, "#000000", true, "Siyah" },
                    { 6, "#FFFFFF", true, "Beyaz" },
                    { 7, "#808080", true, "Gri" },
                    { 8, "#000080", true, "Lacivert" },
                    { 9, "#FFFF00", true, "Sarı" },
                    { 10, "#FFA500", true, "Turuncu" },
                    { 11, "#800080", true, "Mor" },
                    { 12, "#FFC0CB", true, "Pembe" },
                    { 13, "#A52A2A", true, "Kahverengi" },
                    { 14, "#F5F5DC", true, "Bej" },
                    { 15, "#4B4B4B", true, "Füme" },
                    { 16, "#2F2F2F", true, "Antrasit" },
                    { 17, "#FFD700", true, "Altın" },
                    { 18, "#C0C0C0", true, "Gümüş" },
                    { 19, "#800000", true, "Bordo" },
                    { 20, "#40E0D0", true, "Turkuaz" },
                    { 21, "#98FF98", true, "Mint" },
                    { 22, "#FFDB58", true, "Hardal" },
                    { 23, "#722F37", true, "Şarap" },
                    { 24, "#006400", true, "Koyu Yeşil" },
                    { 25, "#ADD8E6", true, "Açık Mavi" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 25);

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
    }
}
