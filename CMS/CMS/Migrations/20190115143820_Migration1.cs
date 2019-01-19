using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace CMS.Migrations
{
    public partial class Migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CategoryId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(unicode: false, nullable: false),
                    MetaDescription = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    MetaKeyword = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    MetaTitle = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    Name = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    PrimaryImageId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Url = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Description = table.Column<string>(unicode: false, nullable: false),
                    MetaDescription = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    MetaKeyword = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    MetaTitle = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    Name = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Status = table.Column<bool>(nullable: false),
                    Url = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Alt = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Name = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Url = table.Column<string>(unicode: false, maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Item = table.Column<string>(unicode: false, maxLength: 1000, nullable: false),
                    Name = table.Column<string>(unicode: false, maxLength: 25, nullable: false),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    CourseId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    ValidatedBy = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Description = table.Column<string>(unicode: false, nullable: false),
                    MetaDescription = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    MetaKeyword = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    MetaTitle = table.Column<string>(unicode: false, maxLength: 250, nullable: true),
                    Name = table.Column<string>(unicode: false, maxLength: 100, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Url = table.Column<string>(unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "BlogCategory");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Page");
        }
    }
}
