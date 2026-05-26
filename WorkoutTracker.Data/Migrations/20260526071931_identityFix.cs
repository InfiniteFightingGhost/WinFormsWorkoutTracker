using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class identityFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderIndex_Allowed",
                table: "WorkoutExercises");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderIndex_Allowed",
                table: "WorkoutExercises",
                sql: "[OrderIndex] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_OrderIndex_Allowed",
                table: "WorkoutExercises");

            migrationBuilder.AddCheckConstraint(
                name: "CK_OrderIndex_Allowed",
                table: "WorkoutExercises",
                sql: "[OrderIndex] > 0");
        }
    }
}
