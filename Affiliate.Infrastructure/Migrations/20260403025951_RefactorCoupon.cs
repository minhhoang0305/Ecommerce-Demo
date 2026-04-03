using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Affiliate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCoupon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercent",
                table: "Coupon");

            migrationBuilder.AddColumn<int>(
                name: "DiscountType",
                table: "Coupon",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MinOrderValue",
                table: "Coupon",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "Coupon",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AppliedCouponCode",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AppliedDiscount",
                table: "Carts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountType",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "MinOrderValue",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "Coupon");

            migrationBuilder.DropColumn(
                name: "AppliedCouponCode",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "AppliedDiscount",
                table: "Carts");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercent",
                table: "Coupon",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
