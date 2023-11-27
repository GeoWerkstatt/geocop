﻿// <auto-generated />
using System;
using GeoCop.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GeoCop.Api.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20231127135108_AddJobIdToDelivery")]
    partial class AddJobIdToDelivery
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DeliveryMandateOrganisation", b =>
                {
                    b.Property<int>("MandatesId")
                        .HasColumnType("integer");

                    b.Property<int>("OrganisationsId")
                        .HasColumnType("integer");

                    b.HasKey("MandatesId", "OrganisationsId");

                    b.HasIndex("OrganisationsId");

                    b.ToTable("DeliveryMandateOrganisation");
                });

            modelBuilder.Entity("GeoCop.Api.Models.Asset", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AssetType")
                        .IsRequired()
                        .HasColumnType("varchar(24)");

                    b.Property<int>("DeliveryId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("FileHash")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("OriginalFilename")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SanitizedFilename")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryId");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("GeoCop.Api.Models.Delivery", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DeclaringUserId")
                        .HasColumnType("integer");

                    b.Property<int>("DeliveryMandateId")
                        .HasColumnType("integer");

                    b.Property<Guid>("JobId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("DeclaringUserId");

                    b.HasIndex("DeliveryMandateId");

                    b.ToTable("Deliveries");
                });

            modelBuilder.Entity("GeoCop.Api.Models.DeliveryMandate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string[]>("FileTypes")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Geometry>("SpatialExtent")
                        .IsRequired()
                        .HasColumnType("geometry");

                    b.HasKey("Id");

                    b.ToTable("DeliveryMandates");
                });

            modelBuilder.Entity("GeoCop.Api.Models.Organisation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Organisations");
                });

            modelBuilder.Entity("GeoCop.Api.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AuthIdentifier")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OrganisationUser", b =>
                {
                    b.Property<int>("OrganisationsId")
                        .HasColumnType("integer");

                    b.Property<int>("UsersId")
                        .HasColumnType("integer");

                    b.HasKey("OrganisationsId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("OrganisationUser");
                });

            modelBuilder.Entity("DeliveryMandateOrganisation", b =>
                {
                    b.HasOne("GeoCop.Api.Models.DeliveryMandate", null)
                        .WithMany()
                        .HasForeignKey("MandatesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GeoCop.Api.Models.Organisation", null)
                        .WithMany()
                        .HasForeignKey("OrganisationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GeoCop.Api.Models.Asset", b =>
                {
                    b.HasOne("GeoCop.Api.Models.Delivery", "Delivery")
                        .WithMany("Assets")
                        .HasForeignKey("DeliveryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Delivery");
                });

            modelBuilder.Entity("GeoCop.Api.Models.Delivery", b =>
                {
                    b.HasOne("GeoCop.Api.Models.User", "DeclaringUser")
                        .WithMany("Deliveries")
                        .HasForeignKey("DeclaringUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GeoCop.Api.Models.DeliveryMandate", "DeliveryMandate")
                        .WithMany("Deliveries")
                        .HasForeignKey("DeliveryMandateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeclaringUser");

                    b.Navigation("DeliveryMandate");
                });

            modelBuilder.Entity("OrganisationUser", b =>
                {
                    b.HasOne("GeoCop.Api.Models.Organisation", null)
                        .WithMany()
                        .HasForeignKey("OrganisationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GeoCop.Api.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GeoCop.Api.Models.Delivery", b =>
                {
                    b.Navigation("Assets");
                });

            modelBuilder.Entity("GeoCop.Api.Models.DeliveryMandate", b =>
                {
                    b.Navigation("Deliveries");
                });

            modelBuilder.Entity("GeoCop.Api.Models.User", b =>
                {
                    b.Navigation("Deliveries");
                });
#pragma warning restore 612, 618
        }
    }
}
