using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class muscleGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MuscleGroupId",
                table: "Exercise",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MuscleGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuscleGroup", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_MuscleGroupId",
                table: "Exercise",
                column: "MuscleGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_MuscleGroup_MuscleGroupId",
                table: "Exercise",
                column: "MuscleGroupId",
                principalTable: "MuscleGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_MuscleGroup_MuscleGroupId",
                table: "Exercise");

            migrationBuilder.DropTable(
                name: "MuscleGroup");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_MuscleGroupId",
                table: "Exercise");

            migrationBuilder.DropColumn(
                name: "MuscleGroupId",
                table: "Exercise");
        }
    }
}
