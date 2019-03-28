using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2019_03_25_2114 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteResult",
                table: "VoteResult");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "VoteResult",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "VoteItem",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteResult",
                table: "VoteResult",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VoteResult",
                table: "VoteResult");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "VoteResult");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "VoteItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VoteResult",
                table: "VoteResult",
                columns: new[] { "VoteId", "VoteItemId" });
        }
    }
}
