﻿// <auto-generated />
using System;
using EfCore9AdvancedPoCWithPostgres.Data;
using EfCore9AdvancedPoCWithPostgres.Models.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EfCore9AdvancedPoCWithPostgres.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250422202713_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.AuditLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Operation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("AuditLogs");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ManagerId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Salary")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Inheritance.BaseEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("BaseEntities", (string)null);

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<OrderDetails>("Details")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime>("OrderedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastViewedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValue(new DateTime(2025, 4, 22, 20, 27, 12, 829, DateTimeKind.Utc).AddTicks(9832));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ValidFrom")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTime>("ValidTo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("'infinity'::timestamp");

                    b.HasKey("Id");

                    b.ToTable("Products", (string)null);

                    b
                        .HasAnnotation("SqlServer:IsTemporal", true)
                        .HasAnnotation("SqlServer:TemporalHistoryTableName", "ProductsHistory")
                        .HasAnnotation("SqlServer:TemporalHistoryTableSchema", null)
                        .HasAnnotation("SqlServer:TemporalPeriodEndPropertyName", "ValidTo")
                        .HasAnnotation("SqlServer:TemporalPeriodStartPropertyName", "ValidFrom");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.ProductDetail", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Specifications")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ProductId");

                    b.ToTable("ProductDetail");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Relationships.ProductTag", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.Property<string>("AssignedBy")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("AssignedOn")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ProductId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("ProductTags");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Relationships.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EfCore9AdvancedPoC.Models.Inheritance.EmployeeEntity", b =>
                {
                    b.HasBaseType("EfCore9AdvancedPoCWithPostgres.Models.Inheritance.BaseEntity");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Salary")
                        .HasColumnType("numeric");

                    b.ToTable("EmployeeEntities", (string)null);
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Inheritance.CustomerEntity", b =>
                {
                    b.HasBaseType("EfCore9AdvancedPoCWithPostgres.Models.Inheritance.BaseEntity");

                    b.Property<byte[]>("Email")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.ToTable("CustomerEntities", (string)null);
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Employee", b =>
                {
                    b.HasOne("EfCore9AdvancedPoCWithPostgres.Models.Employee", "Manager")
                        .WithMany("DirectReports")
                        .HasForeignKey("ManagerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Manager");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Order", b =>
                {
                    b.HasOne("EfCore9AdvancedPoCWithPostgres.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EfCore9AdvancedPoCWithPostgres.Models.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId");

                    b.OwnsOne("EfCore9AdvancedPoCWithPostgres.Models.Owned.ShippingAddress", "ShippingAddress", b1 =>
                        {
                            b1.Property<int>("OrderId")
                                .HasColumnType("integer");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Line1")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("OrderId");

                            b1.ToTable("Orders");

                            b1.WithOwner()
                                .HasForeignKey("OrderId");
                        });

                    b.Navigation("Product");

                    b.Navigation("ShippingAddress")
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.ProductDetail", b =>
                {
                    b.HasOne("EfCore9AdvancedPoCWithPostgres.Models.Product", "Product")
                        .WithOne("ProductDetail")
                        .HasForeignKey("EfCore9AdvancedPoCWithPostgres.Models.ProductDetail", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Relationships.ProductTag", b =>
                {
                    b.HasOne("EfCore9AdvancedPoCWithPostgres.Models.Product", "Product")
                        .WithMany("ProductTags")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EfCore9AdvancedPoCWithPostgres.Models.Relationships.Tag", "Tag")
                        .WithMany("ProductTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.User", b =>
                {
                    b.OwnsOne("EfCore9AdvancedPoCWithPostgres.Models.Owned.UserPreferences", "Preferences", b1 =>
                        {
                            b1.Property<int>("UserId")
                                .HasColumnType("integer");

                            b1.Property<bool>("ReceiveNewsletter")
                                .HasColumnType("boolean");

                            b1.Property<string>("Theme")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.ToJson("Preferences");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });

                    b.Navigation("Preferences")
                        .IsRequired();
                });

            modelBuilder.Entity("EfCore9AdvancedPoC.Models.Inheritance.EmployeeEntity", b =>
                {
                    b.HasOne("EfCore9AdvancedPoCWithPostgres.Models.Inheritance.BaseEntity", null)
                        .WithOne()
                        .HasForeignKey("EfCore9AdvancedPoC.Models.Inheritance.EmployeeEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Inheritance.CustomerEntity", b =>
                {
                    b.HasOne("EfCore9AdvancedPoCWithPostgres.Models.Inheritance.BaseEntity", null)
                        .WithOne()
                        .HasForeignKey("EfCore9AdvancedPoCWithPostgres.Models.Inheritance.CustomerEntity", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Employee", b =>
                {
                    b.Navigation("DirectReports");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Product", b =>
                {
                    b.Navigation("ProductDetail")
                        .IsRequired();

                    b.Navigation("ProductTags");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.Relationships.Tag", b =>
                {
                    b.Navigation("ProductTags");
                });

            modelBuilder.Entity("EfCore9AdvancedPoCWithPostgres.Models.User", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
