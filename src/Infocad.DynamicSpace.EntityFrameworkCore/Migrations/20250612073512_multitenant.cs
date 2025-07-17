using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class multitenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "DynamicSpaceHybridRoom",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "DynamicSpaceHybridCompany",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "DynamicSpaceHybridBuilding",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "DynamicSpaceDynamicEntry",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DynamicSpaceHybridRoom");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DynamicSpaceHybridCompany");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DynamicSpaceHybridBuilding");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "DynamicSpaceDynamicEntry");
        }
    }
}
