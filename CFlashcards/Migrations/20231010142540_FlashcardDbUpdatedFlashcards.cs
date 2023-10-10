using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CFlashcards.Migrations
{
    /// <inheritdoc />
    public partial class FlashcardDbUpdatedFlashcards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLanguageFlashcard",
                table: "Flashcards",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLanguageFlashcard",
                table: "Flashcards");
        }
    }
}
