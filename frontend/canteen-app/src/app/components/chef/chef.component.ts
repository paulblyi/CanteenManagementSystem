import { Component, OnInit } from '@angular/core';
import { TicketService } from '../../services/ticket.service';
import { MealTicket } from '../../models/ticket.model';

@Component({
  selector: 'app-chef',
  templateUrl: './chef.component.html',
  styleUrls: ['./chef.component.css']
})
export class ChefComponent implements OnInit {
  ticketNumber = '';
  validationResult: any = null;
  recentRedemptions: any[] = [];
  approvedTickets: MealTicket[] = [];
  filteredTickets: MealTicket[] = [];
  searchTerm = '';
  selectedDate = new Date();
  loading = false;

  constructor(private ticketService: TicketService) {}

  ngOnInit(): void {
    this.loadApprovedTickets();
  }

  // ---------- Approved Tickets List ----------
  loadApprovedTickets(): void {
    this.loading = true;
    this.ticketService.getApprovedTickets(this.selectedDate).subscribe({
      next: (data: MealTicket[]) => {
        this.approvedTickets = data;
        this.filteredTickets = data;
        this.loading = false;
      },
      error: (err: any) => {
        console.error('Error loading approved tickets:', err);
        this.loading = false;
      }
    });
  }

  filterTickets(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredTickets = this.approvedTickets;
      return;
    }
    this.filteredTickets = this.approvedTickets.filter(t =>
      t.ticketNumber.toLowerCase().includes(term) ||
      t.employeeName.toLowerCase().includes(term) ||
      (t.departmentName && t.departmentName.toLowerCase().includes(term))
    );
  }

  selectTicket(ticket: MealTicket): void {
    this.ticketNumber = ticket.ticketNumber;
    this.validationResult = null;
    // Optional: automatically validate
    // this.validateTicket();
  }

  // ---------- Validation ----------
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
        this.loadApprovedTickets();  // refresh the list
      },
      error: (err) => {
        alert(err.error?.message || 'Error redeeming ticket');
      }
    });
  }
}