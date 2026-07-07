import { Component, OnInit } from "@angular/core";
import { TicketService } from "../../services/ticket.service";
import { MealTicket, TicketRequest } from "../../models/ticket.model";

@Component({
  selector: "app-employee",
  templateUrl: "./employee.component.html",
  styleUrls: ["./employee.component.css"],
})
export class EmployeeComponent implements OnInit {
  tickets: MealTicket[] = [];
  selectedTicket: MealTicket | null = null;
  showRequestForm = false;
  showTicketDetail = false;
  loading = false;
  today = new Date().toISOString().split("T")[0]; // ← ADD THIS

  // Request form
  request: TicketRequest = {
    ticketDate: new Date(),
    mealType: "Lunch",
    notes: "",
  };

  constructor(private ticketService: TicketService) {}

  ngOnInit(): void {
    this.loadMyTickets();
  }

  loadMyTickets(): void {
    this.loading = true;
    this.ticketService.getMyTickets().subscribe({
      next: (data) => {
        this.tickets = data;
        this.loading = false;
      },
      error: (err) => {
        console.error("Error loading tickets:", err);
        this.loading = false;
      },
    });
  }

  requestTicket(): void {
    this.loading = true;
    this.ticketService.requestTicket(this.request).subscribe({
      next: () => {
        this.showRequestForm = false;
        this.loadMyTickets();
        alert("Ticket requested successfully!");
      },
      error: (err) => {
        this.loading = false;
        alert(err.error?.message || "Error requesting ticket");
      },
    });
  }

  cancelTicket(id: number): void {
    if (confirm("Are you sure you want to cancel this ticket?")) {
      this.ticketService.cancelTicket(id).subscribe({
        next: () => {
          this.loadMyTickets();
        },
        error: (err) => {
          alert("Error cancelling ticket");
        },
      });
    }
  }

  viewTicket(ticket: MealTicket): void {
    this.selectedTicket = ticket;
    this.showTicketDetail = true;
  }

  getStatusColor(status: string): string {
    switch (status) {
      case "Pending":
        return "#ffc107";
      case "Approved":
        return "#28a745";
      case "Redeemed":
        return "#007bff";
      case "Cancelled":
        return "#dc3545";
      default:
        return "#6c757d";
    }
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case "Pending":
        return "⏳";
      case "Approved":
        return "✅";
      case "Redeemed":
        return "🍽️";
      case "Cancelled":
        return "❌";
      default:
        return "📌";
    }
  }

  canCancel(ticket: MealTicket): boolean {
    return ticket.status === "Pending" || ticket.status === "Approved";
  }
}
