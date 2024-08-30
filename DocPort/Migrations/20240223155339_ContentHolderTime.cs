using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocPort.Migrations
{
    /// <inheritdoc />
    public partial class ContentHolderTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateCreated",
                schema: "Document",
                table: "ContentHolder",
                newName: "LastTimeUpdated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastTimeUpdated",
                schema: "Document",
                table: "ContentHolder",
                newName: "DateCreated");
        }
    }
}
