import { Component } from "@angular/core";
import { TicketService } from "../../services/ticket.service";

@Component({
  selector: "app-chef",
  templateUrl: "./chef.component.html",
  styleUrls: ["./chef.component.css"],
})
export class ChefComponent {
  ticketNumber = "";
  validationResult: any = null;
  recentRedemptions: any[] = [];

  constructor(private ticketService: TicketService) {
    this.loadRecentRedemptions();
  }

  validateTicket(): void {
    if (!this.ticketNumber) {
      alert("Please enter a ticket number");
      return;
    }

    this.ticketService.validateTicket(this.ticketNumber).subscribe({
      next: (data) => {
        this.validationResult = data;
      },
      error: (err) => {
        this.validationResult = {
          isValid: false,
          message: err.error?.message || "Error validating ticket",
        };
      },
    });
  }

  redeemTicket(): void {
    if (!this.validationResult?.ticket) return;

    this.ticketService
      .redeemTicket({
        ticketNumber: this.validationResult.ticket.ticketNumber,
        verificationMethod: "Manual",
      })
      .subscribe({
        next: () => {
          alert("Ticket redeemed successfully!");
          this.validationResult = null;
          this.ticketNumber = "";
          this.loadRecentRedemptions();
        },
        error: (err) => {
          alert(err.error?.message || "Error redeeming ticket");
        },
      });
  }

  loadRecentRedemptions(): void {
    // You can implement an API call to fetch recent redemptions
    // For now, it's empty.
    this.recentRedemptions = [];
  }
}
