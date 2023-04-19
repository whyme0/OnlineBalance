using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineBalance.Migrations
{
    public partial class Add_IsGotMoneyGift_FieldForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGotMoneyGift",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGotMoneyGift",
                table: "Users");
        }
    }
}
