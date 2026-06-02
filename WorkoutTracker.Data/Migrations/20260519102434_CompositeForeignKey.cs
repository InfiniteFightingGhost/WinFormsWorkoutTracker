using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WorkoutTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class CompositeForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_Workouts_WorkoutId",
                table: "WorkoutExercises");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutSessions_Workouts_WorkoutId",
                table: "WorkoutSessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutSessions_WorkoutId",
                table: "WorkoutSessions");

            migrationBuilder.DropColumn(
                name: "Reps",
                table: "WorkoutExercises");

            migrationBuilder.DropColumn(
                name: "Sets",
                table: "WorkoutExercises");

            migrationBuilder.RenameColumn(
                name: "WorkoutId",
                table: "WorkoutSessions",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MuscleGroups",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(30)",
                oldUnicode: false,
                oldMaxLength: 30);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions",
                column: "UserId");

            migrationBuilder.CreateTable(
                name: "ExerciseSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkoutExerciseId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    Repetitions = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    SetType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseSets_WorkoutExercises_WorkoutExerciseId_ExerciseId",
                        columns: x => new { x.WorkoutExerciseId, x.ExerciseId },
                        principalTable: "WorkoutExercises",
                        principalColumns: new[] { "WorkoutId", "ExerciseId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MuscleGroups",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Chest (Upper/Mid/Lower)" },
                    { 2, "Lats" },
                    { 3, "Upper Back (Traps/Rhomboids)" },
                    { 4, "Lower Back (Erectors)" },
                    { 5, "Front Delts" },
                    { 6, "Side Delts" },
                    { 7, "Rear Delts" },
                    { 8, "Biceps" },
                    { 9, "Triceps" },
                    { 10, "Forearms" },
                    { 11, "Quadriceps" },
                    { 12, "Hamstrings" },
                    { 13, "Glutes" },
                    { 14, "Hip Abductors" },
                    { 15, "Hip Adductors" },
                    { 16, "Calves" },
                    { 17, "Abs (Rectus Abdominis)" },
                    { 18, "Obliques" },
                    { 19, "Deep Core (Transverse Abdominis)" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Gender", "Height", "Password", "Role", "Username", "Weight" },
                values: new object[] { 1, "admin@admin.com", "Male", 180m, "admin123", "Admin", "admin", 75m });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSets_OrderIndex",
                table: "ExerciseSets",
                column: "OrderIndex");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseSets_WorkoutExerciseId_ExerciseId",
                table: "ExerciseSets",
                columns: new[] { "WorkoutExerciseId", "ExerciseId" });

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_WorkoutSessions_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId",
                principalTable: "WorkoutSessions",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutExercises_WorkoutSessions_WorkoutId",
                table: "WorkoutExercises");

            migrationBuilder.DropTable(
                name: "ExerciseSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions");

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MuscleGroups",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WorkoutSessions",
                newName: "WorkoutId");

            migrationBuilder.AddColumn<int>(
                name: "Reps",
                table: "WorkoutExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sets",
                table: "WorkoutExercises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MuscleGroups",
                type: "varchar(30)",
                unicode: false,
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldUnicode: false,
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkoutSessions",
                table: "WorkoutSessions",
                columns: new[] { "UserId", "WorkoutId" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutSessions_WorkoutId",
                table: "WorkoutSessions",
                column: "WorkoutId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutExercises_Workouts_WorkoutId",
                table: "WorkoutExercises",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutSessions_Workouts_WorkoutId",
                table: "WorkoutSessions",
                column: "WorkoutId",
                principalTable: "Workouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
