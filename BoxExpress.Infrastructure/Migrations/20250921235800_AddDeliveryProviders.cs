using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxExpress.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourierName",
                table: "OrderStatusHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeliveryProviderId",
                table: "OrderStatusHistories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnRouteEvidenceUrl",
                table: "OrderStatusHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryProviders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryProviders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusHistories_DeliveryProviderId",
                table: "OrderStatusHistories",
                column: "DeliveryProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderStatusHistories_DeliveryProviders_DeliveryProviderId",
                table: "OrderStatusHistories",
                column: "DeliveryProviderId",
                principalTable: "DeliveryProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            var createdAt = DateTime.UtcNow;

            migrationBuilder.InsertData(
                table: "DeliveryProviders",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Ambit", createdAt }
                }
            );

            migrationBuilder.InsertData(
                table: "DeliveryProviders",
                columns: new[] { "Name", "CreatedAt" },
                values: new object[,]
                {
                    { "Repartidor propio", createdAt }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderStatusHistories_DeliveryProviders_DeliveryProviderId",
                table: "OrderStatusHistories");

            migrationBuilder.DropTable(
                name: "DeliveryProviders");

            migrationBuilder.DropIndex(
                name: "IX_OrderStatusHistories_DeliveryProviderId",
                table: "OrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "CourierName",
                table: "OrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "DeliveryProviderId",
                table: "OrderStatusHistories");

            migrationBuilder.DropColumn(
                name: "OnRouteEvidenceUrl",
                table: "OrderStatusHistories");
        }
    }
}
