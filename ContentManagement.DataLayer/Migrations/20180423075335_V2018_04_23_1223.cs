using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_04_23_1223 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDataProtectionKeys",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FriendlyName = table.Column<string>(nullable: true),
                    XmlData = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppDataProtectionKeys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Portal",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortalKey = table.Column<string>(maxLength: 400, nullable: true),
                    TitleFa = table.Column<string>(maxLength: 200, nullable: false),
                    DescriptionFa = table.Column<string>(maxLength: 500, nullable: false),
                    LogoFilenameFa = table.Column<string>(nullable: true),
                    BulletinFa = table.Column<string>(nullable: true),
                    TitleEn = table.Column<string>(maxLength: 200, nullable: false),
                    DescriptionEn = table.Column<string>(maxLength: 500, nullable: false),
                    LogoFilenameEn = table.Column<string>(nullable: true),
                    BulletinEn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    Username = table.Column<string>(maxLength: 450, nullable: false),
                    Password = table.Column<string>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    LastLogIn = table.Column<DateTimeOffset>(nullable: true),
                    LastIp = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Content",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortalId = table.Column<int>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    ContentType = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    ImageName = table.Column<string>(nullable: true),
                    PublishDate = table.Column<DateTimeOffset>(nullable: false),
                    ViewCount = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsFavorite = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Content", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Content_Portal_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Link",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortalId = table.Column<int>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    LinkType = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    Icon = table.Column<string>(maxLength: 50, nullable: true),
                    IconColor = table.Column<string>(maxLength: 10, nullable: true),
                    Url = table.Column<string>(nullable: false),
                    Priority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Link", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Link_Portal_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Navbar",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<long>(nullable: true),
                    PortalId = table.Column<int>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: true),
                    Icon = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Navbar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Navbar_Navbar_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Navbar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Navbar_Portal_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortalId = table.Column<int>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Text = table.Column<string>(nullable: false),
                    ImageName = table.Column<string>(nullable: true),
                    PublishDate = table.Column<DateTimeOffset>(nullable: false),
                    ViewCount = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Page_Portal_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Slide",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortalId = table.Column<int>(nullable: false),
                    Language = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    SubTitle = table.Column<string>(nullable: true),
                    PublishDate = table.Column<DateTimeOffset>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Filename = table.Column<string>(nullable: false),
                    Priority = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slide", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slide_Portal_PortalId",
                        column: x => x.PortalId,
                        principalTable: "Portal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDataProtectionKeys_FriendlyName",
                table: "AppDataProtectionKeys",
                column: "FriendlyName",
                unique: true,
                filter: "[FriendlyName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Content_PortalId",
                table: "Content",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_Link_PortalId",
                table: "Link",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_Navbar_ParentId",
                table: "Navbar",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Navbar_PortalId",
                table: "Navbar",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_Page_PortalId",
                table: "Page",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_Portal_PortalKey",
                table: "Portal",
                column: "PortalKey",
                unique: true,
                filter: "[PortalKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slide_PortalId",
                table: "Slide",
                column: "PortalId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                table: "UserRole",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDataProtectionKeys");

            migrationBuilder.DropTable(
                name: "Content");

            migrationBuilder.DropTable(
                name: "Link");

            migrationBuilder.DropTable(
                name: "Navbar");

            migrationBuilder.DropTable(
                name: "Page");

            migrationBuilder.DropTable(
                name: "Slide");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "Portal");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
