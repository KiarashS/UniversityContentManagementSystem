using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2019_05_24_1231 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AudioPosition",
                table: "Content",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VideoPosition",
                table: "Content",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContentAudio",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContentId = table.Column<long>(nullable: false),
                    Caption = table.Column<string>(nullable: true),
                    Audioname = table.Column<string>(nullable: false),
                    EnableControls = table.Column<bool>(nullable: false),
                    EnableAutoplay = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentAudio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentAudio_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentVideo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContentId = table.Column<long>(nullable: false),
                    Caption = table.Column<string>(nullable: true),
                    Videoname = table.Column<string>(nullable: false),
                    Width = table.Column<int>(nullable: true),
                    Height = table.Column<int>(nullable: true),
                    EnableControls = table.Column<bool>(nullable: false),
                    EnableAutoplay = table.Column<bool>(nullable: false),
                    Priority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentVideo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentVideo_Content_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Content",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentAudio_ContentId",
                table: "ContentAudio",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentVideo_ContentId",
                table: "ContentVideo",
                column: "ContentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentAudio");

            migrationBuilder.DropTable(
                name: "ContentVideo");

            migrationBuilder.DropColumn(
                name: "AudioPosition",
                table: "Content");

            migrationBuilder.DropColumn(
                name: "VideoPosition",
                table: "Content");
        }
    }
}
