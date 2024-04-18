using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB_AngoraLib.Migrations
{
    /// <inheritdoc />
    public partial class DB_Angora_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    BreederRegNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoadNameAndNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.BreederRegNo);
                });

            migrationBuilder.CreateTable(
                name: "Rabbits",
                columns: table => new
                {
                    RightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Race = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false),
                    ApprovedRaceColorCombination = table.Column<bool>(type: "bit", nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DateOfDeath = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    MotherRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MotherLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FatherRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FatherLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsPublic = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rabbits", x => new { x.RightEarId, x.LeftEarId });
                    table.ForeignKey(
                        name: "FK_Rabbits_Rabbits_FatherRightEarId_FatherLeftEarId",
                        columns: x => new { x.FatherRightEarId, x.FatherLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                    table.ForeignKey(
                        name: "FK_Rabbits_Rabbits_MotherRightEarId_MotherLeftEarId",
                        columns: x => new { x.MotherRightEarId, x.MotherLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                    table.ForeignKey(
                        name: "FK_Rabbits_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "BreederRegNo");
                });

            migrationBuilder.CreateTable(
                name: "Litters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MotherRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MotherLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FatherRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FatherLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Litters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Litters_Rabbits_FatherRightEarId_FatherLeftEarId",
                        columns: x => new { x.FatherRightEarId, x.FatherLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                    table.ForeignKey(
                        name: "FK_Litters_Rabbits_MotherRightEarId_MotherLeftEarId",
                        columns: x => new { x.MotherRightEarId, x.MotherLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                });

            migrationBuilder.CreateTable(
                name: "LitterRabbit",
                columns: table => new
                {
                    LittersId = table.Column<int>(type: "int", nullable: false),
                    RabbitsRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RabbitsLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitterRabbit", x => new { x.LittersId, x.RabbitsRightEarId, x.RabbitsLeftEarId });
                    table.ForeignKey(
                        name: "FK_LitterRabbit_Litters_LittersId",
                        column: x => x.LittersId,
                        principalTable: "Litters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LitterRabbit_Rabbits_RabbitsRightEarId_RabbitsLeftEarId",
                        columns: x => new { x.RabbitsRightEarId, x.RabbitsLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LitterRabbit_RabbitsRightEarId_RabbitsLeftEarId",
                table: "LitterRabbit",
                columns: new[] { "RabbitsRightEarId", "RabbitsLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_Litters_FatherRightEarId_FatherLeftEarId",
                table: "Litters",
                columns: new[] { "FatherRightEarId", "FatherLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_Litters_MotherRightEarId_MotherLeftEarId",
                table: "Litters",
                columns: new[] { "MotherRightEarId", "MotherLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_FatherRightEarId_FatherLeftEarId",
                table: "Rabbits",
                columns: new[] { "FatherRightEarId", "FatherLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_MotherRightEarId_MotherLeftEarId",
                table: "Rabbits",
                columns: new[] { "MotherRightEarId", "MotherLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_OwnerId",
                table: "Rabbits",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LitterRabbit");

            migrationBuilder.DropTable(
                name: "Litters");

            migrationBuilder.DropTable(
                name: "Rabbits");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
