using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SaveForPerksAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIdToCustomerIdInRewardRedemption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reward_redemption_customer_user_id",
                table: "reward_redemption");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "reward_redemption",
                newName: "customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_reward_redemption_user_id",
                table: "reward_redemption",
                newName: "IX_reward_redemption_customer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_reward_redemption_customer_customer_id",
                table: "reward_redemption",
                column: "customer_id",
                principalTable: "customer",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reward_redemption_customer_customer_id",
                table: "reward_redemption");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "reward_redemption",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_reward_redemption_customer_id",
                table: "reward_redemption",
                newName: "IX_reward_redemption_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_reward_redemption_customer_user_id",
                table: "reward_redemption",
                column: "user_id",
                principalTable: "customer",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
