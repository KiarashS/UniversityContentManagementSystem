using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_08_09_1903 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "ContentGallery",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MediaType",
                table: "ContentGallery",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "ContentGallery");

            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "ContentGallery");
        }
    }
}
