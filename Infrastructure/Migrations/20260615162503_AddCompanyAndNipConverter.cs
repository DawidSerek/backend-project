using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyAndNipConverter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployerId",
                table: "Contacts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Contacts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nip",
                table: "Contacts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Regon",
                table: "Contacts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Contacts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_EmployerId",
                table: "Contacts",
                column: "EmployerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_Contacts_EmployerId",
                table: "Contacts",
                column: "EmployerId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_Contacts_EmployerId",
                table: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_EmployerId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "EmployerId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Nip",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Regon",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Contacts");
        }
    }
}
