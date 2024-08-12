using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removePresType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PriceRanges_PrescriptionTypes_PrescriptionTypeID",
                table: "PriceRanges");

            migrationBuilder.DropIndex(
                name: "IX_PriceRanges_PrescriptionTypeID",
                table: "PriceRanges");

            migrationBuilder.DropColumn(
                name: "PrescriptionTypeID",
                table: "PriceRanges");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrescriptionTypeID",
                table: "PriceRanges",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PriceRanges_PrescriptionTypeID",
                table: "PriceRanges",
                column: "PrescriptionTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_PriceRanges_PrescriptionTypes_PrescriptionTypeID",
                table: "PriceRanges",
                column: "PrescriptionTypeID",
                principalTable: "PrescriptionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
