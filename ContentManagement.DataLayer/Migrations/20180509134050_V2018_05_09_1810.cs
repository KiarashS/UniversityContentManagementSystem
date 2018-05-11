using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_05_09_1810 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Page",
                newName: "Imagename");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Imagename",
                table: "Page",
                newName: "ImageName");
        }
    }
}
