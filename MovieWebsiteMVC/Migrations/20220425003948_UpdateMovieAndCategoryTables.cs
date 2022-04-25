using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieWebsiteMVC.Migrations
{
    public partial class UpdateMovieAndCategoryTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pster",
                table: "Movies",
                newName: "Poster");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Poster",
                table: "Movies",
                newName: "Pster");
        }
    }
}
