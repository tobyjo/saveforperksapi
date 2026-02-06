using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SaveForPerksAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRewardOwnerCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "reward_owner",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "reward_owner_category",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__reward_owner_category__id", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "reward_owner",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "category_id",
                value: null);

            migrationBuilder.UpdateData(
                table: "reward_owner",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "category_id",
                value: null);

            migrationBuilder.InsertData(
                table: "reward_owner_category",
                columns: new[] { "id", "image_url", "name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "/images/categories/cafe.png", "Cafe" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "/images/categories/coffee-shop.png", "Coffee Shop" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "/images/categories/restaurant.png", "Restaurant" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "/images/categories/bakery.png", "Bakery" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "/images/categories/bar-pub.png", "Bar & Pub" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_reward_owner_category_id",
                table: "reward_owner",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "UQ__reward_owner_category__name",
                table: "reward_owner_category",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_reward_owner_category",
                table: "reward_owner",
                column: "category_id",
                principalTable: "reward_owner_category",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_reward_owner_category",
                table: "reward_owner");

            migrationBuilder.DropTable(
                name: "reward_owner_category");

            migrationBuilder.DropIndex(
                name: "IX_reward_owner_category_id",
                table: "reward_owner");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "reward_owner");
        }
    }
}
