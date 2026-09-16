import { Component, OnInit } from "@angular/core";
import { TicketService } from "../../services/ticket.service";
import { BatchService } from "../../services/batch.service";
import { MealTicket, TicketApproval } from "../../models/ticket.model";
import { Batch, BatchCreate } from "../../models/batch.model";
import { User } from "../../models/user.model";
import { ROLES } from "../../models/role.model";
import { AuthService } from "src/app/services/auth.service";
import { Department } from "src/app/models/department.model";
import { DepartmentService } from "src/app/services/department.service";

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

  employees: User[] = [];
  loadingEmployees = false;
  departments: Department[] = []; // Assuming you have a Department interface defined somewhere

  batchRequest: BatchCreate = {
    ticketDate: new Date(),
    mealType: "Lunch",
    employeeIds: [],
    departmentId: null,
  };

  availableEmployees: User[] = [];
  selectedEmployees: User[] = [];

  today = new Date().toISOString().split("T")[0];

  constructor(
    private ticketService: TicketService,
    private batchService: BatchService,
    private authService: AuthService,
    private departmentService: DepartmentService,   // ← Add this

  ) {}

  ngOnInit(): void {
    this.loadPendingTickets();
    this.loadBatches();
    this.loadEmployees(); 
    this.loadDepartments();   // ← Load departments
 
  }

  loadPendingTickets(): void {
    this.loading = true;
    this.ticketService.getPendingTickets().subscribe({
      next: (data: MealTicket[]) => {
        // this.pendingTickets = data;
        this.pendingTickets = data.map(ticket => ({ 
          ...ticket, 
          departmentName: ticket.employee?.department?.name || ticket.department || 
          "Unknown"
        }));
         
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

  // ---------- Load Real Employees ----------
  loadEmployees(): void {
    this.loadingEmployees = true;
    this.authService.getEmployees().subscribe({
      next: (data: User[]) => {
        this.employees = data;
        this.loadingEmployees = false;
      },
      error: (err: any) => {
        console.error('Error loading employees:', err);
        this.loadingEmployees = false;
      }
    });
  }

  loadDepartments(): void {
    this.departmentService.getDepartments().subscribe({
      next: (data: Department[]) => {
        this.departments = data;
      },
      error: (err: any) => {
        console.error('Error loading departments:', err);
      }
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
      alert('Please select at least one employee');
      return;
    }

    if (!this.batchRequest.mealType) {
      alert('Please select a meal type');
      return;
    }

    // Build the request with correct types
    const request: BatchCreate = {
      ticketDate: this.batchRequest.ticketDate || new Date(),
      mealType: this.batchRequest.mealType,
      employeeIds: this.selectedEmployees.map((e) => e.id),
      departmentId: this.batchRequest.departmentId ?? null
    };

    console.log('Batch request:', request); // ← Debug log

    this.loading = true;
    this.batchService.createBatch(request).subscribe({
      next: (data: Batch) => {
        this.loading = false;
        this.showBatchForm = false;
        this.selectedEmployees = [];
        this.loadBatches();
        alert(`Batch created successfully! ${data.totalTickets} tickets generated.`);
      },
      error: (err: any) => {
        this.loading = false;
        console.error('Batch error:', err);
        alert(err.error?.message || 'Error creating batch');
      }
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
    // const employee = this.getMockEmployees().find((e) => e.id === +id);
    const employee = this.employees.find((e) => e.id === +id);
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
        departmentId: 1,
        isActive: true,
        createdAt: new Date(),
      },
      {
        id: 2,
        username: "jane.smith",
        fullName: "Jane Smith",
        role: ROLES.EMPLOYEE,
        departmentId: 1,
        isActive: true,
        createdAt: new Date(),
      },
      {
        id: 3,
        username: "bob.johnson",
        fullName: "Bob Johnson",
        role: ROLES.EMPLOYEE,
        departmentId: 2,
        isActive: true,
        createdAt: new Date(),
      },
      {
        id: 4,
        username: "alice.williams",
        fullName: "Alice Williams",
        role: ROLES.EMPLOYEE,
        departmentId: 1,
        isActive: true,
        createdAt: new Date(),
      },
      {
        id: 5,
        username: "charlie.brown",
        fullName: "Charlie Brown",
        role: ROLES.EMPLOYEE,
        departmentId: 3,
        isActive: true,
        createdAt: new Date(),
      },
    ];
  }
}
