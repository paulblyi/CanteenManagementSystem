using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CanteenApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailySummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SummaryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalMealsServed = table.Column<int>(type: "integer", nullable: false),
                    BreakfastCount = table.Column<int>(type: "integer", nullable: false),
                    LunchCount = table.Column<int>(type: "integer", nullable: false),
                    DinnerCount = table.Column<int>(type: "integer", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "numeric", nullable: false),
                    BreakfastRevenue = table.Column<decimal>(type: "numeric", nullable: false),
                    LunchRevenue = table.Column<decimal>(type: "numeric", nullable: false),
                    DinnerRevenue = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalEmployees = table.Column<int>(type: "integer", nullable: false),
                    ActiveEmployees = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EmployeeCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BatchNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<int>(type: "integer", nullable: false),
                    Department = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TicketDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MealType = table.Column<string>(type: "text", nullable: false),
                    TotalTickets = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batches_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BillingRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InvoiceNumber = table.Column<string>(type: "text", nullable: false),
                    BillingPeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BillingPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DepartmentId = table.Column<int>(type: "integer", nullable: true),
                    DepartmentName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TotalTickets = table.Column<int>(type: "integer", nullable: false),
                    BreakfastCount = table.Column<int>(type: "integer", nullable: false),
                    LunchCount = table.Column<int>(type: "integer", nullable: false),
                    DinnerCount = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BreakfastAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    LunchAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DinnerAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillingRecords_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reconciliations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReconciliationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalTicketsIssued = table.Column<int>(type: "integer", nullable: false),
                    TotalTicketsRedeemed = table.Column<int>(type: "integer", nullable: false),
                    TotalTicketsPending = table.Column<int>(type: "integer", nullable: false),
                    Variance = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ReconciledBy = table.Column<int>(type: "integer", nullable: true),
                    ReconciledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    BreakfastIssued = table.Column<int>(type: "integer", nullable: false),
                    BreakfastRedeemed = table.Column<int>(type: "integer", nullable: false),
                    BreakfastVariance = table.Column<int>(type: "integer", nullable: false),
                    LunchIssued = table.Column<int>(type: "integer", nullable: false),
                    LunchRedeemed = table.Column<int>(type: "integer", nullable: false),
                    LunchVariance = table.Column<int>(type: "integer", nullable: false),
                    DinnerIssued = table.Column<int>(type: "integer", nullable: false),
                    DinnerRedeemed = table.Column<int>(type: "integer", nullable: false),
                    DinnerVariance = table.Column<int>(type: "integer", nullable: false),
                    DepartmentBreakdown = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reconciliations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reconciliations_Users_ReconciledBy",
                        column: x => x.ReconciledBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RedemptionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: true),
                    ChefId = table.Column<int>(type: "integer", nullable: true),
                    RedemptionTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VerificationMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedemptionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RedemptionLogs_Users_ChefId",
                        column: x => x.ChefId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RedemptionLogs_Users_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MealTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TicketNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: true),
                    EmployeeName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Department = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TicketDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MealType = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedBy = table.Column<int>(type: "integer", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RedeemedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    QRCode = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    BatchId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealTickets_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MealTickets_Users_ApprovedBy",
                        column: x => x.ApprovedBy,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MealTickets_Users_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "EmployeeCode", "FullName", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2026, 7, 1, 9, 22, 18, 667, DateTimeKind.Utc).AddTicks(2107), "IT", "admin@canteen.com", "ADMIN001", "System Administrator", true, "QWRtaW5AMTIz", "Admin", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_BatchNumber",
                table: "Batches",
                column: "BatchNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batches_CreatedBy",
                table: "Batches",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BillingRecords_CreatedBy",
                table: "BillingRecords",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealTickets_ApprovedBy",
                table: "MealTickets",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealTickets_BatchId",
                table: "MealTickets",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_MealTickets_EmployeeId",
                table: "MealTickets",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MealTickets_TicketNumber",
                table: "MealTickets",
                column: "TicketNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reconciliations_ReconciledBy",
                table: "Reconciliations",
                column: "ReconciledBy");

            migrationBuilder.CreateIndex(
                name: "IX_RedemptionLogs_ChefId",
                table: "RedemptionLogs",
                column: "ChefId");

            migrationBuilder.CreateIndex(
                name: "IX_RedemptionLogs_EmployeeId",
                table: "RedemptionLogs",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeCode",
                table: "Users",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillingRecords");

            migrationBuilder.DropTable(
                name: "DailySummaries");

            migrationBuilder.DropTable(
                name: "MealTickets");

            migrationBuilder.DropTable(
                name: "Reconciliations");

            migrationBuilder.DropTable(
                name: "RedemptionLogs");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
