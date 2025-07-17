using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class initDynamicSpaceDynamicEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DynamicSpaceDynamicEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DynamicTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceDynamicEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceDynamicEntity_DynamicSpaceDynamicType_DynamicTypeId",
                        column: x => x.DynamicTypeId,
                        principalTable: "DynamicSpaceDynamicType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DynamicSpaceDynamicEntityAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DynamicEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DynamicAttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Format = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceDynamicEntityAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceDynamicEntityAttribute_DynamicSpaceDynamicAttribute_DynamicAttributeId",
                        column: x => x.DynamicAttributeId,
                        principalTable: "DynamicSpaceDynamicAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceDynamicEntityAttribute_DynamicSpaceDynamicEntity_DynamicEntityId",
                        column: x => x.DynamicEntityId,
                        principalTable: "DynamicSpaceDynamicEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DynamicSpaceDynamicEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DynamicEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceDynamicEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceDynamicEntry_DynamicSpaceDynamicEntity_DynamicEntityId",
                        column: x => x.DynamicEntityId,
                        principalTable: "DynamicSpaceDynamicEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicEntity_DynamicTypeId",
                table: "DynamicSpaceDynamicEntity",
                column: "DynamicTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicEntityAttribute_DynamicAttributeId",
                table: "DynamicSpaceDynamicEntityAttribute",
                column: "DynamicAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicEntityAttribute_DynamicEntityId",
                table: "DynamicSpaceDynamicEntityAttribute",
                column: "DynamicEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceDynamicEntry_DynamicEntityId",
                table: "DynamicSpaceDynamicEntry",
                column: "DynamicEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicSpaceDynamicEntityAttribute");

            migrationBuilder.DropTable(
                name: "DynamicSpaceDynamicEntry");

            migrationBuilder.DropTable(
                name: "DynamicSpaceDynamicEntity");
        }
    }
}
