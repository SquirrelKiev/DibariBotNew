﻿// <auto-generated />
using System;
using DibariBot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DibariBot.Migrations.PostgresMigrations
{
    [DbContext(typeof(PostgresContext))]
    [Migration("20240907233433_MangaAliases")]
    partial class MangaAliases
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DibariBot.Database.GuildConfig", b =>
                {
                    b.Property<decimal>("GuildId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric(20,0)");

                    b.Property<int?>("EmbedColor")
                        .HasColumnType("integer");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .HasMaxLength(8)
                        .HasColumnType("character varying(8)");

                    b.HasKey("GuildId");

                    b.ToTable("GuildConfigs");
                });

            modelBuilder.Entity("DibariBot.Database.Models.DefaultManga", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Manga")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("GuildId", "ChannelId")
                        .IsUnique();

                    b.ToTable("DefaultMangas");
                });

            modelBuilder.Entity("DibariBot.Database.Models.MangaCommandAlias", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Manga")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("SlashCommandName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("MangaCommandAliases");
                });

            modelBuilder.Entity("DibariBot.Database.Models.RegexChannelEntry", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<long>("RegexFilterId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RegexFilterId");

                    b.HasIndex("ChannelId", "RegexFilterId");

                    b.ToTable("RegexChannelEntries");
                });

            modelBuilder.Entity("DibariBot.Database.Models.RegexFilter", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("ChannelFilterScope")
                        .HasColumnType("integer");

                    b.Property<string>("Filter")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("FilterType")
                        .HasColumnType("integer");

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Template")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GuildId");

                    b.ToTable("RegexFilters");
                });

            modelBuilder.Entity("DibariBot.Database.Models.RegexChannelEntry", b =>
                {
                    b.HasOne("DibariBot.Database.Models.RegexFilter", "RegexFilter")
                        .WithMany("RegexChannelEntries")
                        .HasForeignKey("RegexFilterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RegexFilter");
                });

            modelBuilder.Entity("DibariBot.Database.Models.RegexFilter", b =>
                {
                    b.Navigation("RegexChannelEntries");
                });
#pragma warning restore 612, 618
        }
    }
}
