using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2019_03_23_1918 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PortalId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_PortalId",
                table: "User",
                column: "PortalId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Portal_PortalId",
                table: "User",
                column: "PortalId",
                principalTable: "Portal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Portal_PortalId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_PortalId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PortalId",
                table: "User");
        }
    }
}
