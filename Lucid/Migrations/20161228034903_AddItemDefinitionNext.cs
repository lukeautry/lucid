using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lucid.Migrations
{
    public partial class AddItemDefinitionNext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "item_definitions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    created_at = table.Column<DateTime>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_definitions", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "item_definitions");
        }
    }
}
