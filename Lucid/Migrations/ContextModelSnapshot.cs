﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Lucid.Database;

namespace Lucid.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("Lucid.Models.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasAnnotation("MaxLength", 1024);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("areas");
                });

            modelBuilder.Entity("Lucid.Models.EquipmentSlot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<int>("EquipmentType")
                        .HasColumnName("equipment_type");

                    b.Property<int>("ItemDefinitionId")
                        .HasColumnName("item_definition_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("ItemDefinitionId");

                    b.HasIndex("ItemDefinitionId", "EquipmentType")
                        .IsUnique();

                    b.ToTable("equipment_slots");
                });

            modelBuilder.Entity("Lucid.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<int>("ItemDefinitionId")
                        .HasColumnName("item_definition_id");

                    b.Property<int>("ParentObjectId")
                        .HasColumnName("parent_object_id");

                    b.Property<int>("ParentObjectType")
                        .HasColumnName("parent_object_type");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("ItemDefinitionId");

                    b.ToTable("items");
                });

            modelBuilder.Entity("Lucid.Models.ItemDefinition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("item_definitions");
                });

            modelBuilder.Entity("Lucid.Models.Room", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int>("AreaId")
                        .HasColumnName("area_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasAnnotation("MaxLength", 2048);

                    b.Property<int?>("DownRoomId")
                        .HasColumnName("down_room_id");

                    b.Property<int?>("EastRoomId")
                        .HasColumnName("east_room_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<int?>("NorthRoomId")
                        .HasColumnName("north_room_id");

                    b.Property<int?>("SouthRoomId")
                        .HasColumnName("south_room_id");

                    b.Property<int?>("UpRoomId")
                        .HasColumnName("up_room_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.Property<int?>("WestRoomId")
                        .HasColumnName("west_room_id");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.HasIndex("DownRoomId");

                    b.HasIndex("EastRoomId");

                    b.HasIndex("NorthRoomId");

                    b.HasIndex("SouthRoomId");

                    b.HasIndex("UpRoomId");

                    b.HasIndex("WestRoomId");

                    b.ToTable("rooms");
                });

            modelBuilder.Entity("Lucid.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id");

                    b.Property<int?>("BodyItemId")
                        .HasColumnName("body_item_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnName("created_at");

                    b.Property<int?>("CurrentRoomId")
                        .HasColumnName("current_room_id");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnName("hashed_password");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("name")
                        .HasAnnotation("MaxLength", 64);

                    b.Property<int?>("PrimaryWeaponId")
                        .HasColumnName("primary_weapon_id");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.HasIndex("BodyItemId");

                    b.HasIndex("CurrentRoomId");

                    b.HasIndex("PrimaryWeaponId");

                    b.ToTable("users");
                });

            modelBuilder.Entity("Lucid.Models.EquipmentSlot", b =>
                {
                    b.HasOne("Lucid.Models.ItemDefinition", "ItemDefinition")
                        .WithMany("EquipmentSlots")
                        .HasForeignKey("ItemDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Lucid.Models.Item", b =>
                {
                    b.HasOne("Lucid.Models.ItemDefinition", "ItemDefinition")
                        .WithMany()
                        .HasForeignKey("ItemDefinitionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Lucid.Models.Room", b =>
                {
                    b.HasOne("Lucid.Models.Area", "Area")
                        .WithMany("Rooms")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Lucid.Models.Room", "DownRoom")
                        .WithMany()
                        .HasForeignKey("DownRoomId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Lucid.Models.Room", "EastRoom")
                        .WithMany()
                        .HasForeignKey("EastRoomId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Lucid.Models.Room", "NorthRoom")
                        .WithMany()
                        .HasForeignKey("NorthRoomId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Lucid.Models.Room", "SouthRoom")
                        .WithMany()
                        .HasForeignKey("SouthRoomId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Lucid.Models.Room", "UpRoom")
                        .WithMany()
                        .HasForeignKey("UpRoomId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("Lucid.Models.Room", "WestRoom")
                        .WithMany()
                        .HasForeignKey("WestRoomId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("Lucid.Models.User", b =>
                {
                    b.HasOne("Lucid.Models.Item", "BodyItem")
                        .WithMany()
                        .HasForeignKey("BodyItemId");

                    b.HasOne("Lucid.Models.Room", "CurrentRoom")
                        .WithMany()
                        .HasForeignKey("CurrentRoomId");

                    b.HasOne("Lucid.Models.Item", "PrimaryWeapon")
                        .WithMany()
                        .HasForeignKey("PrimaryWeaponId");
                });
        }
    }
}
