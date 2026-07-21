import { Component, OnInit } from "@angular/core";
import { ReportService } from "../../services/report.service";
import { DailyReport, DepartmentReport } from "../../models/report.model";

@Component({
  selector: "app-reports",
  templateUrl: "./reports.component.html",
  styleUrls: ["./reports.component.css"],
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
