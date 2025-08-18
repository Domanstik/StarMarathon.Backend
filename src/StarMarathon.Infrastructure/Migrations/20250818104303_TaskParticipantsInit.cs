using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StarMarathon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TaskParticipantsInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskParticipants_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskParticipants_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskParticipants_EmployeeId",
                table: "TaskParticipants",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskParticipants_TaskId_EmployeeId",
                table: "TaskParticipants",
                columns: new[] { "TaskId", "EmployeeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskParticipants");
        }
    }
}
