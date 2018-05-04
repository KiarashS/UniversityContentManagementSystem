using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContentManagement.DataLayer.Migrations
{
    public partial class V2018_05_03_1901 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Navbar_Navbar_ParentId",
                table: "Navbar");

            migrationBuilder.AddForeignKey(
                name: "FK_Navbar_Navbar_ParentId",
                table: "Navbar",
                column: "ParentId",
                principalTable: "Navbar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Navbar_Navbar_ParentId",
                table: "Navbar");

            migrationBuilder.AddForeignKey(
                name: "FK_Navbar_Navbar_ParentId",
                table: "Navbar",
                column: "ParentId",
                principalTable: "Navbar",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
