using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_09_04_1737 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "FooterLink");

            migrationBuilder.DropColumn(
                name: "IconColor",
                table: "FooterLink");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "FooterLink",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IconColor",
                table: "FooterLink",
                maxLength: 10,
                nullable: true);
        }
    }
}
