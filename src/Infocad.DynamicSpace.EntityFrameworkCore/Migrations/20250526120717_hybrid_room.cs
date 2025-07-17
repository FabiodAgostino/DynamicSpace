using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class hybrid_room : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DynamicSpaceHybridBuilding_DynamicSpaceDynamicEntity_DynamicEntityId",
                table: "DynamicSpaceHybridBuilding");

            migrationBuilder.AlterColumn<Guid>(
                name: "DynamicEntityId",
                table: "DynamicSpaceHybridBuilding",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateTable(
                name: "DynamicSpaceHybridRoom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DynamicEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceHybridRoom", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceHybridRoom_DynamicSpaceDynamicEntity_DynamicEntityId",
                        column: x => x.DynamicEntityId,
                        principalTable: "DynamicSpaceDynamicEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceHybridRoom_DynamicEntityId",
                table: "DynamicSpaceHybridRoom",
                column: "DynamicEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicSpaceHybridBuilding_DynamicSpaceDynamicEntity_DynamicEntityId",
                table: "DynamicSpaceHybridBuilding",
                column: "DynamicEntityId",
                principalTable: "DynamicSpaceDynamicEntity",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DynamicSpaceHybridBuilding_DynamicSpaceDynamicEntity_DynamicEntityId",
                table: "DynamicSpaceHybridBuilding");

            migrationBuilder.DropTable(
                name: "DynamicSpaceHybridRoom");

            migrationBuilder.AlterColumn<Guid>(
                name: "DynamicEntityId",
                table: "DynamicSpaceHybridBuilding",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicSpaceHybridBuilding_DynamicSpaceDynamicEntity_DynamicEntityId",
                table: "DynamicSpaceHybridBuilding",
                column: "DynamicEntityId",
                principalTable: "DynamicSpaceDynamicEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
