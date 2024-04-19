using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB_AngoraLib.Migrations
{
    /// <inheritdoc />
    public partial class DB_AngoraMig_01 : Migration
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
                    IsAdmin = table.Column<bool>(type: "bit", nullable: true)
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
                    IsPublic = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rabbits", x => new { x.RightEarId, x.LeftEarId });
                    table.ForeignKey(
                        name: "FK_Rabbits_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "BreederRegNo");
                });

            migrationBuilder.CreateTable(
                name: "RabbitParents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MotherRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MotherLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FatherRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FatherLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChildRightEarId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChildLeftEarId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RabbitParents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RabbitParents_Rabbits_ChildRightEarId_ChildLeftEarId",
                        columns: x => new { x.ChildRightEarId, x.ChildLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RabbitParents_Rabbits_FatherRightEarId_FatherLeftEarId",
                        columns: x => new { x.FatherRightEarId, x.FatherLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                    table.ForeignKey(
                        name: "FK_RabbitParents_Rabbits_MotherRightEarId_MotherLeftEarId",
                        columns: x => new { x.MotherRightEarId, x.MotherLeftEarId },
                        principalTable: "Rabbits",
                        principalColumns: new[] { "RightEarId", "LeftEarId" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_RabbitParents_ChildRightEarId_ChildLeftEarId",
                table: "RabbitParents",
                columns: new[] { "ChildRightEarId", "ChildLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_RabbitParents_FatherRightEarId_FatherLeftEarId",
                table: "RabbitParents",
                columns: new[] { "FatherRightEarId", "FatherLeftEarId" });

            migrationBuilder.CreateIndex(
                name: "IX_RabbitParents_MotherRightEarId_MotherLeftEarId",
                table: "RabbitParents",
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
                name: "RabbitParents");

            migrationBuilder.DropTable(
                name: "Rabbits");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
