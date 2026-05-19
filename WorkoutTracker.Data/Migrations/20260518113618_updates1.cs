using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class updates1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Workout_Start",
                table: "Workout");

            migrationBuilder.DropCheckConstraint(
                name: "CK_End_Start",
                table: "Workout");

            migrationBuilder.DropColumn(
                name: "End",
                table: "Workout");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Workout");

            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "WorkoutSession",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "WorkoutSession",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Reps",
                table: "WorkoutExercise",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sets",
                table: "WorkoutExercise",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSession_Start",
                table: "WorkoutSession",
                column: "Start");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Start_End",
                table: "WorkoutSession",
                sql: "[Start] <= [End]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkoutSession_Start",
                table: "WorkoutSession");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Start_End",
                table: "WorkoutSession");

            migrationBuilder.DropColumn(
                name: "End",
                table: "WorkoutSession");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "WorkoutSession");

            migrationBuilder.DropColumn(
                name: "Reps",
                table: "WorkoutExercise");

            migrationBuilder.DropColumn(
                name: "Sets",
                table: "WorkoutExercise");

            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "Workout",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "Workout",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Workout_Start",
                table: "Workout",
                column: "Start");

            migrationBuilder.AddCheckConstraint(
                name: "CK_End_Start",
                table: "Workout",
                sql: "[Start] <= [End]");
        }
    }
}
