using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class NewDatabaseStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_AspNetUsers_AplicationUserId",
                table: "UserContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_AspNetUsers_ContactsId",
                table: "UserContacts");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UserId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ContactsId",
                table: "UserContacts",
                newName: "ContactId");

            migrationBuilder.RenameColumn(
                name: "AplicationUserId",
                table: "UserContacts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserContacts_ContactsId",
                table: "UserContacts",
                newName: "IX_UserContacts_ContactId");

            migrationBuilder.AddColumn<string>(
                name: "ContactId",
                table: "Messages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId_ContactId",
                table: "Messages",
                columns: new[] { "UserId", "ContactId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_UserContacts_UserId_ContactId",
                table: "Messages",
                columns: new[] { "UserId", "ContactId" },
                principalTable: "UserContacts",
                principalColumns: new[] { "UserId", "ContactId" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContacts_AspNetUsers_ContactId",
                table: "UserContacts",
                column: "ContactId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContacts_AspNetUsers_UserId",
                table: "UserContacts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_UserContacts_UserId_ContactId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_AspNetUsers_ContactId",
                table: "UserContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_AspNetUsers_UserId",
                table: "UserContacts");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UserId_ContactId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "ContactId",
                table: "UserContacts",
                newName: "ContactsId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserContacts",
                newName: "AplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserContacts_ContactId",
                table: "UserContacts",
                newName: "IX_UserContacts_ContactsId");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContacts_AspNetUsers_AplicationUserId",
                table: "UserContacts",
                column: "AplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContacts_AspNetUsers_ContactsId",
                table: "UserContacts",
                column: "ContactsId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
