using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class idk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_WorkoutSessions_WorkoutId",
                table: "WorkoutExercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WorkoutSessions");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WorkoutSessions",
                type: "int",
                nullable: false
                ).Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "WorkoutSessions",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_UserId",
                table: "WorkoutSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_WorkoutSessions_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId",
                principalTable: "WorkoutSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_WorkoutSessions_WorkoutId",
                table: "WorkoutExercises");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_UserId",
                table: "WorkoutSessions");
            
            migrationBuilder.DropColumn(
                name: "Id",
                table: "WorkoutSessions");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WorkoutSessions",
                type: "int",
                nullable: false
                ).Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "WorkoutSessions",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_WorkoutSessions_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId",
                principalTable: "WorkoutSessions",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
