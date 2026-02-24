using Microsoft.EntityFrameworkCore.Migrations;

namespace Amazing.Persistence.Migrations
{
    public partial class _AddTypedContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Contents",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Contents",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Contents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Type",      table: "Contents");
            migrationBuilder.DropColumn(name: "ImageUrl",  table: "Contents");
            migrationBuilder.DropColumn(name: "Height",    table: "Contents");
            migrationBuilder.DropColumn(name: "Width",     table: "Contents");
            migrationBuilder.DropColumn(name: "VideoUrl",  table: "Contents");
            migrationBuilder.DropColumn(name: "Duration",  table: "Contents");
            migrationBuilder.DropColumn(name: "Longitude", table: "Contents");
            migrationBuilder.DropColumn(name: "Latitude",  table: "Contents");
            migrationBuilder.DropColumn(name: "Author",    table: "Contents");
        }
    }
}
