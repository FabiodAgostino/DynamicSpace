using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class AddRuleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DynamicRuleId",
                table: "DynamicSpaceDynamicEntityAttribute",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DynamicSpaceDynamicRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Rule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceDynamicRule", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicEntityAttribute_DynamicRuleId",
                table: "DynamicSpaceDynamicEntityAttribute",
                column: "DynamicRuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicSpaceDynamicEntityAttribute_DynamicSpaceDynamicRule_DynamicRuleId",
                table: "DynamicSpaceDynamicEntityAttribute",
                column: "DynamicRuleId",
                principalTable: "DynamicSpaceDynamicRule",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DynamicSpaceDynamicEntityAttribute_DynamicSpaceDynamicRule_DynamicRuleId",
                table: "DynamicSpaceDynamicEntityAttribute");

            migrationBuilder.DropTable(
                name: "DynamicSpaceDynamicRule");

            migrationBuilder.DropIndex(
                name: "IX_DynamicSpaceDynamicEntityAttribute_DynamicRuleId",
                table: "DynamicSpaceDynamicEntityAttribute");

            migrationBuilder.DropColumn(
                name: "DynamicRuleId",
                table: "DynamicSpaceDynamicEntityAttribute");
        }
    }
}
