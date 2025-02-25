using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VresToRestau2.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AdminName = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "ProfessionalUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    RestaurantName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionalUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegisteredUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    ProfilePicture = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewId = table.Column<int>(type: "integer", nullable: false),
                    RestaurantName = table.Column<string>(type: "text", nullable: false),
                    ResponseDetails = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RestaurantName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Afm = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    MapLink = table.Column<string>(type: "text", nullable: false),
                    Cuisine = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<int>(type: "integer", nullable: false),
                    Hours = table.Column<string>(type: "text", nullable: false),
                    PhotosLink = table.Column<string>(type: "text", nullable: false),
                    MenuLink = table.Column<string>(type: "text", nullable: false),
                    Stars = table.Column<int>(type: "integer", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false),
                    Stars = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteRestaurants",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RestaurantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteRestaurants", x => new { x.UserId, x.RestaurantId });
                    table.ForeignKey(
                        name: "FK_FavoriteRestaurants_RegisteredUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "RegisteredUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteRestaurants_Restaurants_RestaurantId",
                        column: x => x.RestaurantId,
                        principalTable: "Restaurants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteRestaurants_RestaurantId",
                table: "FavoriteRestaurants",
                column: "RestaurantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "FavoriteRestaurants");

            migrationBuilder.DropTable(
                name: "ProfessionalUsers");

            migrationBuilder.DropTable(
                name: "Responses");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "RegisteredUsers");

            migrationBuilder.DropTable(
                name: "Restaurants");
        }
    }
}
