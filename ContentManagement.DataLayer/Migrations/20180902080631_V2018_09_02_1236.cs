using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_09_02_1236 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SourceAddress = table.Column<string>(nullable: true),
                    ActionBy = table.Column<string>(nullable: true),
                    ActionType = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Portal = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    ActionLevel = table.Column<byte>(nullable: false),
                    ActionDate = table.Column<DateTimeOffset>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityLog");
        }
    }
}
