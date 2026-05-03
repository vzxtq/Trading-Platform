using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingEngine.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "UserIdentities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiresAt",
                table: "UserIdentities",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "UserIdentities");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiresAt",
                table: "UserIdentities");
        }
    }
}
