using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lucid.Migrations
{
    public partial class EquipmentSlots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "equipment_slots",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    created_at = table.Column<DateTime>(nullable: false),
                    equipment_type = table.Column<int>(nullable: false),
                    item_definition_id = table.Column<int>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipment_slots", x => x.id);
                    table.ForeignKey(
                        name: "FK_equipment_slots_item_definitions_item_definition_id",
                        column: x => x.item_definition_id,
                        principalTable: "item_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<int>(
                name: "body_item_id",
                table: "users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "primary_weapon_id",
                table: "users",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_body_item_id",
                table: "users",
                column: "body_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_primary_weapon_id",
                table: "users",
                column: "primary_weapon_id");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_slots_item_definition_id",
                table: "equipment_slots",
                column: "item_definition_id");

            migrationBuilder.CreateIndex(
                name: "IX_equipment_slots_item_definition_id_equipment_type",
                table: "equipment_slots",
                columns: new[] { "item_definition_id", "equipment_type" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_items_body_item_id",
                table: "users",
                column: "body_item_id",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_items_primary_weapon_id",
                table: "users",
                column: "primary_weapon_id",
                principalTable: "items",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_items_body_item_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_items_primary_weapon_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_body_item_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_primary_weapon_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "body_item_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "primary_weapon_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "equipment_slots");
        }
    }
}
