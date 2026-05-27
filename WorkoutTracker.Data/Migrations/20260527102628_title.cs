using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class title : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Start_End",
                table: "WorkoutSessions");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "WorkoutSessions",
                type: "varchar(100)",
                unicode: false,
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "WorkoutSessions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Start_End",
                table: "WorkoutSessions",
                sql: "[Start] <= [End]");
        }
    }
}
