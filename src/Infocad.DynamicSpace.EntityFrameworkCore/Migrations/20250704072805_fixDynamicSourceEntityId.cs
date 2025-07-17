using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class fixDynamicSourceEntityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DynamicSpaceDynamicHierarchyEntities_DynamicSpaceDynamicEntity_DynamicSourceEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DynamicSpaceDynamicHierarchyEntities",
                table: "DynamicSpaceDynamicHierarchyEntities");

            migrationBuilder.AlterColumn<Guid>(
                name: "DynamicSourceEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DynamicSpaceDynamicHierarchyEntities",
                table: "DynamicSpaceDynamicHierarchyEntities",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicHierarchyEntities_DynamicHierarchyId_DynamicSourceEntityId_DynamicTargetEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities",
                columns: new[] { "DynamicHierarchyId", "DynamicSourceEntityId", "DynamicTargetEntityId" },
                unique: true,
                filter: "[DynamicSourceEntityId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicSpaceDynamicHierarchyEntities_DynamicSpaceDynamicEntity_DynamicSourceEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities",
                column: "DynamicSourceEntityId",
                principalTable: "DynamicSpaceDynamicEntity",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DynamicSpaceDynamicHierarchyEntities_DynamicSpaceDynamicEntity_DynamicSourceEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DynamicSpaceDynamicHierarchyEntities",
                table: "DynamicSpaceDynamicHierarchyEntities");

            migrationBuilder.DropIndex(
                name: "IX_DynamicSpaceDynamicHierarchyEntities_DynamicHierarchyId_DynamicSourceEntityId_DynamicTargetEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities");

            migrationBuilder.AlterColumn<Guid>(
                name: "DynamicSourceEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DynamicSpaceDynamicHierarchyEntities",
                table: "DynamicSpaceDynamicHierarchyEntities",
                columns: new[] { "DynamicHierarchyId", "DynamicSourceEntityId", "DynamicTargetEntityId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DynamicSpaceDynamicHierarchyEntities_DynamicSpaceDynamicEntity_DynamicSourceEntityId",
                table: "DynamicSpaceDynamicHierarchyEntities",
                column: "DynamicSourceEntityId",
                principalTable: "DynamicSpaceDynamicEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
