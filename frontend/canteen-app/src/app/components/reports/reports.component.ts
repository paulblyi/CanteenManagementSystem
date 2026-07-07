import { Component, OnInit } from "@angular/core";
import { ReportService } from "../../services/report.service";
import { DailyReport, DepartmentReport } from "../../models/report.model";

@Component({
  selector: "app-reports",
  template: `
    <div class="reports-portal">
      <h2>Reports & Dashboard</h2>

      <div class="report-filters">
        <div class="filter-group">
          <label>Report Type</label>
          <select [(ngModel)]="reportType" (change)="loadReport()">
            <option value="daily">Daily Report</option>
            <option value="monthly">Monthly Report</option>
            <option value="department">Department Report</option>
          </select>
        </div>

        <div class="filter-group" *ngIf="reportType === 'daily'">
          <label>Date</label>
          <input
            type="date"
            [(ngModel)]="selectedDate"
            (change)="loadReport()"
          />
        </div>

        <div class="filter-group" *ngIf="reportType === 'monthly'">
          <label>Month</label>
          <input
            type="month"
            [(ngModel)]="selectedMonth"
            (change)="loadReport()"
          />
        </div>

        <div class="filter-group" *ngIf="reportType === 'department'">
          <label>Start Date</label>
          <input type="date" [(ngModel)]="startDate" (change)="loadReport()" />
          <label>End Date</label>
          <input type="date" [(ngModel)]="endDate" (change)="loadReport()" />
        </div>

        <button class="btn btn-success" (click)="exportReport()">
          📊 Export Report
        </button>
      </div>

      <div *ngIf="loading" class="loading">Loading report...</div>

      <!-- Daily Report -->
      <div *ngIf="reportType === 'daily' && dailyReport" class="report-content">
        <h3>Daily Report - {{ dailyReport.date | date: "fullDate" }}</h3>

        <div class="report-stats">
          <div class="stat-box">
            <div class="stat-label">Total Issued</div>
            <div class="stat-value">{{ dailyReport.totalTicketsIssued }}</div>
          </div>
          <div class="stat-box">
            <div class="stat-label">Total Redeemed</div>
            <div class="stat-value">{{ dailyReport.totalTicketsRedeemed }}</div>
          </div>
          <div class="stat-box">
            <div class="stat-label">Total Revenue</div>
            <div class="stat-value">
              {{ dailyReport.totalRevenue.toFixed(2) }}
            </div>
          </div>
        </div>

        <div class="meal-breakdown">
          <h4>Meal Type Breakdown</h4>
          <table class="report-table">
            <thead>
              <tr>
                <th>Meal Type</th>
                <th>Issued</th>
                <th>Redeemed</th>
                <th>Pending</th>
                <th>Revenue</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>Breakfast</td>
                <td>{{ dailyReport.breakfastIssued }}</td>
                <td>{{ dailyReport.breakfastRedeemed }}</td>
                <td>{{ dailyReport.breakfastPending }}</td>
                <td>{{ dailyReport.breakfastRevenue.toFixed(2) }}</td>
              </tr>
              <tr>
                <td>Lunch</td>
                <td>{{ dailyReport.lunchIssued }}</td>
                <td>{{ dailyReport.lunchRedeemed }}</td>
                <td>{{ dailyReport.lunchPending }}</td>
                <td>{{ dailyReport.lunchRevenue.toFixed(2) }}</td>
              </tr>
              <tr>
                <td>Dinner</td>
                <td>{{ dailyReport.dinnerIssued }}</td>
                <td>{{ dailyReport.dinnerRedeemed }}</td>
                <td>{{ dailyReport.dinnerPending }}</td>
                <td>{{ dailyReport.dinnerRevenue.toFixed(2) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div
          *ngIf="dailyReport.departmentBreakdown.length > 0"
          class="department-breakdown"
        >
          <h4>Department Breakdown</h4>
          <table class="report-table">
            <thead>
              <tr>
                <th>Department</th>
                <th>Employees</th>
                <th>Issued</th>
                <th>Redeemed</th>
                <th>Cost</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let dept of dailyReport.departmentBreakdown">
                <td>{{ dept.department }}</td>
                <td>{{ dept.totalEmployees }}</td>
                <td>{{ dept.ticketsIssued }}</td>
                <td>{{ dept.ticketsRedeemed }}</td>
                <td>{{ dept.totalCost.toFixed(2) }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div *ngIf="dailyReport.topEmployees.length > 0" class="top-employees">
          <h4>Top Employees</h4>
          <table class="report-table">
            <thead>
              <tr>
                <th>Employee</th>
                <th>Department</th>
                <th>Meals</th>
                <th>Favorite Meal</th>
                <th>Total Cost</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let emp of dailyReport.topEmployees">
                <td>{{ emp.employeeName }}</td>
                <td>{{ emp.department }}</td>
                <td>{{ emp.totalTickets }}</td>
                <td>{{ emp.favoriteMeal }}</td>
                <td>{{ emp.totalCost.toFixed(2) }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      <!-- Department Report -->
      <div
        *ngIf="reportType === 'department' && departmentReport"
        class="report-content"
      >
        <h3>Department Report</h3>
        <table class="report-table">
          <thead>
            <tr>
              <th>Department</th>
              <th>Employees</th>
              <th>Issued</th>
              <th>Redeemed</th>
              <th>Total Cost</th>
              <th>Cost/Employee</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let dept of departmentReport">
              <td>{{ dept.department }}</td>
              <td>{{ dept.totalEmployees }}</td>
              <td>{{ dept.ticketsIssued }}</td>
              <td>{{ dept.ticketsRedeemed }}</td>
              <td>{{ dept.totalCost.toFixed(2) }}</td>
              <td>{{ dept.costPerEmployee.toFixed(2) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [
    `
      .reports-portal {
        padding: 20px;
        max-width: 1200px;
        margin: 0 auto;
      }
      h2 {
        color: #333;
        margin-bottom: 30px;
      }
      .report-filters {
        background: white;
        padding: 20px;
        border-radius: 10px;
        margin-bottom: 20px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        display: flex;
        flex-wrap: wrap;
        gap: 20px;
        align-items: end;
      }
      .filter-group {
        display: flex;
        flex-direction: column;
        gap: 5px;
      }
      .filter-group label {
        font-weight: 500;
        font-size: 13px;
        color: #555;
      }
      .filter-group select,
      .filter-group input {
        padding: 8px 12px;
        border: 1px solid #ddd;
        border-radius: 6px;
      }
      .btn {
        padding: 8px 16px;
        border: none;
        border-radius: 6px;
        cursor: pointer;
      }
      .btn-success {
        background: #28a745;
        color: white;
      }
      .btn-success:hover {
        background: #218838;
      }
      .loading {
        text-align: center;
        padding: 40px;
        color: #999;
      }
      .report-content {
        background: white;
        padding: 20px;
        border-radius: 10px;
        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      }
      .report-stats {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
        gap: 15px;
        margin: 20px 0;
      }
      .stat-box {
        text-align: center;
        padding: 15px;
        background: #f8f9fa;
        border-radius: 8px;
      }
      .stat-box .stat-label {
        color: #888;
        font-size: 13px;
      }
      .stat-box .stat-value {
        font-size: 24px;
        font-weight: bold;
        color: #333;
      }
      .report-table {
        width: 100%;
        border-collapse: collapse;
        margin: 10px 0;
      }
      .report-table th {
        background: #f8f9fa;
        padding: 10px;
        text-align: left;
        font-weight: 600;
        border-bottom: 2px solid #dee2e6;
      }
      .report-table td {
        padding: 10px;
        border-bottom: 1px solid #eee;
      }
      .meal-breakdown,
      .department-breakdown,
      .top-employees {
        margin: 20px 0;
      }
      h4 {
        color: #555;
        margin-bottom: 10px;
      }
    `,
  ],
})
export class ReportsComponent implements OnInit {
  reportType = "daily";
  selectedDate = new Date();
  selectedMonth = new Date().toISOString().slice(0, 7);
  startDate = new Date(new Date().setDate(1));
  endDate = new Date();
  loading = false;

  dailyReport: DailyReport | null = null;
  departmentReport: DepartmentReport[] = [];

  constructor(private reportService: ReportService) {}

  ngOnInit(): void {
    this.loadReport();
  }

  loadReport(): void {
    this.loading = true;

    if (this.reportType === "daily") {
      this.reportService.getDailyReport(this.selectedDate).subscribe({
        next: (data) => {
          this.dailyReport = data;
          this.loading = false;
        },
        error: (err) => {
          console.error("Error loading daily report:", err);
          this.loading = false;
        },
      });
    } else if (this.reportType === "monthly") {
      const [year, month] = this.selectedMonth.split("-").map(Number);
      this.reportService.getMonthlyReport(year, month).subscribe({
        next: (data) => {
          this.dailyReport = data;
          this.loading = false;
        },
        error: (err) => {
          console.error("Error loading monthly report:", err);
          this.loading = false;
        },
      });
    } else if (this.reportType === "department") {
      this.reportService
        .getDepartmentReport(this.startDate, this.endDate)
        .subscribe({
          next: (data) => {
            this.departmentReport = data;
            this.loading = false;
          },
          error: (err) => {
            console.error("Error loading department report:", err);
            this.loading = false;
          },
        });
    }
  }

  exportReport(): void {
    this.reportService.exportReport(this.startDate, this.endDate).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = `report_${this.startDate.toISOString().split("T")[0]}_${this.endDate.toISOString().split("T")[0]}.csv`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        alert("Error exporting report");
      },
    });
  }
}
