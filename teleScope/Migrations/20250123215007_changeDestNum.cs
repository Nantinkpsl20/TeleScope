using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teleScope.Migrations
{
    /// <inheritdoc />
    public partial class changeDestNum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Αλλάζουμε τη στήλη phone από BIGINT σε VARCHAR(20)
            migrationBuilder.AlterColumn<string>(
                name: "destination_number",
                table: "calls",
                type: "VARCHAR(20)", // Το νέο type της στήλης
                nullable: false,     // Αν επιτρέπεται να είναι NULL
                oldClrType: typeof(long), // Ο παλιός τύπος (BIGINT)
                oldType: "INT");

   
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Επαναφορά σε BIGINT αν γίνει rollback
            migrationBuilder.AlterColumn<long>(
                name: "destination_number",
                table: "calls",
                type: "INT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(20)");

        }
   
    }
}
