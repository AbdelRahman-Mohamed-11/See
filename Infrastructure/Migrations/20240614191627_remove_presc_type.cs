using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class remove_presc_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPrescriptions_PrescriptionTypes_PrescriptionTypeID",
                table: "UserPrescriptions");

            migrationBuilder.DropIndex(
                name: "IX_UserPrescriptions_PrescriptionTypeID",
                table: "UserPrescriptions");

            migrationBuilder.DropColumn(
                name: "PrescriptionTypeID",
                table: "UserPrescriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PrescriptionTypeID",
                table: "UserPrescriptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserPrescriptions_PrescriptionTypeID",
                table: "UserPrescriptions",
                column: "PrescriptionTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPrescriptions_PrescriptionTypes_PrescriptionTypeID",
                table: "UserPrescriptions",
                column: "PrescriptionTypeID",
                principalTable: "PrescriptionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
