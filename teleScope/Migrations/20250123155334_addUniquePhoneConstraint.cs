using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teleScope.Migrations
{
    /// <inheritdoc />
    public partial class addUniquePhoneConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
              name: "UQ_phone",
              table: "phone_numbers",
              column: "phone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the UNIQUE constraint
            migrationBuilder.DropUniqueConstraint(
                name: "UQ_phone",
                table: "phone_numbers");
        }
    }
}
