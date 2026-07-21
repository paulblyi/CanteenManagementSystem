import { Component } from "@angular/core";
import { TicketService } from "../../services/ticket.service";
import { RedemptionLog } from "../../models/redemption.model";
@Component({
  selector: "app-chef",
  templateUrl: "./chef.component.html",
  styleUrls: ["./chef.component.css"],
})
export class ChefComponent {
  ticketNumber = "";
  validationResult: any = null;
  recentRedemptions: RedemptionLog[] = [];

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
    this.ticketService.getRecentRedemptions().subscribe({
      next: (data) => {
        this.recentRedemptions = data;
      },
      error: (err) => {
        console.error("Error loading recent redemptions:", err);
      },
    });
  }
}
