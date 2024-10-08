﻿// <auto-generated />
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
    [Migration("20240219062803_SwitchToBotBase")]
    partial class SwitchToBotBase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BotBase.Database.GuildPrefixPreference", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<decimal>("GuildId")
                        .HasColumnType("numeric(20,0)");

                    b.Property<string>("Prefix")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GuildId")
                        .IsUnique();

                    b.ToTable("GuildPrefixPreferences");
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
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GuildId", "ChannelId")
                        .IsUnique();

                    b.ToTable("DefaultMangas");
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
