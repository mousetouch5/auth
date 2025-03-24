using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auth.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f608bd17-7cbe-4120-808b-9f5205be0089", "AQAAAAIAAYagAAAAEJbyDWCISxpFMFjFemdMJOQYjMSE62fyMLlcixPnXng+cK1SSaRMiQTRamB64Pjxcw==", "59431e38-6482-4935-97b8-1e843c290f49" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin-user-id",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "26acb561-3fa5-40d2-aea0-b9cd9ec064b9", "AQAAAAIAAYagAAAAEFPgICVdmq/wLWMQdF9yUnF7r8h7qxx4Cr7anBSK4mCb08oMXT1qeSa3BsaFY78kNg==", "a69050b9-8bb1-4a42-a16d-a2c308a90f41" });
        }
    }
}
