import { Component, OnInit } from '@angular/core';
import { ReconciliationService } from '../../services/reconciliation.service';
import { Reconciliation, VarianceDetail } from '../../models/reconciliation.model';

@Component({
  selector: 'app-reconciliation',
  template: `
    <div class="reconciliation-portal">
      <h2>Reconciliation Engine</h2>
      
      <div class="actions-bar">
        <div class="date-picker">
          <label>Select Date:</label>
          <input type="date" [(ngModel)]="selectedDate" [value]="today">
          <button class="btn btn-primary" (click)="reconcileDate()">Run Reconciliation</button>
        </div>
      </div>

      <div *ngIf="loading" class="loading">Processing...</div>

      <div *ngIf="currentReconciliation" class="reconciliation-summary">
        <h3>Reconciliation Summary - {{ currentReconciliation.reconciliationDate | date:'fullDate' }}</h3>
        
        <div class="summary-grid">
          <div class="summary-item">
            <div class="label">Total Issued</div>
            <div class="value">{{ currentReconciliation.totalTicketsIssued }}</div>
          </div>
          <div class="summary-item">
            <div class="label">Total Redeemed</div>
            <div class="value">{{ currentReconciliation.totalTicketsRedeemed }}</div>
          </div>
          <div class="summary-item">
            <div class="label">Pending</div>
            <div class="value">{{ currentReconciliation.totalTicketsPending }}</div>
          </div>
          <div class="summary-item" [class.has-variance]="currentReconciliation.variance > 0">
            <div class="label">Variance</div>
            <div class="value">{{ currentReconciliation.variance }}</div>
          </div>
        </div>

        <div class="meal-breakdown">
          <h4>Meal Type Breakdown</h4>
          <table class="breakdown-table">
            <thead>
              <tr>
                <th>Meal Type</th>
                <th>Issued</th>
                <th>Redeemed</th>
                <th>Pending</th>
                <th>Variance</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>Breakfast</td>
                <td>{{ currentReconciliation.breakfast.issued }}</td>
                <td>{{ currentReconciliation.breakfast.redeemed }}</td>
                <td>{{ currentReconciliation.breakfast.pending }}</td>
                <td [class.has-variance]="currentReconciliation.breakfast.variance > 0">
                  {{ currentReconciliation.breakfast.variance }}
                </td>
              </tr>
              <tr>
                <td>Lunch</td>
                <td>{{ currentReconciliation.lunch.issued }}</td>
                <td>{{ currentReconciliation.lunch.redeemed }}</td>
                <td>{{ currentReconciliation.lunch.pending }}</td>
                <td [class.has-variance]="currentReconciliation.lunch.variance > 0">
                  {{ currentReconciliation.lunch.variance }}
                </td>
              </tr>
              <tr>
                <td>Dinner</td>
                <td>{{ currentReconciliation.dinner.issued }}</td>
                <td>{{ currentReconciliation.dinner.redeemed }}</td>
                <td>{{ currentReconciliation.dinner.pending }}</td>
                <td [class.has-variance]="currentReconciliation.dinner.variance > 0">
                  {{ currentReconciliation.dinner.variance }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <div *ngIf="currentReconciliation.variance > 0" class="variance-details">
          <h4>Variance Details</h4>
          <button class="btn btn-info" (click)="loadVarianceDetails()">View Variance Details</button>
          
          <div *ngIf="varianceDetails.length > 0">
            <table class="variance-table">
              <thead>
                <tr>
                  <th>Ticket Number</th>
                  <th>Employee</th>
                  <th>Department</th>
                  <th>Meal Type</th>
                  <th>Variance Type</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let detail of varianceDetails">
                  <td>{{ detail.ticketNumber }}</td>
                  <td>{{ detail.employeeName }}</td>
                  <td>{{ detail.department }}</td>
                  <td>{{ detail.mealType }}</td>
                  <td><span class="variance-type">{{ detail.varianceType }}</span></td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>

        <div class="reconciliation-actions" *ngIf="currentReconciliation.status === 'Pending'">
          <button class="btn btn-success" (click)="completeReconciliation()">
            Complete Reconciliation
          </button>
          <button class="btn btn-secondary" (click)="autoReconcile()">
            Auto-Reconcile
          </button>
        </div>

        <div *ngIf="currentReconciliation.status === 'Completed'" class="completed-status">
          ✅ Reconciliation Completed 
          <span *ngIf="currentReconciliation.reconciledBy">
            by {{ currentReconciliation.reconciledBy }}
            at {{ currentReconciliation.reconciledAt | date:'medium' }}
          </span>
        </div>
      </div>

      <div class="reconciliation-history">
        <h3>Reconciliation History</h3>
        <table class="history-table">
          <thead>
            <tr>
              <th>Date</th>
              <th>Issued</th>
              <th>Redeemed</th>
              <th>Variance</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let recon of reconciliations">
              <td>{{ recon.reconciliationDate | date }}</td>
              <td>{{ recon.totalTicketsIssued }}</td>
              <td>{{ recon.totalTicketsRedeemed }}</td>
              <td [class.has-variance]="recon.variance > 0">{{ recon.variance }}</td>
              <td>
                <span class="status-badge" [class.completed]="recon.status === 'Completed'"
                      [class.pending]="recon.status === 'Pending'"
                      [class.variance]="recon.status === 'Variance'">
                  {{ recon.status }}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-info" (click)="viewReconciliation(recon.id)">View</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .reconciliation-portal { padding: 20px; max-width: 1200px; margin: 0 auto; }
    h2 { color: #333; margin-bottom: 30px; }
    .actions-bar { background: white; padding: 20px; border-radius: 10px; margin-bottom: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .date-picker { display: flex; align-items: center; gap: 15px; flex-wrap: wrap; }
    .date-picker label { font-weight: 500; }
    .date-picker input { padding: 8px 12px; border: 1px solid #ddd; border-radius: 6px; }
    .btn { padding: 8px 16px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-primary { background: #667eea; color: white; }
    .btn-primary:hover { background: #5a67d8; }
    .btn-success { background: #28a745; color: white; }
    .btn-success:hover { background: #218838; }
    .btn-secondary { background: #6c757d; color: white; }
    .btn-secondary:hover { background: #5a6268; }
    .btn-info { background: #17a2b8; color: white; }
    .btn-info:hover { background: #138496; }
    .btn-sm { padding: 4px 10px; font-size: 12px; }
    .loading { text-align: center; padding: 40px; color: #999; }
    .reconciliation-summary { background: white; padding: 20px; border-radius: 10px; margin-bottom: 30px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .summary-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 15px; margin: 20px 0; }
    .summary-item { text-align: center; padding: 15px; background: #f8f9fa; border-radius: 8px; }
    .summary-item .label { color: #888; font-size: 13px; }
    .summary-item .value { font-size: 24px; font-weight: bold; color: #333; }
    .summary-item.has-variance .value { color: #dc3545; }
    .meal-breakdown { margin: 20px 0; }
    .breakdown-table, .variance-table, .history-table { width: 100%; border-collapse: collapse; margin: 10px 0; }
    .breakdown-table th, .variance-table th, .history-table th { background: #f8f9fa; padding: 10px; text-align: left; font-weight: 600; border-bottom: 2px solid #dee2e6; }
    .breakdown-table td, .variance-table td, .history-table td { padding: 10px; border-bottom: 1px solid #eee; }
    .breakdown-table td.has-variance { color: #dc3545; font-weight: bold; }
    .variance-type { display: inline-block; padding: 2px 8px; border-radius: 12px; font-size: 12px; }
    .variance-type:contains('Missing') { background: #f8d7da; color: #721c24; }
    .variance-type:contains('Extra') { background: #fff3cd; color: #856404; }
    .variance-type:contains('Duplicate') { background: #d1ecf1; color: #0c5460; }
    .reconciliation-actions { display: flex; gap: 10px; margin-top: 20px; }
    .completed-status { padding: 10px; background: #d4edda; border-radius: 6px; color: #155724; }
    .status-badge { display: inline-block; padding: 2px 10px; border-radius: 12px; font-size: 12px; }
    .status-badge.completed { background: #d4edda; color: #155724; }
    .status-badge.pending { background: #fff3cd; color: #856404; }
    .status-badge.variance { background: #f8d7da; color: #721c24; }
    .reconciliation-history { background: white; padding: 20px; border-radius: 10px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .history-table td.has-variance { color: #dc3545; font-weight: bold; }
  `]
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
    return new Date().toISOString().split('T')[0];
  }

  reconcileDate(): void {
    this.loading = true;
    this.reconciliationService.reconcileDate(this.selectedDate).subscribe({
      next: (data) => {
        this.currentReconciliation = data;
        this.loading = false;
        this.loadReconciliations();
      },
      error: (err) => {
        this.loading = false;
        alert(err.error?.message || 'Error running reconciliation');
      }
    });
  }

  completeReconciliation(): void {
    if (!this.currentReconciliation) return;
    
    this.loading = true;
    this.reconciliationService.completeReconciliation(
      this.currentReconciliation.id,
      'Completed by user'
    ).subscribe({
      next: (data) => {
        this.currentReconciliation = data;
        this.loading = false;
        this.loadReconciliations();
        alert('Reconciliation completed successfully!');
      },
      error: (err) => {
        this.loading = false;
        alert(err.error?.message || 'Error completing reconciliation');
      }
    });
  }

  autoReconcile(): void {
    if (!this.currentReconciliation) return;

    this.loading = true;
    this.reconciliationService.autoReconcile(this.currentReconciliation.id).subscribe({
      next: (data) => {
        this.currentReconciliation = data;
        this.loading = false;
        this.loadReconciliations();
        alert('Auto-reconciliation completed!');
      },
      error: (err) => {
        this.loading = false;
        alert(err.error?.message || 'Error auto-reconciling');
      }
    });
  }

  loadReconciliations(): void {
    this.reconciliationService.getReconciliations().subscribe({
      next: (data) => {
        this.reconciliations = data;
      },
      error: (err) => {
        console.error('Error loading reconciliations:', err);
      }
    });
  }

  viewReconciliation(id: number): void {
    this.reconciliationService.getReconciliation(id).subscribe({
      next: (data) => {
        this.currentReconciliation = data;
        this.varianceDetails = [];
      },
      error: (err) => {
        alert('Error loading reconciliation details');
      }
    });
  }

  loadVarianceDetails(): void {
    if (!this.currentReconciliation) return;

    this.reconciliationService.getVarianceDetails(this.currentReconciliation.id).subscribe({
      next: (data) => {
        this.varianceDetails = data;
      },
      error: (err) => {
        alert('Error loading variance details');
      }
    });
  }
}
