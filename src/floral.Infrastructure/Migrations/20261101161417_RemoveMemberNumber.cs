using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace floral.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMemberNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ux_users_member_number",
                table: "users");

            migrationBuilder.DropColumn(
                name: "member_number",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "member_number",
                table: "users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ux_users_member_number",
                table: "users",
                column: "member_number",
                unique: true);
        }
    }
}
