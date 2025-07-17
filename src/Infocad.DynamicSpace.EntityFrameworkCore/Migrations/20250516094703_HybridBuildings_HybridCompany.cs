using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class HybridBuildings_HybridCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicSpaceCompany");

            migrationBuilder.AddColumn<bool>(
                name: "IsHybrid",
                table: "DynamicSpaceDynamicEntity",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DynamicSpaceHybridBuilding",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DynamicEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    X = table.Column<float>(type: "real", nullable: false),
                    Y = table.Column<float>(type: "real", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceHybridBuilding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceHybridBuilding_DynamicSpaceDynamicEntity_DynamicEntityId",
                        column: x => x.DynamicEntityId,
                        principalTable: "DynamicSpaceDynamicEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DynamicSpaceHybridCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DynamicEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RagioneSociale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cognome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Indirizzo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceHybridCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceHybridCompany_DynamicSpaceDynamicEntity_DynamicEntityId",
                        column: x => x.DynamicEntityId,
                        principalTable: "DynamicSpaceDynamicEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceHybridBuilding_DynamicEntityId",
                table: "DynamicSpaceHybridBuilding",
                column: "DynamicEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceHybridCompany_DynamicEntityId",
                table: "DynamicSpaceHybridCompany",
                column: "DynamicEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DynamicSpaceHybridBuilding");

            migrationBuilder.DropTable(
                name: "DynamicSpaceHybridCompany");

            migrationBuilder.DropColumn(
                name: "IsHybrid",
                table: "DynamicSpaceDynamicEntity");

            migrationBuilder.CreateTable(
                name: "DynamicSpaceCompany",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cognome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DynamicEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Indirizzo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RagioneSociale = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DynamicSpaceCompany", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DynamicSpaceCompany_DynamicSpaceDynamicEntity_DynamicEntityId",
                        column: x => x.DynamicEntityId,
                        principalTable: "DynamicSpaceDynamicEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DynamicSpaceCompany_DynamicEntityId",
                table: "DynamicSpaceCompany",
                column: "DynamicEntityId");
        }
    }
}
