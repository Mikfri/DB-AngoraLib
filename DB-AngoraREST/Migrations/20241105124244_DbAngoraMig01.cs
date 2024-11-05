using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DB_AngoraREST.Migrations
{
    /// <inheritdoc />
    public partial class DbAngoraMig01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoadNameAndNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZipCode = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    BreederRegNo = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BreederApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserApplicantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateSubmitted = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedBreederRegNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentationPath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreederApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreederApplications_AspNetUsers_UserApplicantId",
                        column: x => x.UserApplicantId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BreederBrands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BreederBrandName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BreederBrandDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BreederBrandLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFindable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BreederBrands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BreederBrands_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorite",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ItemId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorite", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorite_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rabbits",
                columns: table => new
                {
                    EarCombId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RightEarId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LeftEarId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Race = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DateOfDeath = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    ForSale = table.Column<int>(type: "int", nullable: false),
                    ForBreeding = table.Column<int>(type: "int", nullable: false),
                    FatherId_Placeholder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Father_EarCombId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MotherId_Placeholder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mother_EarCombId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rabbits", x => x.EarCombId);
                    table.ForeignKey(
                        name: "FK_Rabbits_AspNetUsers_OriginId",
                        column: x => x.OriginId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rabbits_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rabbits_Rabbits_Father_EarCombId",
                        column: x => x.Father_EarCombId,
                        principalTable: "Rabbits",
                        principalColumn: "EarCombId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rabbits_Rabbits_Mother_EarCombId",
                        column: x => x.Mother_EarCombId,
                        principalTable: "Rabbits",
                        principalColumn: "EarCombId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RabbitEarCombId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Rabbits_RabbitEarCombId",
                        column: x => x.RabbitEarCombId,
                        principalTable: "Rabbits",
                        principalColumn: "EarCombId");
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateRated = table.Column<DateOnly>(type: "date", nullable: false),
                    EarCombId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RabbitRatedEarCombId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WeightPoint = table.Column<int>(type: "int", nullable: false),
                    WeightNotice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyPoint = table.Column<int>(type: "int", nullable: false),
                    BodyNotice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FurPoint = table.Column<int>(type: "int", nullable: false),
                    FurNotice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalPoint = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Rabbits_RabbitRatedEarCombId",
                        column: x => x.RabbitRatedEarCombId,
                        principalTable: "Rabbits",
                        principalColumn: "EarCombId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransferRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RabbitId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IssuerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RecipentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Price = table.Column<int>(type: "int", nullable: true),
                    SaleConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateAccepted = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferRequests_AspNetUsers_IssuerId",
                        column: x => x.IssuerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequests_AspNetUsers_RecipentId",
                        column: x => x.RecipentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRequests_Rabbits_RabbitId",
                        column: x => x.RabbitId,
                        principalTable: "Rabbits",
                        principalColumn: "EarCombId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trimmings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RabbitId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DateTrimmed = table.Column<DateOnly>(type: "date", nullable: false),
                    FirstSortmentWeightGram = table.Column<int>(type: "int", nullable: false),
                    SecondSortmentWeightGram = table.Column<int>(type: "int", nullable: false),
                    DisposableWoolWeightGram = table.Column<int>(type: "int", nullable: false),
                    TimeUsedMinutes = table.Column<int>(type: "int", nullable: true),
                    HairLengthCm = table.Column<float>(type: "real", nullable: true),
                    WoolDensity = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trimmings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trimmings_Rabbits_RabbitId",
                        column: x => x.RabbitId,
                        principalTable: "Rabbits",
                        principalColumn: "EarCombId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Moderator", "MODERATOR" },
                    { "2", null, "Breeder", "BREEDER" },
                    { "3", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "BreederRegNo", "City", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoadNameAndNo", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType", "ZipCode" },
                values: new object[,]
                {
                    { "IdasId", 0, "5095", "Kirke Såby", "a2565a0e-1a05-438b-9038-cf59461dfeff", "IdaFriborg87@gmail.com", false, "Ida", "Friborg", false, null, "IDAFRIBORG87@GMAIL.COM", "IDAFRIBORG87@GMAIL.COM", "AQAAAAIAAYagAAAAEEC36zWHrYSn5f80RsW8dodWfGXMs6L6WndHxR9qDkAPKgVLUCpw/viNyrsvB59hBQ==", "27586455", false, "Fynsvej 14", "99ab4973-682d-4c5f-a755-0a38753c6404", false, "IdaFriborg87@gmail.com", "Breeder", 4060 },
                    { "MajasId", 0, "5053", "Benløse", "5a3ae753-4010-4d1a-be0f-85306fb92221", "MajaJoensen89@gmail.com", false, "Maja", "Hulstrøm", false, null, "MAJAJOENSEN89@GMAIL.COM", "MAJAJOENSEN89@GMAIL.COM", "AQAAAAIAAYagAAAAEF0y5I1XhmHHmvO5n1DfAefTX+HewnYPacwRwwjINL+gAnFKS4TXXseveTSOev7KYA==", "28733085", false, "Sletten 4", "e17b6fd0-0afc-4d45-b9bc-fbc0f29b3396", false, "MajaJoensen89@gmail.com", "Breeder", 4100 }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "City", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RoadNameAndNo", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType", "ZipCode" },
                values: new object[] { "MikkelsId", 0, "Kirke Såby", "29dc9ba5-5b5b-453c-b929-e1c4f9678e5d", "Mikk.fri@gmail.com", false, "Mikkel", "Friborg", false, null, "MIKK.FRI@GMAIL.COM", "MIKK.FRI@GMAIL.COM", "AQAAAAIAAYagAAAAEGcPmU+jGIF6vMoLU2t2/PuliuryBsPH9f+ICTcRJYbwJeeA+keCBZVCjV6s5mxxmw==", "81183394", false, "Fynsvej 14", "6400298b-232d-4c13-98be-c4906f84d37c", false, "Mikk.fri@gmail.com", "User", 4060 });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "User:Read", "Any", "3" },
                    { 2, "User:Create", "Any", "3" },
                    { 3, "User:Update", "Any", "3" },
                    { 4, "User:Delete", "Any", "3" },
                    { 5, "Rabbit:Create", "Any", "3" },
                    { 6, "Rabbit:Read", "Any", "3" },
                    { 7, "Rabbit:Update", "Any", "3" },
                    { 8, "Rabbit:Delete", "Any", "3" },
                    { 9, "Rabbit:Create", "Any", "1" },
                    { 10, "Rabbit:Read", "Any", "1" },
                    { 11, "Rabbit:Update", "Any", "1" },
                    { 12, "Rabbit:Delete", "Any", "1" },
                    { 13, "Rabbit:Create", "Own", "2" },
                    { 14, "Rabbit:Read", "Own", "2" },
                    { 15, "Rabbit:Update", "Own", "2" },
                    { 16, "Rabbit:Delete", "Own", "2" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "1", "IdasId" },
                    { "2", "MajasId" },
                    { "3", "MikkelsId" }
                });

            migrationBuilder.InsertData(
                table: "Rabbits",
                columns: new[] { "EarCombId", "Color", "DateOfBirth", "DateOfDeath", "FatherId_Placeholder", "Father_EarCombId", "ForBreeding", "ForSale", "Gender", "LeftEarId", "MotherId_Placeholder", "Mother_EarCombId", "NickName", "OriginId", "OwnerId", "Race", "RightEarId" },
                values: new object[,]
                {
                    { "3658-0819", 24, new DateOnly(2019, 5, 31), new DateOnly(2023, 1, 31), null, null, 0, 0, 0, "0819", null, null, "Karina", null, "MajasId", 0, "3658" },
                    { "4398-3020", 28, new DateOnly(2022, 7, 22), null, null, null, 1, 0, 1, "3020", null, null, "Douglas", null, "MajasId", 10, "4398" },
                    { "4640-105", 13, new DateOnly(2021, 4, 5), null, null, null, 1, 0, 1, "105", null, null, "Ingolf", null, "IdasId", 0, "4640" },
                    { "4640-120", 16, new DateOnly(2021, 5, 11), new DateOnly(2023, 11, 3), null, null, 0, 0, 0, "120", null, null, "Mulan", null, "IdasId", 0, "4640" },
                    { "4977-206", 15, new DateOnly(2022, 2, 2), new DateOnly(2024, 4, 5), "M63-044", null, 0, 0, 1, "206", "M63-989", null, "Dario", null, "MajasId", 17, "4977" },
                    { "4977-213", 6, new DateOnly(2022, 3, 24), null, "M63-125", null, 1, 0, 0, "213", "M63-155", null, "Frida", null, "MajasId", 17, "4977" },
                    { "4977-315", 15, new DateOnly(2023, 1, 13), new DateOnly(2024, 4, 15), "4977-205", null, 0, 0, 0, "315", "13-232", null, "Miranda", null, "MajasId", 17, "4977" },
                    { "5053-0120", 28, new DateOnly(2020, 3, 25), new DateOnly(2021, 5, 31), null, null, 0, 0, 0, "0120", null, null, "Ulla", "MajasId", "MajasId", 0, "5053" },
                    { "5095-001", 18, new DateOnly(2019, 2, 27), new DateOnly(2024, 4, 13), null, null, 0, 0, 0, "001", null, null, "Kaliba", "IdasId", "IdasId", 0, "5095" },
                    { "M63-2104", 24, new DateOnly(2023, 5, 22), null, "M63-085", null, 1, 0, 0, "2104", "M63-164", null, "Ortovi", null, "MajasId", 17, "M63" },
                    { "M63-3102", 19, new DateOnly(2023, 9, 23), null, "M63-0204", null, 1, 0, 0, "3102", "M63-0000", null, "Xådda", null, "MajasId", 17, "M63" },
                    { "V23-023", 4, new DateOnly(2020, 4, 10), new DateOnly(2024, 4, 23), null, null, 0, 0, 1, "023", null, null, "Aslan", null, "MajasId", 17, "V23" },
                    { "5053-0123", 6, new DateOnly(2023, 5, 30), null, null, "4977-206", 0, 0, 0, "0123", null, "4977-213", "Pichu", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0223", 16, new DateOnly(2023, 5, 30), null, null, "4977-206", 1, 0, 0, "0223", null, "4977-213", "Chinchou", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0323", 15, new DateOnly(2023, 8, 17), new DateOnly(2023, 12, 18), null, "4977-206", 0, 0, 1, "0323", null, "4977-213", "Hunter", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0423", 15, new DateOnly(2023, 5, 30), null, null, "4977-206", 0, 0, 0, "0423", null, "4977-213", "Gastly", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0523", 6, new DateOnly(2023, 8, 17), null, null, "4977-206", 0, 0, 1, "0523", null, "4977-213", "Charizard", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0623", 19, new DateOnly(2023, 8, 17), null, null, "4977-206", 0, 0, 0, "0623", null, "4977-213", "Karla", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0723", 21, new DateOnly(2023, 10, 15), null, null, "4977-206", 1, 1, 1, "0723", null, "4977-315", "Sandshrew", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0823", 27, new DateOnly(2023, 10, 15), null, null, "4977-206", 1, 0, 0, "0823", null, "4977-315", "Pepsi", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0923", 26, new DateOnly(2023, 10, 15), null, null, "4977-206", 1, 0, 1, "0923", null, "4977-315", "Cola", "MajasId", "MajasId", 17, "5053" },
                    { "5053-1023", 21, new DateOnly(2023, 10, 15), null, null, "4977-206", 0, 1, 0, "1023", null, "4977-315", "Marabou", "MajasId", "MajasId", 17, "5053" },
                    { "5095-002", 16, new DateOnly(2020, 6, 12), new DateOnly(2022, 7, 22), null, null, 0, 0, 0, "002", null, "5095-001", "Sov", "IdasId", "IdasId", 0, "5095" },
                    { "5095-003", 28, new DateOnly(2020, 3, 12), new DateOnly(2023, 11, 3), null, null, 0, 0, 0, "003", null, "5095-001", "Smørklat Smør", "IdasId", "IdasId", 0, "5095" },
                    { "5053-0124", 6, new DateOnly(2024, 4, 1), null, "V23-023", null, 0, 0, 1, "0124", null, "5053-0423", "Rollo Darminatan", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0224", 18, new DateOnly(2024, 4, 18), null, null, "5053-0723", 0, 0, 0, "0224", null, "M63-3102", "Chokolade", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0324", 18, new DateOnly(2024, 4, 18), null, null, "5053-0723", 0, 0, 1, "0324", null, "M63-3102", "Beartic", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0524", 21, new DateOnly(2024, 4, 18), null, null, "5053-0723", 0, 0, 1, "0524", null, "M63-3102", "Metchi", "MajasId", "MajasId", 17, "5053" },
                    { "5053-0724", 19, new DateOnly(2024, 4, 18), null, null, "5053-0723", 0, 0, 1, "0724", null, "M63-3102", "Dewgong", "MajasId", "MajasId", 17, "5053" },
                    { "5053-10724", 18, new DateOnly(2024, 4, 18), null, null, "5053-0723", 0, 0, 0, "10724", null, "M63-3102", "Ice Beam", "MajasId", "MajasId", 17, "5053" },
                    { "5095-0124", 27, new DateOnly(2024, 5, 7), null, "V23-023", null, 1, 0, 1, "0124", null, "5053-0223", "Aron", "IdasId", "IdasId", 17, "5095" },
                    { "5095-0224", 6, new DateOnly(2024, 5, 7), null, "V23-023", null, 1, 1, 0, "0224", null, "5053-0223", "Azelf", "IdasId", "IdasId", 17, "5095" },
                    { "5095-0324", 6, new DateOnly(2024, 5, 7), null, "V23-023", null, 0, 0, 1, "0324", null, "5053-0223", "Arcanine", "IdasId", "IdasId", 17, "5095" },
                    { "5095-0624", 5, new DateOnly(2024, 5, 7), null, "V23-023", null, 0, 0, 0, "0624", null, "5053-0223", "Articuno", "IdasId", "IdasId", 17, "5095" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_BreederRegNo",
                table: "AspNetUsers",
                column: "BreederRegNo",
                unique: true,
                filter: "[BreederRegNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BreederApplications_UserApplicantId",
                table: "BreederApplications",
                column: "UserApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_BreederBrands_UserId",
                table: "BreederBrands",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_UserId",
                table: "Favorite",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_RabbitEarCombId",
                table: "Photos",
                column: "RabbitEarCombId");

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_Father_EarCombId",
                table: "Rabbits",
                column: "Father_EarCombId");

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_Mother_EarCombId",
                table: "Rabbits",
                column: "Mother_EarCombId");

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_OriginId",
                table: "Rabbits",
                column: "OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_Rabbits_OwnerId",
                table: "Rabbits",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_RabbitRatedEarCombId",
                table: "Ratings",
                column: "RabbitRatedEarCombId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_IssuerId",
                table: "TransferRequests",
                column: "IssuerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_RabbitId",
                table: "TransferRequests",
                column: "RabbitId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRequests_RecipentId",
                table: "TransferRequests",
                column: "RecipentId");

            migrationBuilder.CreateIndex(
                name: "IX_Trimmings_RabbitId",
                table: "Trimmings",
                column: "RabbitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BreederApplications");

            migrationBuilder.DropTable(
                name: "BreederBrands");

            migrationBuilder.DropTable(
                name: "Favorite");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "TransferRequests");

            migrationBuilder.DropTable(
                name: "Trimmings");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Rabbits");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
