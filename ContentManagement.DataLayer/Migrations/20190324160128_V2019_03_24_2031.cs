using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2019_03_24_2031 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Vote",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortalId = table.Column<int>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsMultiChoice = table.Column<bool>(nullable: false),
                    PublishDate = table.Column<DateTimeOffset>(nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vote_Portal_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoteItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VoteId = table.Column<long>(nullable: false),
                    ItemTitle = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoteItem_Vote_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Vote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VoteResult",
                columns: table => new
                {
                    VoteId = table.Column<long>(nullable: false),
                    VoteItemId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteResult", x => new { x.VoteId, x.VoteItemId });
                    table.ForeignKey(
                        name: "FK_VoteResult_Vote_VoteId",
                        column: x => x.VoteId,
                        principalTable: "Vote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoteResult_VoteItem_VoteItemId",
                        column: x => x.VoteItemId,
                        principalTable: "VoteItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vote_PortalId",
                table: "Vote",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteItem_VoteId",
                table: "VoteItem",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteResult_VoteId",
                table: "VoteResult",
                column: "VoteId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteResult_VoteItemId",
                table: "VoteResult",
                column: "VoteItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoteResult");

            migrationBuilder.DropTable(
                name: "VoteItem");

            migrationBuilder.DropTable(
                name: "Vote");
        }
    }
}
