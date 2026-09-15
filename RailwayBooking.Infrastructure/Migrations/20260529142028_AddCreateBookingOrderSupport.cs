using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateBookingOrderSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_booking_orders_UserId",
                table: "booking_orders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "booking_orders");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "booking_orders",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_booking_orders_UserId",
                table: "booking_orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_booking_orders_HoldToken",
                table: "booking_orders",
                column: "HoldToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_booking_orders_HoldToken",
                table: "booking_orders");

            migrationBuilder.DropIndex(
                name: "IX_booking_orders_UserId",
                table: "booking_orders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "booking_orders");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "booking_orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_booking_orders_UserId",
                table: "booking_orders",
                column: "UserId");
        }
    }
}
