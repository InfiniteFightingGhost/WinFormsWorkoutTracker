 using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixDateTimeError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Start_End",
                table: "WorkoutSessions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "WorkoutSessions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "WorkoutSessions",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Start_End",
                table: "WorkoutSessions",
                sql: "[Start] <= [End]");
        }
    }
}
