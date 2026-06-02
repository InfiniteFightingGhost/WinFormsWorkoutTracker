using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class exerciseLengthsAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Exercises",
                type: "varchar(500)",
                unicode: false,
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Exercises",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldUnicode: false,
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "Exercises",
                type: "varchar(200)",
                unicode: false,
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldUnicode: false,
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Exercises",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldUnicode: false,
                oldMaxLength: 200);
        }
    }
}
