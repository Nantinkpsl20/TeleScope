using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teleScope.Migrations
{
    /// <inheritdoc />
    public partial class changeDateTimeType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
        name: "issue_date",
        table: "bills",
        type: "datetime2",
        nullable: false,
        oldClrType: typeof(DateTime),
        oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "due_date",
                table: "bills",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "phone_numbers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
        name: "issue_date",
        table: "bills",
        type: "datetime",
        nullable: false,
        oldClrType: typeof(DateTime),
        oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "due_date",
                table: "bills",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "phone_numbers",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
