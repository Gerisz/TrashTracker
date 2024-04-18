using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace TrashTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContentTypeToImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "TrashImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Trashes",
                type: "geography",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "TrashImages");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Trashes",
                type: "geography",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography");
        }
    }
}
