using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PhishAnalyzer.Data.Migrations
{
    public partial class Migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "headers",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "mxRecords",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "riskRating",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "senderDomain",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "senderDomainRegDate",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "senderIP",
                table: "Message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "headers",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "mxRecords",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "riskRating",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "senderDomain",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "senderDomainRegDate",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "senderIP",
                table: "Message");
        }
    }
}
