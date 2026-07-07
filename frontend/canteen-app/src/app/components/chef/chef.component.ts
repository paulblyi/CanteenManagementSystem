import { Component } from '@angular/core';
import { TicketService } from '../../services/ticket.service';

@Component({
  selector: 'app-chef',
  template: `
    <div class="chef-portal">
      <h2>Chef Portal - Meal Redemption</h2>
      
      <div class="validation-section">
        <h3>Validate Ticket</h3>
        <div class="validation-form">
          <div class="form-group">
            <label>Enter Ticket Number</label>
            <input type="text" [(ngModel)]="ticketNumber" 
                   placeholder="e.g., TKT-20260701-ABC123"
                   (keyup.enter)="validateTicket()">
          </div>
          <button class="btn btn-primary" (click)="validateTicket()">Validate</button>
        </div>
        
        <div *ngIf="validationResult" class="validation-result">
          <div [class.valid]="validationResult.isValid" [class.invalid]="!validationResult.isValid">
            <h4>{{ validationResult.isValid ? '✅ Valid Ticket' : '❌ Invalid Ticket' }}</h4>
            <p>{{ validationResult.message }}</p>
            <div *ngIf="validationResult.ticket" class="ticket-info">
              <div><strong>Employee:</strong> {{ validationResult.ticket.employeeName }}</div>
              <div><strong>Meal:</strong> {{ validationResult.ticket.mealType }}</div>
              <div><strong>Department:</strong> {{ validationResult.ticket.department }}</div>
            </div>
            <button *ngIf="validationResult.isValid" 
                    class="btn btn-success mt-20"
                    (click)="redeemTicket()">
              Redeem Ticket
            </button>
          </div>
        </div>
      </div>

      <div class="recent-redemptions">
        <h3>Recent Redemptions</h3>
        <div *ngIf="recentRedemptions.length === 0" class="empty-state">
          No recent redemptions
        </div>
        <div *ngFor="let redemption of recentRedemptions" class="redemption-item">
          <span class="ticket-number">{{ redemption.ticketNumber }}</span>
          <span class="employee">{{ redemption.employeeName }}</span>
          <span class="time">{{ redemption.redeemedAt | date:'short' }}</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .chef-portal { padding: 20px; max-width: 800px; margin: 0 auto; }
    .validation-section { background: white; padding: 20px; border-radius: 10px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .validation-form { display: flex; gap: 10px; margin-top: 15px; }
    .validation-form .form-group { flex: 1; }
    .validation-form input { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 6px; }
    .btn { padding: 10px 20px; border: none; border-radius: 6px; cursor: pointer; }
    .btn-primary { background: #667eea; color: white; }
    .btn-success { background: #28a745; color: white; }
    .btn-success:hover { background: #218838; }
    .validation-result { margin-top: 20px; }
    .validation-result .valid { padding: 15px; background: #d4edda; border-radius: 6px; border: 1px solid #c3e6cb; }
    .validation-result .invalid { padding: 15px; background: #f8d7da; border-radius: 6px; border: 1px solid #f5c6cb; }
    .ticket-info { margin-top: 10px; padding: 10px; background: #f8f9fa; border-radius: 4px; }
    .ticket-info div { padding: 4px 0; }
    .mt-20 { margin-top: 20px; }
    .recent-redemptions { background: white; padding: 20px; border-radius: 10px; margin-top: 30px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); }
    .redemption-item { display: flex; gap: 20px; padding: 10px 0; border-bottom: 1px solid #eee; }
    .redemption-item:last-child { border-bottom: none; }
    .ticket-number { font-weight: bold; color: #333; }
    .employee { color: #666; }
    .time { color: #999; font-size: 12px; }
    .empty-state { text-align: center; padding: 20px; color: #999; }
  `]
})
export class ChefComponent {
  ticketNumber = '';
  validationResult: any = null;
  recentRedemptions: any[] = [];

  constructor(private ticketService: TicketService) {
    // Load recent redemptions
    this.loadRecentRedemptions();
  }

  validateTicket(): void {
    if (!this.ticketNumber) {
      alert('Please enter a ticket number');
      return;
    }

    this.ticketService.validateTicket(this.ticketNumber).subscribe({
      next: (data) => {
        this.validationResult = data;
      },
      error: (err) => {
        this.validationResult = {
          isValid: false,
          message: err.error?.message || 'Error validating ticket'
        };
      }
    });
  }

  redeemTicket(): void {
    if (!this.validationResult?.ticket) return;

    this.ticketService.redeemTicket({
      ticketNumber: this.validationResult.ticket.ticketNumber,
      verificationMethod: 'Manual'
    }).subscribe({
      next: () => {
        alert('Ticket redeemed successfully!');
        this.validationResult = null;
        this.ticketNumber = '';
        this.loadRecentRedemptions();
      },
      error: (err) => {
        alert(err.error?.message || 'Error redeeming ticket');
      }
    });
  }

  loadRecentRedemptions(): void {
    // In real implementation, fetch from API
    this.recentRedemptions = [];
  }
}
