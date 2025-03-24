using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auth.Migrations
{
    /// <inheritdoc />
    public partial class AddFriendshipStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Friendships",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d7657470-e560-4fd7-aea7-268d17a18700", "AQAAAAIAAYagAAAAEPxXKGBEVKKvPSdO9cHulyCDdA6l2ZcDKzVZABvzqvfm69pJUyPTHl3SwEL0BiiNvw==", "eb4e4b2d-d8ae-4174-b5ef-51a8cbd99968" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Friendships");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "86190c50-5416-4a2c-bd39-e911a7052e26", "AQAAAAIAAYagAAAAEAv7clt7w5chTzEshyqMZDdBLINEkf/MI9uLoRQDY1nJFXxWSeKzL3LqPJKc6cvyrA==", "c3d2dea8-2213-452b-9789-63a3e9c3409d" });
        }
    }
}
