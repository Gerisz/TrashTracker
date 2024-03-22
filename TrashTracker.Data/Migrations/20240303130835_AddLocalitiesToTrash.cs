using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrashTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalitiesToTrash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Locality",
                table: "Trashes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubLocality",
                table: "Trashes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locality",
                table: "Trashes");

            migrationBuilder.DropColumn(
                name: "SubLocality",
                table: "Trashes");
        }
    }
}
