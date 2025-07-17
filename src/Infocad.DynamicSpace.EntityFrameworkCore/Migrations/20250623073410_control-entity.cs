using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class controlentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DynamicControlId",
                table: "DynamicSpaceDynamicEntityAttribute",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicEntityAttribute_DynamicControlId",
                table: "DynamicSpaceDynamicEntityAttribute",
                column: "DynamicControlId");

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicSpaceDynamicEntityAttribute_DynamicSpaceDynamicControl_DynamicControlId",
                table: "DynamicSpaceDynamicEntityAttribute",
                column: "DynamicControlId",
                principalTable: "DynamicSpaceDynamicControl",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DynamicSpaceDynamicEntityAttribute_DynamicSpaceDynamicControl_DynamicControlId",
                table: "DynamicSpaceDynamicEntityAttribute");

            migrationBuilder.DropIndex(
                name: "IX_DynamicSpaceDynamicEntityAttribute_DynamicControlId",
                table: "DynamicSpaceDynamicEntityAttribute");

            migrationBuilder.DropColumn(
                name: "DynamicControlId",
                table: "DynamicSpaceDynamicEntityAttribute");
        }
    }
}
