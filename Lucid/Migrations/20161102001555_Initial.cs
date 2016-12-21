using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lucid.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    created_at = table.Column<DateTime>(nullable: false),
                    description = table.Column<string>(maxLength: 1024, nullable: true),
                    name = table.Column<string>(maxLength: 64, nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_areas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    area_id = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    description = table.Column<string>(maxLength: 2048, nullable: true),
                    down_room_id = table.Column<int>(nullable: true),
                    east_room_id = table.Column<int>(nullable: true),
                    name = table.Column<string>(maxLength: 256, nullable: false),
                    north_room_id = table.Column<int>(nullable: true),
                    south_room_id = table.Column<int>(nullable: true),
                    up_room_id = table.Column<int>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: false),
                    west_room_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.id);
                    table.ForeignKey(
                        name: "FK_rooms_areas_area_id",
                        column: x => x.area_id,
                        principalTable: "areas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_rooms_rooms_down_room_id",
                        column: x => x.down_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_rooms_rooms_east_room_id",
                        column: x => x.east_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_rooms_rooms_north_room_id",
                        column: x => x.north_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_rooms_rooms_south_room_id",
                        column: x => x.south_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_rooms_rooms_up_room_id",
                        column: x => x.up_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_rooms_rooms_west_room_id",
                        column: x => x.west_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGeneratedOnAdd", true),
                    created_at = table.Column<DateTime>(nullable: false),
                    current_room_id = table.Column<int>(nullable: true),
                    hashed_password = table.Column<string>(nullable: false),
                    name = table.Column<string>(maxLength: 64, nullable: false),
                    updated_at = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_rooms_current_room_id",
                        column: x => x.current_room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rooms_area_id",
                table: "rooms",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_down_room_id",
                table: "rooms",
                column: "down_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_east_room_id",
                table: "rooms",
                column: "east_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_north_room_id",
                table: "rooms",
                column: "north_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_south_room_id",
                table: "rooms",
                column: "south_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_up_room_id",
                table: "rooms",
                column: "up_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_west_room_id",
                table: "rooms",
                column: "west_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_current_room_id",
                table: "users",
                column: "current_room_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "areas");
        }
    }
}
