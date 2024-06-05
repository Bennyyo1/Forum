using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.Migrations
{
    /// <inheritdoc />
    public partial class addedVotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Votes",
                table: "Post",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReported",
                table: "Comment",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Votes",
                table: "Comment",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Votes",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "IsReported",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Votes",
                table: "Comment");
        }
    }
}
