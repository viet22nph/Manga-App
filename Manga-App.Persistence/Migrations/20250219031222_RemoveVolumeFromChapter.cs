using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Manga_App.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVolumeFromChapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Chapter");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Volume",
                table: "Chapter",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
