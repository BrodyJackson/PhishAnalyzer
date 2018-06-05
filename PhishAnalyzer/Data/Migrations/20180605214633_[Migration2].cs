using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PhishAnalyzer.Data.Migrations
{
    public partial class Migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "recieved",
                table: "Message",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "recipient",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "recipientCC",
                table: "Message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "recipient",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "recipientCC",
                table: "Message");

            migrationBuilder.AlterColumn<DateTime>(
                name: "recieved",
                table: "Message",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
