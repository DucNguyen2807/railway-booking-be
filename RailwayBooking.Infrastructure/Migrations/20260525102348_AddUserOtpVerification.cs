using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RailwayBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOtpVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OtpCodeExpiryUtc",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtpCodeHash",
                table: "users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpCodeExpiryUtc",
                table: "users");

            migrationBuilder.DropColumn(
                name: "OtpCodeHash",
                table: "users");
        }
    }
}
