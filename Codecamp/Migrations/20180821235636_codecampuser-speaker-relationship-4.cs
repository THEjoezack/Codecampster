﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Codecamp.Migrations
{
    public partial class codecampuserspeakerrelationship4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Speakers_CodecampUserId",
                table: "Speakers");

            migrationBuilder.AddColumn<int>(
                name: "SpeakerId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_CodecampUserId",
                table: "Speakers",
                column: "CodecampUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SpeakerId",
                table: "AspNetUsers",
                column: "SpeakerId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Speakers_SpeakerId",
                table: "AspNetUsers",
                column: "SpeakerId",
                principalTable: "Speakers",
                principalColumn: "SpeakerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Speakers_SpeakerId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_Speakers_CodecampUserId",
                table: "Speakers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SpeakerId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SpeakerId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_Speakers_CodecampUserId",
                table: "Speakers",
                column: "CodecampUserId",
                unique: true,
                filter: "[CodecampUserId] IS NOT NULL");
        }
    }
}
