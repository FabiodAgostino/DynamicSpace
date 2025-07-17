using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infocad.DynamicSpace.Migrations
{
    /// <inheritdoc />
    public partial class AddNavgigationDynamicAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NavigationSettings",
                table: "DynamicSpaceDynamicAttribute",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NavigationSettings",
                table: "DynamicSpaceDynamicAttribute");
        }
    }
}
