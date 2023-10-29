using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SampleMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class Complete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Natjecanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BodoviPobjeda = table.Column<int>(type: "integer", nullable: false),
                    Naziv = table.Column<string>(type: "text", nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    PublicUrl = table.Column<string>(type: "text", nullable: true),
                    BodoviRemi = table.Column<int>(type: "integer", nullable: false),
                    BodoviPoraz = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Natjecanje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Natjecanje_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Natjecatelj",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Naziv = table.Column<string>(type: "text", nullable: false),
                    BrojBodova = table.Column<int>(type: "integer", nullable: false),
                    Razlika = table.Column<int>(type: "integer", nullable: false),
                    NatjecanjeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Natjecatelj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Natjecatelj_Natjecanje_NatjecanjeId",
                        column: x => x.NatjecanjeId,
                        principalTable: "Natjecanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rezultat",
                columns: table => new
                {
                    IdDomacin = table.Column<int>(type: "integer", nullable: false),
                    IdGost = table.Column<int>(type: "integer", nullable: false),
                    Ishod = table.Column<int>(type: "integer", nullable: false),
                    Vrijednost = table.Column<string>(type: "text", nullable: true),
                    Kolo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezultat", x => new { x.IdDomacin, x.IdGost });
                    table.ForeignKey(
                        name: "FK_Rezultat_Natjecatelj_IdDomacin",
                        column: x => x.IdDomacin,
                        principalTable: "Natjecatelj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rezultat_Natjecatelj_IdGost",
                        column: x => x.IdGost,
                        principalTable: "Natjecatelj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_UserIdentifier",
                table: "User",
                column: "UserIdentifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Natjecanje_UserId",
                table: "Natjecanje",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Natjecatelj_NatjecanjeId",
                table: "Natjecatelj",
                column: "NatjecanjeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezultat_IdGost",
                table: "Rezultat",
                column: "IdGost");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rezultat");

            migrationBuilder.DropTable(
                name: "Natjecatelj");

            migrationBuilder.DropTable(
                name: "Natjecanje");

            migrationBuilder.DropIndex(
                name: "IX_User_UserIdentifier",
                table: "User");
        }
    }
}
