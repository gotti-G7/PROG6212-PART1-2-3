
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDayApi1.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Users table
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    FullName = table.Column<string>(
                        type: "varchar(50)",
                        nullable: false),

                    Email = table.Column<string>(
                        type: "varchar(50)",
                        nullable: false),

                    Password = table.Column<string>(
                        type: "varchar(50)",
                        nullable: false),

                    Role = table.Column<string>(
                        type: "varchar(50)",
                        nullable: false,
                        defaultValue: "Participant")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);

                    table.CheckConstraint(
                        "CK_Users_Role",
                        "[Role] IN ('Organiser', 'Participant', 'Admin')");
                });

            // Create Categories table
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    Name = table.Column<string>(
                        type: "varchar(50)",
                        nullable: false),

                    DistanceKM = table.Column<decimal>(
                        type: "decimal(5,2)",
                        precision: 5,
                        scale: 2,
                        nullable: false),

                    Description = table.Column<string>(
                        type: "varchar(100)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Categories",
                        x => x.CategoryID);
                });

            // Create Events table
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventID = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    UserID = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Name = table.Column<string>(
                        type: "varchar(50)",
                        nullable: false),

                    Description = table.Column<string>(
                        type: "varchar(100)",
                        nullable: false),

                    EventDate = table.Column<DateTime>(
                        type: "date",
                        nullable: false),

                    Location = table.Column<string>(
                        type: "varchar(50)",
                        nullable: false),

                    RouteInfo = table.Column<string>(
                        type: "varchar(1000)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventID);

                    table.ForeignKey(
                        name: "FK_Events_Organiser",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create Event_Categories junction table
            migrationBuilder.CreateTable(
                name: "Event_Categories",
                columns: table => new
                {
                    EventID = table.Column<int>(
                        type: "int",
                        nullable: false),

                    CategoryID = table.Column<int>(
                        type: "int",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Event_Categories",
                        x => new { x.EventID, x.CategoryID });

                    table.ForeignKey(
                        name: "FK_EventCategories_Event",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_EventCategories_Category",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create Enrolments table
            migrationBuilder.CreateTable(
                name: "Enrolments",
                columns: table => new
                {
                    EnrolmentID = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    EventID = table.Column<int>(
                        type: "int",
                        nullable: false),

                    CategoryID = table.Column<int>(
                        type: "int",
                        nullable: false),

                    UserID = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Status = table.Column<string>(
                        type: "varchar(20)",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_Enrolments",
                        x => x.EnrolmentID);

                    table.ForeignKey(
                        name: "FK_Enrolments_Event",
                        column: x => x.EventID,
                        principalTable: "Events",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Enrolments_Category",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);

                    table.ForeignKey(
                        name: "FK_Enrolments_Users",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create Results table
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    ResultID = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation(
                            "SqlServer:Identity",
                            "1, 1"),

                    EnrolmentID = table.Column<int>(
                        type: "int",
                        nullable: false),

                    FinishTime = table.Column<TimeSpan>(
                        type: "time(7)",
                        nullable: false),

                    Position = table.Column<int>(
                        type: "int",
                        nullable: false),

                    RecordedAt = table.Column<DateTime>(
                        type: "datetime",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.ResultID);

                    table.ForeignKey(
                        name: "FK_Results_Enrolments",
                        column: x => x.EnrolmentID,
                        principalTable: "Enrolments",
                        principalColumn: "EnrolmentID",
                        onDelete: ReferentialAction.Restrict);
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserID",
                table: "Events",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Categories_CategoryID",
                table: "Event_Categories",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_EventID",
                table: "Enrolments",
                column: "EventID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_CategoryID",
                table: "Enrolments",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrolments_UserID",
                table: "Enrolments",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Results_EnrolmentID",
                table: "Results",
                column: "EnrolmentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop dependent tables first
            migrationBuilder.DropTable(name: "Results");
            migrationBuilder.DropTable(name: "Event_Categories");
            migrationBuilder.DropTable(name: "Enrolments");
            migrationBuilder.DropTable(name: "Events");
            migrationBuilder.DropTable(name: "Categories");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
    

