using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PrescriptionForManagerAndUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserPrescriptionId",
                table: "OrderItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoatingTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoatingTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoatingTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LensType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LensTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LensType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrescriptionTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrescriptionCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendorCountries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorCountryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCountries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceRanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationManagerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrescriptionTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LensTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoatingTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorCountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SphereMin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SphereMax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CylinderMin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CylinderMax = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceRanges_CoatingTypes_CoatingTypeID",
                        column: x => x.CoatingTypeID,
                        principalTable: "CoatingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceRanges_LensType_LensTypeID",
                        column: x => x.LensTypeID,
                        principalTable: "LensType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceRanges_Managers_ApplicationManagerID",
                        column: x => x.ApplicationManagerID,
                        principalTable: "Managers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceRanges_PrescriptionTypes_PrescriptionTypeID",
                        column: x => x.PrescriptionTypeID,
                        principalTable: "PrescriptionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceRanges_VendorCountries_VendorCountryID",
                        column: x => x.VendorCountryID,
                        principalTable: "VendorCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPrescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrescriptionTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LensTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoatingTypeID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorCountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfPrescirpiton = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DistanceSphereRight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DistanceSphereLeft = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DistanceCylinderRight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DistanceCylinderLeft = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DistanceRAxis = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DistanceLAxis = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RightAddition = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LeftAddition = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPrescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPrescriptions_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrescriptions_CoatingTypes_CoatingTypeID",
                        column: x => x.CoatingTypeID,
                        principalTable: "CoatingTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrescriptions_LensType_LensTypeID",
                        column: x => x.LensTypeID,
                        principalTable: "LensType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrescriptions_PrescriptionTypes_PrescriptionTypeID",
                        column: x => x.PrescriptionTypeID,
                        principalTable: "PrescriptionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPrescriptions_VendorCountries_VendorCountryID",
                        column: x => x.VendorCountryID,
                        principalTable: "VendorCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceRanges_ApplicationManagerID",
                table: "PriceRanges",
                column: "ApplicationManagerID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRanges_CoatingTypeID",
                table: "PriceRanges",
                column: "CoatingTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRanges_LensTypeID",
                table: "PriceRanges",
                column: "LensTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRanges_PrescriptionTypeID",
                table: "PriceRanges",
                column: "PrescriptionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PriceRanges_VendorCountryID",
                table: "PriceRanges",
                column: "VendorCountryID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrescriptions_CoatingTypeID",
                table: "UserPrescriptions",
                column: "CoatingTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrescriptions_LensTypeID",
                table: "UserPrescriptions",
                column: "LensTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrescriptions_PrescriptionTypeID",
                table: "UserPrescriptions",
                column: "PrescriptionTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrescriptions_UserID",
                table: "UserPrescriptions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPrescriptions_VendorCountryID",
                table: "UserPrescriptions",
                column: "VendorCountryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceRanges");

            migrationBuilder.DropTable(
                name: "UserPrescriptions");

            migrationBuilder.DropTable(
                name: "CoatingTypes");

            migrationBuilder.DropTable(
                name: "LensType");

            migrationBuilder.DropTable(
                name: "PrescriptionTypes");

            migrationBuilder.DropTable(
                name: "VendorCountries");

            migrationBuilder.DropColumn(
                name: "UserPrescriptionId",
                table: "OrderItems");
        }
    }
}
