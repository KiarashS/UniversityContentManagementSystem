using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_05_11_2142 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ShowInMainPortal",
                table: "Portal",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RawText",
                table: "Page",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RawText",
                table: "Content",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShowInMainPortal",
                table: "Portal");

            migrationBuilder.DropColumn(
                name: "RawText",
                table: "Page");

            migrationBuilder.DropColumn(
                name: "RawText",
                table: "Content");
        }
    }
}
