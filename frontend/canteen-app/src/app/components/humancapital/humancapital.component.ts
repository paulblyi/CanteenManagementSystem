import { Component, OnInit } from "@angular/core";
import { TicketService } from "../../services/ticket.service";
import { BatchService } from "../../services/batch.service";
import { MealTicket, TicketApproval } from "../../models/ticket.model";
import { Batch, BatchCreate } from "../../models/batch.model";
import { User } from "../../models/user.model";
import { ROLES } from "../../models/role.model";

@Component({
  selector: "app-humancapital",
  templateUrl: "./humancapital.component.html",
  styleUrls: ["./humancapital.component.css"],
})
export class HumanCapitalComponent implements OnInit {
  pendingTickets: MealTicket[] = [];
  batches: Batch[] = [];
  selectedBatch: Batch | null = null;
  showBatchForm = false;
  showTicketDetail = false;
  selectedTicket: MealTicket | null = null;
  loading = false;

  batchRequest: BatchCreate = {
    ticketDate: new Date(),
    mealType: "Lunch",
    employeeIds: [],
    department: "",
  };

  availableEmployees: User[] = [];
  selectedEmployees: User[] = [];

  today = new Date().toISOString().split("T")[0];

  constructor(
    private ticketService: TicketService,
    private batchService: BatchService,
  ) {}

  ngOnInit(): void {
    this.loadPendingTickets();
    this.loadBatches();
  }

  loadPendingTickets(): void {
    this.loading = true;
    this.ticketService.getPendingTickets().subscribe({
      next: (data: MealTicket[]) => {
        this.pendingTickets = data;
        this.loading = false;
      },
      error: (err: any) => {
        console.error("Error loading pending tickets:", err);
        this.loading = false;
      },
    });
  }

  loadBatches(): void {
    this.batchService.getBatches().subscribe({
      next: (data: Batch[]) => {
        this.batches = data;
      },
      error: (err: any) => {
        console.error("Error loading batches:", err);
      },
    });
  }

  approveTicket(ticketId: number, status: "Approved" | "Rejected"): void {
    const approval: TicketApproval = {
      ticketId,
      status,
      notes: status === "Rejected" ? "Rejected by HC" : "Approved",
    };

    this.ticketService.approveTicket(approval).subscribe({
      next: () => {
        this.loadPendingTickets();
        alert(`Ticket ${status.toLowerCase()} successfully!`);
      },
      error: (err: any) => {
        alert("Error processing ticket");
      },
    });
  }

  createBatch(): void {
    if (this.selectedEmployees.length === 0) {
      alert("Please select at least one employee");
      return;
    }

    this.batchRequest.employeeIds = this.selectedEmployees.map((e) => e.id);

    this.loading = true;
    this.batchService.createBatch(this.batchRequest).subscribe({
      next: (data: Batch) => {
        this.loading = false;
        this.showBatchForm = false;
        this.selectedEmployees = [];
        this.loadBatches();
        alert(
          `Batch created successfully! ${data.totalTickets} tickets generated.`,
        );
      },
      error: (err: any) => {
        this.loading = false;
        alert(err.error?.message || "Error creating batch");
      },
    });
  }

  cancelBatch(id: number): void {
    if (confirm("Are you sure you want to cancel this batch?")) {
      this.batchService.cancelBatch(id).subscribe({
        next: () => {
          this.loadBatches();
          alert("Batch cancelled successfully!");
        },
        error: (err: any) => {
          alert("Error cancelling batch");
        },
      });
    }
  }

  viewBatch(batch: Batch): void {
    this.selectedBatch = batch;
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
      case "Active":
        return "#17a2b8";
      case "Completed":
        return "#28a745";
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
      case "Active":
        return "🔄";
      case "Completed":
        return "✔️";
      default:
        return "📌";
    }
  }

  removeEmployee(employee: User): void {
    this.selectedEmployees = this.selectedEmployees.filter(
      (e) => e.id !== employee.id,
    );
  }

  addEmployeeById(id: string): void {
    const employee = this.getMockEmployees().find((e) => e.id === +id);
    if (employee && !this.selectedEmployees.find((e) => e.id === employee.id)) {
      this.selectedEmployees.push(employee);
    }
  }

  // Mock employees – replace with real API call
  getMockEmployees(): User[] {
    return [
      {
        id: 1,
        username: "john.doe",
        fullName: "John Doe",
        role: ROLES.EMPLOYEE,
        department: "IT",
        isActive: true,
        createdAt: new Date(),
      },
      {
        id: 2,
        username: "jane.smith",
        fullName: "Jane Smith",
        role: ROLES.EMPLOYEE,
        department: "HR",
        isActive: true,
        createdAt: new Date(),
      },
      {
        id: 3,
        username: "bob.johnson",
        fullName: "Bob Johnson",
        role: ROLES.EMPLOYEE,
        department: "Finance",
        isActive: true,
        createdAt: new Date(),
      },
      {
        id: 4,
        username: "alice.williams",
        fullName: "Alice Williams",
        role: ROLES.EMPLOYEE,
        department: "IT",
        isActive: true,
        createdAt: new Date(),
      },
      {
        id: 5,
        username: "charlie.brown",
        fullName: "Charlie Brown",
        role: ROLES.EMPLOYEE,
        department: "Operations",
        isActive: true,
        createdAt: new Date(),
      },
    ];
  }
}
