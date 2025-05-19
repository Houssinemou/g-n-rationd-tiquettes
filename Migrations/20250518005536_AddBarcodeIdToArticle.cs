using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace générationdétiquettes.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodeIdToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarcodeId",
                table: "Articles",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeId",
                table: "Articles");
        }
    }
}
