﻿// <auto-generated />
using System;
using EctBlazorApp.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EctBlazorApp.Server.Migrations
{
    [DbContext(typeof(EctDbContext))]
    [Migration("20201120154111_AddAdminEntity")]
    partial class AddAdminEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("EctBlazorApp.Shared.CalendarEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AttendeesAsString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EctUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("End")
                        .HasColumnType("datetime2");

                    b.Property<string>("Organizer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Start")
                        .HasColumnType("datetime2");

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EctUserId");

                    b.ToTable("CalendarEvents");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.EctAdmin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Administrators");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.EctTeam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("LeaderId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LeaderId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.EctUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastSignIn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MemberOfId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberOfId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.ReceivedMail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EctUserId")
                        .HasColumnType("int");

                    b.Property<string>("From")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ReceivedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EctUserId");

                    b.ToTable("ReceivedEmails");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.SentMail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("EctUserId")
                        .HasColumnType("int");

                    b.Property<string>("RecipientsAsString")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Subject")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EctUserId");

                    b.ToTable("SentEmails");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.CalendarEvent", b =>
                {
                    b.HasOne("EctBlazorApp.Shared.EctUser", null)
                        .WithMany("CalendarEvents")
                        .HasForeignKey("EctUserId");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.EctAdmin", b =>
                {
                    b.HasOne("EctBlazorApp.Shared.EctUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.EctTeam", b =>
                {
                    b.HasOne("EctBlazorApp.Shared.EctUser", "Leader")
                        .WithMany("LeaderOf")
                        .HasForeignKey("LeaderId");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.EctUser", b =>
                {
                    b.HasOne("EctBlazorApp.Shared.EctTeam", "MemberOf")
                        .WithMany("Members")
                        .HasForeignKey("MemberOfId");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.ReceivedMail", b =>
                {
                    b.HasOne("EctBlazorApp.Shared.EctUser", null)
                        .WithMany("ReceivedEmails")
                        .HasForeignKey("EctUserId");
                });

            modelBuilder.Entity("EctBlazorApp.Shared.SentMail", b =>
                {
                    b.HasOne("EctBlazorApp.Shared.EctUser", null)
                        .WithMany("SentEmails")
                        .HasForeignKey("EctUserId");
                });
#pragma warning restore 612, 618
        }
    }
}
