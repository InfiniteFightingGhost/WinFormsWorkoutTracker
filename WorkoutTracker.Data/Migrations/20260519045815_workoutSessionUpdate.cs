using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class workoutSessionUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Workout");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Workout");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "WorkoutSession",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "WorkoutSession",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "WorkoutSession");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "WorkoutSession");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Workout",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Workout",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
