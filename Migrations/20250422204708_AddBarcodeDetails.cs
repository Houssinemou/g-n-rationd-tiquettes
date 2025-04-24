using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace générationdétiquettes.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodeDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Barcodes");

            migrationBuilder.AddColumn<string>(
                name: "CodeFamille",
                table: "Barcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CodeLocalisation",
                table: "Barcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LibelleFamille",
                table: "Barcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LibelleLocalisation",
                table: "Barcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Texte",
                table: "Barcodes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeFamille",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "CodeLocalisation",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "LibelleFamille",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "LibelleLocalisation",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "Texte",
                table: "Barcodes");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Barcodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
