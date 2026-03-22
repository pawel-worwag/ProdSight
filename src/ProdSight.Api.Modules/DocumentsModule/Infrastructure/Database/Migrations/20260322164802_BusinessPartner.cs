using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProdSight.Api.Modules.DocumentsModule.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class BusinessPartner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessPartners",
                schema: "documents-module",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPartners", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPartners_Name",
                schema: "documents-module",
                table: "BusinessPartners",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessPartners",
                schema: "documents-module");
        }
    }
}
