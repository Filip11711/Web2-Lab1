using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SampleMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class BugFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrojOdigranihKola",
                table: "Natjecatelj",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrojOdigranihKola",
                table: "Natjecatelj");
        }
    }
}
