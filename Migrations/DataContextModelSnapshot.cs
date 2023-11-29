﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiTutorial.Data;

#nullable disable

namespace WebApiTutorial.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PollOptionUser", b =>
                {
                    b.Property<int>("VotedForID")
                        .HasColumnType("int");

                    b.Property<string>("VotedUsersUsername")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("VotedForID", "VotedUsersUsername");

                    b.HasIndex("VotedUsersUsername");

                    b.ToTable("PollOptionUser");
                });

            modelBuilder.Entity("WebApiTutorial.Models.Poll", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("WebApiTutorial.Models.PollOption", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int?>("PollID")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("PollID");

                    b.ToTable("PollOption");
                });

            modelBuilder.Entity("WebApiTutorial.Models.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PollOptionUser", b =>
                {
                    b.HasOne("WebApiTutorial.Models.PollOption", null)
                        .WithMany()
                        .HasForeignKey("VotedForID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApiTutorial.Models.User", null)
                        .WithMany()
                        .HasForeignKey("VotedUsersUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WebApiTutorial.Models.PollOption", b =>
                {
                    b.HasOne("WebApiTutorial.Models.Poll", null)
                        .WithMany("Options")
                        .HasForeignKey("PollID");
                });

            modelBuilder.Entity("WebApiTutorial.Models.Poll", b =>
                {
                    b.Navigation("Options");
                });
#pragma warning restore 612, 618
        }
    }
}
