using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DB_AngoraREST.Migrations
{
    /// <inheritdoc />
    public partial class AngoraMig02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePic",
                table: "Rabbits",
                newName: "ProfilePicture");

            migrationBuilder.RenameColumn(
                name: "ProfilePic",
                table: "AspNetUsers",
                newName: "ProfilePicture");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "IdasId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5ee28dc3-3b8f-4b3c-9e3e-e6e655937d6c", "AQAAAAIAAYagAAAAEN8CzH8ODNtMRZckiaQFlQpEoqZpPzXOdC6Z4wx/F21uOQcnTkEp/xW3/vsWavm/Tw==", "c6c51119-864d-4544-aeaf-4da03f64dcfe" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "MajasId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d06efda5-9597-44f7-baed-faff5057893a", "AQAAAAIAAYagAAAAEOeCAgWpbT6E651aWVXMklXJc5spRPXsz7WBrOsJoaCYeQxhtQ9UBSh+oa0tqQqLpQ==", "3cce23cd-b03a-4fba-8ebf-3638187cacbd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "MikkelsId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "95ce733d-b184-44db-b1f3-4c70404af9a1", "AQAAAAIAAYagAAAAEDC255sWQZtr9+vL2N257o2Mu9OY0bAqpvlKDFC8MvuROJnC8edzCr99zZlUGjez9A==", "0d73dbda-34f5-4b23-b9ca-54fbaba1bb8c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePicture",
                table: "Rabbits",
                newName: "ProfilePic");

            migrationBuilder.RenameColumn(
                name: "ProfilePicture",
                table: "AspNetUsers",
                newName: "ProfilePic");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "IdasId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "578d6628-7dd5-459b-b9e6-335efb9e3805", "AQAAAAIAAYagAAAAEMtvlJHSahmJABUeTWBSct3cn9RlSU5Vh5X2ZB0GJfIZcZKSBzs/WN8v7A9sVTCJjw==", "509cf922-25db-469d-9d98-66ea72b14ba7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "MajasId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d80f99dd-d4cf-45ae-afa0-1da479a9d68d", "AQAAAAIAAYagAAAAECaQMmEGlqOx8KrmWfTnZgPTEBtaVqOoPMszFfpJcUy8Todir6nPaqX6Bu3ElYgK7Q==", "2a4a28a0-848b-43d7-9ae8-7d95e02f3554" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "MikkelsId",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c49a0173-450f-43ca-9b73-9b412aabebea", "AQAAAAIAAYagAAAAECQiVthBfYBjIUREklL2aseJEIEXk5sWS+rBFOFIXnos3poVfPLz+KglR10qt9R0jA==", "35d29d92-a04f-4397-816e-4ece51c585e2" });
        }
    }
}
