using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_09_01_2019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FooterSection",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortalId = table.Column<int>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    IsEnable = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FooterSection_Portal_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FooterLink",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FooterSectionId = table.Column<long>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Icon = table.Column<string>(maxLength: 50, nullable: true),
                    IconColor = table.Column<string>(maxLength: 10, nullable: true),
                    Url = table.Column<string>(nullable: false),
                    IsBlankUrlTarget = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FooterLink_FooterSection_FooterSectionId",
                        column: x => x.FooterSectionId,
                        principalTable: "FooterSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FooterLink_FooterSectionId",
                table: "FooterLink",
                column: "FooterSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_FooterSection_PortalId",
                table: "FooterSection",
                column: "PortalId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FooterLink");

            migrationBuilder.DropTable(
                name: "FooterSection");
        }
    }
}
