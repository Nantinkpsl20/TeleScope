using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teleScope.Migrations
{
    /// <inheritdoc />
    public partial class removeUniqueIndex : Migration
    {
        /// <inheritdoc />
    
            protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Alter the 'phone' column type from INT to BIGINT
            migrationBuilder.AlterColumn<long>(
                name: "phone",
                table: "phone_numbers",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INT");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the change: Alter the 'phone' column type back to INT
            migrationBuilder.AlterColumn<int>(
                name: "phone",
                table: "phone_numbers",
                type: "INT",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT");

        }
    
    }
}
