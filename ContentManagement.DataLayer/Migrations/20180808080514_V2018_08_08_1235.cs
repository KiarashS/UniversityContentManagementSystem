using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_08_08_1235 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GalleryPosition",
                table: "Content",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContentGallery",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContentId = table.Column<long>(nullable: false),
                    Imagename = table.Column<string>(nullable: false),
                    Priority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentGallery", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentGallery_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentGallery_ContentId",
                table: "ContentGallery",
                column: "ContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentGallery");

            migrationBuilder.DropColumn(
                name: "GalleryPosition",
                table: "Content");
        }
    }
}
