using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CMS.Migrations.CMS
{
    public partial class Migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    PrimaryImageId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    MetaTitle = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    MetaKeyword = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    MetaDescription = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    MetaTitle = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    MetaKeyword = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    MetaDescription = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Alt = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(25)", unicode: false, maxLength: 25, nullable: false),
                    Item = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ValidatedBy = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    MetaTitle = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    MetaKeyword = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    MetaDescription = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    AddedOn = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Status = table.Column<bool>(type: "bit", nullable: false)
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
