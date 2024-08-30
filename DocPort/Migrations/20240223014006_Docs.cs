using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocPort.Migrations
{
    /// <inheritdoc />
    public partial class Docs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Document");

            migrationBuilder.CreateTable(
                name: "Doc",
                schema: "Document",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    LastTimeUpdated = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    IsTrashed = table.Column<bool>(type: "bit", nullable: false),
                    IsTrashedPrime = table.Column<bool>(type: "bit", nullable: false),
                    DirectoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doc", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContentHolder",
                schema: "Document",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    IsTrashed = table.Column<bool>(type: "bit", nullable: false),
                    IsTrashedPrime = table.Column<bool>(type: "bit", nullable: false),
                    DirectoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentHolder", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContentHolder_Doc_DirectoryID",
                        column: x => x.DirectoryID,
                        principalSchema: "Document",
                        principalTable: "Doc",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentHolder_DirectoryID",
                schema: "Document",
                table: "ContentHolder",
                column: "DirectoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentHolder",
                schema: "Document");

            migrationBuilder.DropTable(
                name: "Doc",
                schema: "Document");
        }
    }
}
