using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSeatHoldUserIdToGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_seat_holds_UserId",
                table: "seat_holds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "seat_holds");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "seat_holds",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.CreateIndex(
                name: "IX_seat_holds_UserId",
                table: "seat_holds",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_seat_holds_UserId",
                table: "seat_holds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "seat_holds");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "seat_holds",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_seat_holds_UserId",
                table: "seat_holds",
                column: "UserId");
        }
    }
}
