using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Data.Migrations
{
    public partial class InitialCreate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    UserName = table.Column<string>(maxLength: 50, nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    Password = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.UserName);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    TopicID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicName = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.TopicID);
                });

            migrationBuilder.CreateTable(
                name: "Trainee_Assignment",
                columns: table => new
                {
                    RegistrationCode = table.Column<string>(maxLength: 50, nullable: false),
                    TraineeID = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainee_Assignment", x => new { x.RegistrationCode, x.TraineeID });
                });

            migrationBuilder.CreateTable(
                name: "Trainee_Comment",
                columns: table => new
                {
                    ClassID = table.Column<int>(nullable: false),
                    ModuleID = table.Column<int>(nullable: false),
                    TraineeID = table.Column<string>(maxLength: 50, nullable: false),
                    Comment = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainee_Comment", x => new { x.ClassID, x.ModuleID, x.TraineeID });
                });

            migrationBuilder.CreateTable(
                name: "TypeFeedback",
                columns: table => new
                {
                    TypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(maxLength: 50, nullable: true),
                    IsDelete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeFeedback", x => x.TypeID);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 255, nullable: true),
                    AdminID = table.Column<int>(nullable: false),
                    AdminUserName = table.Column<string>(nullable: true),
                    IsDelete = table.Column<bool>(nullable: false),
                    TypeFeedbackID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK_Feedback_Admin_AdminUserName",
                        column: x => x.AdminUserName,
                        principalTable: "Admin",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedback_TypeFeedback_TypeFeedbackID",
                        column: x => x.TypeFeedbackID,
                        principalTable: "TypeFeedback",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedback_Question",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(nullable: false),
                    QuestionID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback_Question", x => new { x.FeedbackID, x.QuestionID });
                    table.ForeignKey(
                        name: "FK_Feedback_Question_Feedback_FeedbackID",
                        column: x => x.FeedbackID,
                        principalTable: "Feedback",
                        principalColumn: "FeedbackID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    ModuleID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminID = table.Column<int>(nullable: false),
                    AdminUserName = table.Column<string>(nullable: true),
                    ModuleName = table.Column<string>(maxLength: 50, nullable: true),
                    StartTime = table.Column<DateTime>(nullable: false),
                    EndTime = table.Column<DateTime>(nullable: false),
                    IsDelete = table.Column<bool>(nullable: false),
                    FeedbackStartTime = table.Column<DateTime>(nullable: false),
                    FeedbackEndTime = table.Column<DateTime>(nullable: false),
                    FeedbackID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.ModuleID);
                    table.ForeignKey(
                        name: "FK_Module_Admin_AdminUserName",
                        column: x => x.AdminUserName,
                        principalTable: "Admin",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Module_Feedback_FeedbackID",
                        column: x => x.FeedbackID,
                        principalTable: "Feedback",
                        principalColumn: "FeedbackID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_AdminUserName",
                table: "Feedback",
                column: "AdminUserName");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_TypeFeedbackID",
                table: "Feedback",
                column: "TypeFeedbackID");

            migrationBuilder.CreateIndex(
                name: "IX_Module_AdminUserName",
                table: "Module",
                column: "AdminUserName");

            migrationBuilder.CreateIndex(
                name: "IX_Module_FeedbackID",
                table: "Module",
                column: "FeedbackID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback_Question");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "Trainee_Assignment");

            migrationBuilder.DropTable(
                name: "Trainee_Comment");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "TypeFeedback");
        }
    }
}
