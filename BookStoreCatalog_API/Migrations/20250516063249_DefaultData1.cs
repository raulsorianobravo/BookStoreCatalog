using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreCatalog_API.Migrations
{
    /// <inheritdoc />
    public partial class DefaultData1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "AuthorUrl", "CreatedAt", "Description", "DescriptionUrl", "Title", "TitleUrl", "idBook" },
                values: new object[] { 2, "Author992", "URL992", new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Description992", "URL992", "Title992", "URL992", "2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
