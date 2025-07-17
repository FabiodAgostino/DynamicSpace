using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class AddFormatEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DynamicFormatId",
                table: "DynamicSpaceDynamicEntityAttribute",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "DynamicSpaceDynamicAttribute",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "DynamicSpaceDynamicFormat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AttributeType = table.Column<int>(type: "int", nullable: false),
                    FormatPattern = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceDynamicFormat", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicEntityAttribute_DynamicFormatId",
                table: "DynamicSpaceDynamicEntityAttribute",
                column: "DynamicFormatId");

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicSpaceDynamicEntityAttribute_DynamicSpaceDynamicFormat_DynamicFormatId",
                table: "DynamicSpaceDynamicEntityAttribute",
                column: "DynamicFormatId",
                principalTable: "DynamicSpaceDynamicFormat",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DynamicSpaceDynamicEntityAttribute_DynamicSpaceDynamicFormat_DynamicFormatId",
                table: "DynamicSpaceDynamicEntityAttribute");

            migrationBuilder.DropTable(
                name: "DynamicSpaceDynamicFormat");

            migrationBuilder.DropIndex(
                name: "IX_DynamicSpaceDynamicEntityAttribute_DynamicFormatId",
                table: "DynamicSpaceDynamicEntityAttribute");

            migrationBuilder.DropColumn(
                name: "DynamicFormatId",
                table: "DynamicSpaceDynamicEntityAttribute");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "DynamicSpaceDynamicAttribute",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
