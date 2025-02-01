using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace teleScope.Migrations
{
    /// <inheritdoc />
    public partial class autoDeleteBill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Διαγραφή του υπάρχοντος foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK__bills__customer___0D7A0286",
                table: "bills");

            // Προσθήκη του νέου foreign key με ON DELETE CASCADE
            migrationBuilder.AddForeignKey(
                name: "FK__bills__customer___0D7A0286",
                table: "bills",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
     
            migrationBuilder.DropForeignKey(
                name: "FK__bills__customer___0D7A0286",
                table: "bills");

            migrationBuilder.AddForeignKey(
                name: "FK__bills__customer___0D7A0286",
                table: "bills",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Restrict); // Προηγούμενη συμπεριφορά
        }

    }
}

