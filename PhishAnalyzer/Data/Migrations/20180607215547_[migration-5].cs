using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PhishAnalyzer.Data.Migrations
{
    public partial class migration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "headers",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "senderIP",
                table: "Message",
                newName: "headerSender");

            migrationBuilder.AddColumn<bool>(
                name: "headerSenderValid",
                table: "Message",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "headerSenderValid",
                table: "Message");

            migrationBuilder.RenameColumn(
                name: "headerSender",
                table: "Message",
                newName: "senderIP");

            migrationBuilder.AddColumn<string>(
                name: "headers",
                table: "Message",
                nullable: true);
        }
    }
}
