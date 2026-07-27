import { Component, OnInit } from "@angular/core";
import { ReconciliationService } from "../../services/reconciliation.service";
import {
  Reconciliation,
  VarianceDetail,
} from "../../models/reconciliation.model";

@Component({
  selector: "app-reconciliation",
  templateUrl: "./reconciliation.component.html",
  styleUrls: ["./reconciliation.component.css"],
})
export class ReconciliationComponent implements OnInit {
  selectedDate = new Date();
  currentReconciliation: Reconciliation | null = null;
  reconciliations: Reconciliation[] = [];
  varianceDetails: VarianceDetail[] = [];
  loading = false;

  constructor(private reconciliationService: ReconciliationService) {}

  ngOnInit(): void {
    this.loadReconciliations();
  }

  get today(): string {
    return new Date().toISOString().split("T")[0];
  }

  reconcileDate(): void {
    this.loading = true;
    this.reconciliationService.reconcileDate(this.selectedDate).subscribe({
      next: (data: Reconciliation) => {
        this.currentReconciliation = data;
        this.loading = false;
        this.loadReconciliations();
      },
      error: (err: any) => {
        this.loading = false;
        alert(err.error?.message || "Error running reconciliation");
      },
    });
  }

  completeReconciliation(): void {
    if (!this.currentReconciliation) return;

    this.loading = true;
    this.reconciliationService
      .completeReconciliation(
        this.currentReconciliation.id,
        "Completed by user",
      )
      .subscribe({
        next: (data: Reconciliation) => {
          this.currentReconciliation = data;
          this.loading = false;
          this.loadReconciliations();
          alert("Reconciliation completed successfully!");
        },
        error: (err: any) => {
          this.loading = false;
          alert(err.error?.message || "Error completing reconciliation");
        },
      });
  }

  autoReconcile(): void {
    if (!this.currentReconciliation) return;

    this.loading = true;
    this.reconciliationService
      .autoReconcile(this.currentReconciliation.id)
      .subscribe({
        next: (data: Reconciliation) => {
          this.currentReconciliation = data;
          this.loading = false;
          this.loadReconciliations();
          alert("Auto-reconciliation completed!");
        },
        error: (err: any) => {
          this.loading = false;
          alert(err.error?.message || "Error auto-reconciling");
        },
      });
  }

  loadReconciliations(): void {
    this.reconciliationService.getReconciliations().subscribe({
      next: (data: Reconciliation[]) => {
        this.reconciliations = data;
      },
      error: (err: any) => {
        console.error("Error loading reconciliations:", err);
      },
    });
  }

  viewReconciliation(id: number): void {
    this.reconciliationService.getReconciliation(id).subscribe({
      next: (data: Reconciliation) => {
        this.currentReconciliation = data;
        this.varianceDetails = [];
      },
      error: (err: any) => {
        alert("Error loading reconciliation details");
      },
    });
  }

  loadVarianceDetails(): void {
    if (!this.currentReconciliation) return;

    this.reconciliationService
      .getVarianceDetails(this.currentReconciliation.id)
      .subscribe({
        next: (data: VarianceDetail[]) => {
          this.varianceDetails = data;
        },
        error: (err: any) => {
          alert("Error loading variance details");
        },
      });
  }
}
