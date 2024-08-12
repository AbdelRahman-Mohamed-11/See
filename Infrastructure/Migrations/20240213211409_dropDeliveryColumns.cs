using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class dropDeliveryColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryCostDetails_DeliveryCostPlanSetup_DeliveryCostPlanSetupId",
                table: "DeliveryCostDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryCostPlanSetup",
                table: "DeliveryCostPlanSetup");

            migrationBuilder.RenameTable(
                name: "DeliveryCostPlanSetup",
                newName: "DeliveryCostPlanSetups");

            migrationBuilder.RenameColumn(
                name: "DeliveryCostPlanSetupId",
                table: "DeliveryCostDetails",
                newName: "DeliveryCostPlanSetupFkId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryCostDetails_DeliveryCostPlanSetupId",
                table: "DeliveryCostDetails",
                newName: "IX_DeliveryCostDetails_DeliveryCostPlanSetupFkId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryCostPlanSetups",
                table: "DeliveryCostPlanSetups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryCostDetails_DeliveryCostPlanSetups_DeliveryCostPlanSetupFkId",
                table: "DeliveryCostDetails",
                column: "DeliveryCostPlanSetupFkId",
                principalTable: "DeliveryCostPlanSetups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryCostDetails_DeliveryCostPlanSetups_DeliveryCostPlanSetupFkId",
                table: "DeliveryCostDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryCostPlanSetups",
                table: "DeliveryCostPlanSetups");

            migrationBuilder.RenameTable(
                name: "DeliveryCostPlanSetups",
                newName: "DeliveryCostPlanSetup");

            migrationBuilder.RenameColumn(
                name: "DeliveryCostPlanSetupFkId",
                table: "DeliveryCostDetails",
                newName: "DeliveryCostPlanSetupId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryCostDetails_DeliveryCostPlanSetupFkId",
                table: "DeliveryCostDetails",
                newName: "IX_DeliveryCostDetails_DeliveryCostPlanSetupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryCostPlanSetup",
                table: "DeliveryCostPlanSetup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryCostDetails_DeliveryCostPlanSetup_DeliveryCostPlanSetupId",
                table: "DeliveryCostDetails",
                column: "DeliveryCostPlanSetupId",
                principalTable: "DeliveryCostPlanSetup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
