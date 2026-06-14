using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_Contact_OrganizationId",
                table: "Contact");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contact",
                table: "Contact");

            migrationBuilder.RenameTable(
                name: "Contact",
                newName: "Contacts");

            migrationBuilder.RenameIndex(
                name: "IX_Contact_OrganizationId",
                table: "Contacts",
                newName: "IX_Contacts_OrganizationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Contacts_OrganizationId",
                table: "Contacts",
                column: "OrganizationId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Contacts_OrganizationId",
                table: "Contacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contacts",
                table: "Contacts");

            migrationBuilder.RenameTable(
                name: "Contacts",
                newName: "Contact");

            migrationBuilder.RenameIndex(
                name: "IX_Contacts_OrganizationId",
                table: "Contact",
                newName: "IX_Contact_OrganizationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contact",
                table: "Contact",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_Contact_OrganizationId",
                table: "Contact",
                column: "OrganizationId",
                principalTable: "Contact",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
