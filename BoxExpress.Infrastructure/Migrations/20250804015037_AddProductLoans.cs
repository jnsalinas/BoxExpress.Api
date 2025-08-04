using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductLoans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponsibleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLoans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLoans_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductLoans_Users_ProcessedById",
                        column: x => x.ProcessedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductLoans_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductLoanDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductLoanId = table.Column<int>(type: "int", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<int>(type: "int", nullable: false),
                    DeliveredQuantity = table.Column<int>(type: "int", nullable: false),
                    ReturnedQuantity = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLoanDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductLoanDetails_ProductLoans_ProductLoanId",
                        column: x => x.ProductLoanId,
                        principalTable: "ProductLoans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductLoanDetails_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLoanDetails_ProductLoanId",
                table: "ProductLoanDetails",
                column: "ProductLoanId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLoanDetails_ProductVariantId",
                table: "ProductLoanDetails",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLoans_CreatedById",
                table: "ProductLoans",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLoans_ProcessedById",
                table: "ProductLoans",
                column: "ProcessedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLoans_WarehouseId",
                table: "ProductLoans",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductLoanDetails");

            migrationBuilder.DropTable(
                name: "ProductLoans");
        }
    }
}
