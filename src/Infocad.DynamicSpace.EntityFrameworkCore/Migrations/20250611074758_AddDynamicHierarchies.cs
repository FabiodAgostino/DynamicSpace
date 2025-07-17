using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class AddDynamicHierarchies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicSpaceDynamicHierarchy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Default = table.Column<bool>(type: "bit", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceDynamicHierarchy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DynamicSpaceDynamicHierarchyEntities",
                columns: table => new
                {
                    DynamicHierarchyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DynamicSourceEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DynamicTargetEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayFields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceDynamicHierarchyEntities", x => new { x.DynamicHierarchyId, x.DynamicSourceEntityId, x.DynamicTargetEntityId });
                    table.ForeignKey(
                        name: "FK_DynamicSpaceDynamicHierarchyEntities_DynamicSpaceDynamicEntity_DynamicSourceEntityId",
                        column: x => x.DynamicSourceEntityId,
                        principalTable: "DynamicSpaceDynamicEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceDynamicHierarchyEntities_DynamicSpaceDynamicHierarchy_DynamicHierarchyId",
                        column: x => x.DynamicHierarchyId,
                        principalTable: "DynamicSpaceDynamicHierarchy",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicHierarchyEntities_DynamicSourceEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities",
                column: "DynamicSourceEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicSpaceDynamicHierarchyEntities");

            migrationBuilder.DropTable(
                name: "DynamicSpaceDynamicHierarchy");
        }
    }
}
