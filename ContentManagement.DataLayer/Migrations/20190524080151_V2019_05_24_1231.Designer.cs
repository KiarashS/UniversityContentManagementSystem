﻿// <auto-generated />
using System;
using ContentManagement.DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ContentManagement.DataLayer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190524080151_V2019_05_24_1231")]
    partial class V2019_05_24_1231
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContentManagement.Entities.ActivityLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionBy");

                    b.Property<DateTimeOffset>("ActionDate");

                    b.Property<byte>("ActionLevel");

                    b.Property<string>("ActionType");

                    b.Property<string>("Language");

                    b.Property<string>("Message");

                    b.Property<string>("Portal");

                    b.Property<string>("SourceAddress");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("ActivityLog");
                });

            modelBuilder.Entity("ContentManagement.Entities.AppDataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FriendlyName");

                    b.Property<string>("XmlData");

                    b.HasKey("Id");

                    b.HasIndex("FriendlyName")
                        .IsUnique()
                        .HasFilter("[FriendlyName] IS NOT NULL");

                    b.ToTable("AppDataProtectionKeys");
                });

            modelBuilder.Entity("ContentManagement.Entities.Content", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset?>("ArchiveDate");

                    b.Property<int>("AudioPosition");

                    b.Property<int>("ContentType");

                    b.Property<int>("GalleryPosition");

                    b.Property<string>("Imagename");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsArchive");

                    b.Property<bool>("IsFavorite");

                    b.Property<string>("Keywords")
                        .IsRequired();

                    b.Property<int>("Language");

                    b.Property<int>("PortalId");

                    b.Property<int?>("Priority");

                    b.Property<DateTimeOffset>("PublishDate");

                    b.Property<string>("RawText");

                    b.Property<string>("Summary");

                    b.Property<string>("Text");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("VideoPosition");

                    b.Property<int>("ViewCount");

                    b.HasKey("Id");

                    b.HasIndex("PortalId");

                    b.ToTable("Content");
                });

            modelBuilder.Entity("ContentManagement.Entities.ContentAudio", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Audioname")
                        .IsRequired();

                    b.Property<string>("Caption");

                    b.Property<long>("ContentId");

                    b.Property<bool>("EnableAutoplay");

                    b.Property<bool>("EnableControls");

                    b.Property<int?>("Priority");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.ToTable("ContentAudio");
                });

            modelBuilder.Entity("ContentManagement.Entities.ContentGallery", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Caption");

                    b.Property<long>("ContentId");

                    b.Property<string>("Imagename")
                        .IsRequired();

                    b.Property<int?>("Priority");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.ToTable("ContentGallery");
                });

            modelBuilder.Entity("ContentManagement.Entities.ContentVideo", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Caption");

                    b.Property<long>("ContentId");

                    b.Property<bool>("EnableAutoplay");

                    b.Property<bool>("EnableControls");

                    b.Property<int?>("Height");

                    b.Property<int?>("Priority");

                    b.Property<string>("Videoname")
                        .IsRequired();

                    b.Property<int?>("Width");

                    b.HasKey("Id");

                    b.HasIndex("ContentId");

                    b.ToTable("ContentVideo");
                });

            modelBuilder.Entity("ContentManagement.Entities.FooterLink", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("FooterSectionId");

                    b.Property<bool>("IsBlankUrlTarget");

                    b.Property<int?>("Priority");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("FooterSectionId");

                    b.ToTable("FooterLink");
                });

            modelBuilder.Entity("ContentManagement.Entities.FooterSection", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsEnable");

                    b.Property<int>("Language");

                    b.Property<int>("PortalId");

                    b.Property<int?>("Priority");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("PortalId");

                    b.ToTable("FooterSection");
                });

            modelBuilder.Entity("ContentManagement.Entities.ImageLink", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Imagename")
                        .IsRequired();

                    b.Property<bool>("IsBlankUrlTarget");

                    b.Property<int>("Language");

                    b.Property<int>("PortalId");

                    b.Property<int?>("Priority");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PortalId");

                    b.ToTable("ImageLink");
                });

            modelBuilder.Entity("ContentManagement.Entities.Link", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Icon")
                        .HasMaxLength(50);

                    b.Property<string>("IconColor")
                        .HasMaxLength(10);

                    b.Property<bool>("IsBlankUrlTarget");

                    b.Property<int>("Language");

                    b.Property<int>("LinkType");

                    b.Property<int>("PortalId");

                    b.Property<int?>("Priority");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<string>("Url")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PortalId");

                    b.ToTable("Link");
                });

            modelBuilder.Entity("ContentManagement.Entities.Navbar", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Icon")
                        .HasMaxLength(50);

                    b.Property<bool>("IsBlankUrlTarget");

                    b.Property<int>("Language");

                    b.Property<long?>("ParentId");

                    b.Property<int>("PortalId");

                    b.Property<int?>("Priority");

                    b.Property<string>("Text");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("PortalId");

                    b.ToTable("Navbar");
                });

            modelBuilder.Entity("ContentManagement.Entities.Page", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Imagename");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Keywords")
                        .IsRequired();

                    b.Property<int>("Language");

                    b.Property<int>("PortalId");

                    b.Property<DateTimeOffset>("PublishDate");

                    b.Property<string>("RawText")
                        .IsRequired();

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("ViewCount");

                    b.HasKey("Id");

                    b.HasIndex("PortalId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Page");
                });

            modelBuilder.Entity("ContentManagement.Entities.Portal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AdminEmail");

                    b.Property<string>("BulletinEn");

                    b.Property<string>("BulletinFa");

                    b.Property<string>("DescriptionEn")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("DescriptionFa")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("HtmlTitleEn")
                        .IsRequired();

                    b.Property<string>("HtmlTitleFa")
                        .IsRequired();

                    b.Property<string>("LogoFilenameEn");

                    b.Property<string>("LogoFilenameFa");

                    b.Property<string>("PortalKey")
                        .HasMaxLength(400);

                    b.Property<bool>("ShowInMainPortal");

                    b.Property<string>("TitleEn")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("TitleFa")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.HasIndex("PortalKey")
                        .IsUnique()
                        .HasFilter("[PortalKey] IS NOT NULL");

                    b.ToTable("Portal");
                });

            modelBuilder.Entity("ContentManagement.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Role");
                });

            modelBuilder.Entity("ContentManagement.Entities.Slide", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset?>("ExpireDate");

                    b.Property<string>("Filename")
                        .IsRequired();

                    b.Property<bool>("IsBlankUrlTarget");

                    b.Property<int>("Language");

                    b.Property<int>("PortalId");

                    b.Property<int?>("Priority");

                    b.Property<DateTimeOffset>("PublishDate");

                    b.Property<string>("SubTitle");

                    b.Property<string>("SupTitle");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.HasIndex("PortalId");

                    b.ToTable("Slide");
                });

            modelBuilder.Entity("ContentManagement.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<bool>("IsActive");

                    b.Property<string>("LastIp");

                    b.Property<DateTimeOffset?>("LastLogIn");

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<int?>("PortalId");

                    b.Property<string>("SerialNumber")
                        .HasMaxLength(450);

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(450);

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PortalId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("ContentManagement.Entities.UserRole", b =>
                {
                    b.Property<long>("UserId");

                    b.Property<int>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("ContentManagement.Entities.Vote", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset?>("ExpireDate");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsMultiChoice");

                    b.Property<bool>("IsVisibleResults");

                    b.Property<int>("Language");

                    b.Property<int>("PortalId");

                    b.Property<DateTimeOffset>("PublishDate");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("PortalId");

                    b.ToTable("Vote");
                });

            modelBuilder.Entity("ContentManagement.Entities.VoteItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ItemTitle")
                        .IsRequired();

                    b.Property<int?>("Priority");

                    b.Property<long>("VoteId");

                    b.HasKey("Id");

                    b.HasIndex("VoteId");

                    b.ToTable("VoteItem");
                });

            modelBuilder.Entity("ContentManagement.Entities.VoteResult", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("VoteId");

                    b.Property<long>("VoteItemId");

                    b.HasKey("Id");

                    b.HasIndex("VoteId");

                    b.HasIndex("VoteItemId");

                    b.ToTable("VoteResult");
                });

            modelBuilder.Entity("ContentManagement.Entities.Content", b =>
                {
                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("Contents")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.ContentAudio", b =>
                {
                    b.HasOne("ContentManagement.Entities.Content", "Content")
                        .WithMany("ContentAudios")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.ContentGallery", b =>
                {
                    b.HasOne("ContentManagement.Entities.Content", "Content")
                        .WithMany("ContentGalleries")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.ContentVideo", b =>
                {
                    b.HasOne("ContentManagement.Entities.Content", "Content")
                        .WithMany("ContentVideos")
                        .HasForeignKey("ContentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.FooterLink", b =>
                {
                    b.HasOne("ContentManagement.Entities.FooterSection", "FooterSection")
                        .WithMany("Links")
                        .HasForeignKey("FooterSectionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.FooterSection", b =>
                {
                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("FooterSections")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.ImageLink", b =>
                {
                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("ImageLinks")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.Link", b =>
                {
                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("Links")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.Navbar", b =>
                {
                    b.HasOne("ContentManagement.Entities.Navbar", "Parent")
                        .WithMany("Childrens")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("Navbars")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.Page", b =>
                {
                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("Pages")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.Slide", b =>
                {
                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("Slides")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.User", b =>
                {
                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("Users")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.UserRole", b =>
                {
                    b.HasOne("ContentManagement.Entities.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContentManagement.Entities.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.Vote", b =>
                {
                    b.HasOne("ContentManagement.Entities.Portal", "Portal")
                        .WithMany("Votes")
                        .HasForeignKey("PortalId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContentManagement.Entities.VoteItem", b =>
                {
                    b.HasOne("ContentManagement.Entities.Vote", "Vote")
                        .WithMany("Items")
                        .HasForeignKey("VoteId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("ContentManagement.Entities.VoteResult", b =>
                {
                    b.HasOne("ContentManagement.Entities.Vote", "Vote")
                        .WithMany("VoteResults")
                        .HasForeignKey("VoteId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContentManagement.Entities.VoteItem", "VoteItem")
                        .WithMany("VoteItemResults")
                        .HasForeignKey("VoteItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
