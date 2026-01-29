using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SaveForPerksAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameUsersToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reward_redemption_user",
                table: "reward_redemption");

            migrationBuilder.DropForeignKey(
                name: "fk_scan_event_user",
                table: "scan_event");

            migrationBuilder.DropForeignKey(
                name: "fk_user_balance_user",
                table: "user_balance");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DeleteData(
                table: "reward",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    auth_provider_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    qr_code_value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__3213E83F4461F44E", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "reward",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "name",
                value: "Pay for 5 coffees, get sixth free");

            migrationBuilder.InsertData(
                table: "user",
                columns: new[] { "id", "auth_provider_id", "created_at", "email", "name", "qr_code_value" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999999"), "auth0|user001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "alice@example.com", "Alice Customer", "QR001-ALICE-9999" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "auth0|user002", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bob@example.com", "Bob Customer", "QR002-BOB-AAAA" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "auth0|user003", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "charlie@example.com", "Charlie Customer", "QR003-CHARLIE-BBBB" }
                });

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E6164CB0A1AE0",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__users__C82CBBE99CDF45A3",
                table: "user",
                column: "auth_provider_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__users__C8EB4B8153934E5A",
                table: "user",
                column: "qr_code_value",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_reward_redemption_user",
                table: "reward_redemption",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_scan_event_user",
                table: "scan_event",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_balance_user",
                table: "user_balance",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reward_redemption_user",
                table: "reward_redemption");

            migrationBuilder.DropForeignKey(
                name: "fk_scan_event_user",
                table: "scan_event");

            migrationBuilder.DropForeignKey(
                name: "fk_user_balance_user",
                table: "user_balance");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    auth_provider_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    qr_code_value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__3213E83F4461F44E", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "reward",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "name",
                value: "Free Coffee at 5 points");

            migrationBuilder.InsertData(
                table: "reward",
                columns: new[] { "id", "cost_points", "created_at", "is_active", "metadata", "name", "reward_owner_id", "reward_type" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, "Wedding Drink Allowance of 2 drinks", new Guid("22222222-2222-2222-2222-222222222222"), "allowance_limit" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "auth_provider_id", "created_at", "email", "name", "qr_code_value" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999999"), "auth0|user001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "alice@example.com", "Alice Customer", "QR001-ALICE-9999" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "auth0|user002", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bob@example.com", "Bob Customer", "QR002-BOB-AAAA" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "auth0|user003", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "charlie@example.com", "Charlie Customer", "QR003-CHARLIE-BBBB" }
                });

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E6164CB0A1AE0",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__users__C82CBBE99CDF45A3",
                table: "users",
                column: "auth_provider_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__users__C8EB4B8153934E5A",
                table: "users",
                column: "qr_code_value",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_reward_redemption_user",
                table: "reward_redemption",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_scan_event_user",
                table: "scan_event",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_balance_user",
                table: "user_balance",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
