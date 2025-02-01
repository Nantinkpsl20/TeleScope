using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teleScope.Migrations
{
    /// <inheritdoc />
    public partial class changeDestNumType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alter the 'destination_number' column type from INT to BIGINT
            migrationBuilder.AlterColumn<long>(
                name: "destination_number",
                table: "calls",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INT");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the change: Alter the 'phone' column type back to INT
            migrationBuilder.AlterColumn<int>(
                name: "destination_number",
                table: "calls",
                type: "INT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT");

        }

    }

}
