using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DietPlaner.Migrations
{
    /// <inheritdoc />
    public partial class AddWardAndAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoginName",
                table: "User",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WardId",
                table: "Patient",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Ward",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Floor = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ward", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_WardId",
                table: "Patient",
                column: "WardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Ward_WardId",
                table: "Patient",
                column: "WardId",
                principalTable: "Ward",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Ward_WardId",
                table: "Patient");

            migrationBuilder.DropTable(
                name: "Ward");

            migrationBuilder.DropIndex(
                name: "IX_Patient_WardId",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "LoginName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Patient");
        }
    }
}
