﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PollApp.Api.Data;

#nullable disable

namespace PollApp.Api.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PollApp.Api.Models.Poll", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("PollApp.Api.Models.PollOption", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<int?>("PollID")
                        .HasColumnType("integer");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.HasIndex("PollID");

                    b.ToTable("PollOption");
                });

            modelBuilder.Entity("PollApp.Api.Models.User", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PollOptionUser", b =>
                {
                    b.Property<int>("VotedForID")
                        .HasColumnType("integer");

                    b.Property<string>("VotedUsersUsername")
                        .HasColumnType("text");

                    b.HasKey("VotedForID", "VotedUsersUsername");

                    b.HasIndex("VotedUsersUsername");

                    b.ToTable("PollOptionUser");
                });

            modelBuilder.Entity("PollApp.Api.Models.PollOption", b =>
                {
                    b.HasOne("PollApp.Api.Models.Poll", null)
                        .WithMany("Options")
                        .HasForeignKey("PollID");
                });

            modelBuilder.Entity("PollOptionUser", b =>
                {
                    b.HasOne("PollApp.Api.Models.PollOption", null)
                        .WithMany()
                        .HasForeignKey("VotedForID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PollApp.Api.Models.User", null)
                        .WithMany()
                        .HasForeignKey("VotedUsersUsername")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PollApp.Api.Models.Poll", b =>
                {
                    b.Navigation("Options");
                });
#pragma warning restore 612, 618
        }
    }
}
