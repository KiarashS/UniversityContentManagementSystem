using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_05_08_1743 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Page",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Page_Slug",
                table: "Page",
                column: "Slug",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Page_Slug",
                table: "Page");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Page");
        }
    }
}
