using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pronia_self.Migrations
{
    /// <inheritdoc />
    public partial class typeMistakefixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Describtion",
                table: "Slides",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Slides",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Slides",
                newName: "Describtion");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Slides",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
