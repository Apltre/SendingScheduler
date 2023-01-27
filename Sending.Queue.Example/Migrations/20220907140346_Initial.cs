using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sending.Queue.Example.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "send_jobs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    handle_order = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    attempt_number = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    service_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_send_jobs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "result_handling_jobs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    send_job_id = table.Column<long>(type: "bigint", nullable: false),
                    send_job_status = table.Column<int>(type: "integer", nullable: false),
                    send_job_error_data = table.Column<string>(type: "jsonb", nullable: true),
                    error_data = table.Column<string>(type: "jsonb", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    attempt_number = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    service_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_result_handling_jobs", x => x.id);
                    table.ForeignKey(
                        name: "fk_result_handling_jobs_send_jobs_send_job_id",
                        column: x => x.send_job_id,
                        principalTable: "send_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "send_jobs_data",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    data = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    response_data = table.Column<string>(type: "jsonb", nullable: true),
                    meta_data = table.Column<string>(type: "jsonb", nullable: true),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modified_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_send_jobs_data", x => x.id);
                    table.ForeignKey(
                        name: "fk_send_jobs_data_send_jobs_id",
                        column: x => x.id,
                        principalTable: "send_jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_result_handling_jobs_send_job_id",
                table: "result_handling_jobs",
                column: "send_job_id");

            migrationBuilder.CreateIndex(
                name: "ix_result_handling_jobs_service_id_status_start_time",
                table: "result_handling_jobs",
                columns: new[] { "service_id", "status", "start_time" });

            migrationBuilder.CreateIndex(
                name: "ix_send_jobs_service_id_status_start_time_handle_order",
                table: "send_jobs",
                columns: new[] { "service_id", "status", "start_time", "handle_order" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "result_handling_jobs");

            migrationBuilder.DropTable(
                name: "send_jobs_data");

            migrationBuilder.DropTable(
                name: "send_jobs");
        }
    }
}
