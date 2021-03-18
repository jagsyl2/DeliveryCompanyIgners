﻿// <auto-generated />
using System;
using DeliveryCompany.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeliveryCompany.DataLayer.Migrations
{
    [DbContext(typeof(DeliveryCompanyDbContext))]
    [Migration("20210318163238_AssignNumbersToEnumStateOfPackage")]
    partial class AssignNumbersToEnumStateOfPackage
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("DeliveryCompany.DataLayer.Models.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime>("DateOfRegistration")
                        .HasColumnType("datetime2");

                    b.Property<int>("ModeWaybill")
                        .HasColumnType("int");

                    b.Property<Guid>("Number")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RecipientCity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("RecipientLat")
                        .HasColumnType("float");

                    b.Property<double>("RecipientLon")
                        .HasColumnType("float");

                    b.Property<string>("RecipientName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientPostCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientStreet")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientStreetNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientSurname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SenderId")
                        .HasColumnType("int");

                    b.Property<int>("Size")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("VehicleNumber")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SenderId");

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("DeliveryCompany.DataLayer.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<double>("lat")
                        .HasColumnType("float");

                    b.Property<double>("lon")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DeliveryCompany.DataLayer.Models.Vehicle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("AverageSpeed")
                        .HasColumnType("int");

                    b.Property<int>("DriverId")
                        .HasColumnType("int");

                    b.Property<int>("LoadCapacity")
                        .HasColumnType("int");

                    b.Property<string>("Mark")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Model")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RegistrationNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DriverId");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("DeliveryCompany.DataLayer.Models.Package", b =>
                {
                    b.HasOne("DeliveryCompany.DataLayer.Models.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("DeliveryCompany.DataLayer.Models.Vehicle", b =>
                {
                    b.HasOne("DeliveryCompany.DataLayer.Models.User", "Driver")
                        .WithMany()
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Driver");
                });
#pragma warning restore 612, 618
        }
    }
}
