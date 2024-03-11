using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dotnet_training.Migrations
{
    /// <inheritdoc />
    public partial class ShortenerUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortenerUrls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LongUrl = table.Column<string>(type: "text", nullable: true),
                    ShortUrl = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortenerUrls", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShortenerUrls_Code",
                table: "ShortenerUrls",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortenerUrls");
        }
    }
}
