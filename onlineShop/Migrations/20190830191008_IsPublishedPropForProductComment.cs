using Microsoft.EntityFrameworkCore.Migrations;

namespace onlineShop.Migrations
{
    public partial class IsPublishedPropForProductComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "ProductComments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "ProductComments");
        }
    }
}
