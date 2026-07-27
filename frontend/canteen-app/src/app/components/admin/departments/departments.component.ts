import { Component, OnInit } from "@angular/core";
import { DepartmentService } from "../../../services/department.service";
import {
  Department,
  CreateDepartmentRequest,
  UpdateDepartmentRequest,
} from "../../../models/department.model";

@Component({
  selector: "app-departments",
  templateUrl: "./departments.component.html",
  styleUrls: ["./departments.component.css"],
})
export class DepartmentsComponent implements OnInit {
  // Data
  departments: Department[] = [];
  filteredDepartments: Department[] = [];
  loading = false;
  searchTerm = "";

  // Add modal
  showAddModal = false;
  newDepartment: CreateDepartmentRequest = {
    name: "",
    description: "",
  };

  // Edit modal
  showEditModal = false;
  currentDepartment: Department | null = null;
  editDepartment: UpdateDepartmentRequest = {
    name: "",
    description: "",
    isActive: true,
  };

  constructor(private deptService: DepartmentService) {}

  ngOnInit(): void {
    this.loadDepartments();
  }

  // ---------- Data Loading ----------
  loadDepartments(): void {
    this.loading = true;
    this.deptService.getDepartments(true).subscribe({
      next: (data: Department[]) => {
        this.departments = data;
        this.filteredDepartments = data;
        this.loading = false;
      },
      error: (err) => {
        console.error("Error loading departments:", err);
        this.loading = false;
      },
    });
  }

  // ---------- Filtering ----------
  filterDepartments(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredDepartments = this.departments;
      return;
    }
    this.filteredDepartments = this.departments.filter(
      (d) =>
        d.name.toLowerCase().includes(term) ||
        (d.description && d.description.toLowerCase().includes(term)),
    );
  }

  // ---------- Stats ----------
  getActiveCount(): number {
    return this.departments.filter((d) => d.isActive).length;
  }

  getTotalUsersCount(): number {
    return this.departments.reduce((sum, d) => sum + (d.userCount || 0), 0);
  }

  getAverageUsersPerDept(): number {
    const total = this.departments.length;
    if (total === 0) return 0;
    const users = this.departments.reduce(
      (sum, d) => sum + (d.userCount || 0),
      0,
    );
    return Math.round(users / total);
  }

  // ---------- Add Modal ----------
  openAddModal(): void {
    this.newDepartment = { name: "", description: "" };
    this.showAddModal = true;
  }

  closeAddModal(): void {
    this.showAddModal = false;
  }

  createDepartment(): void {
    if (!this.newDepartment.name.trim()) {
      alert("Department name is required.");
      return;
    }
    this.deptService.createDepartment(this.newDepartment).subscribe({
      next: () => {
        this.closeAddModal();
        this.loadDepartments();
        alert("Department created successfully.");
      },
      error: (err) => {
        alert(err.error?.message || "Error creating department.");
      },
    });
  }

  // ---------- Edit Modal ----------
  openEditModal(dept: Department): void {
    this.currentDepartment = dept;
    this.editDepartment = {
      name: dept.name,
      description: dept.description || "",
      isActive: dept.isActive,
    };
    this.showEditModal = true;
  }

  closeEditModal(): void {
    this.showEditModal = false;
    this.currentDepartment = null;
  }

  updateDepartment(): void {
    if (!this.currentDepartment) return;
    if (!this.editDepartment.name.trim()) {
      alert("Department name is required.");
      return;
    }
    this.deptService
      .updateDepartment(this.currentDepartment.id, this.editDepartment)
      .subscribe({
        next: () => {
          this.closeEditModal();
          this.loadDepartments();
          alert("Department updated successfully.");
        },
        error: (err) => {
          alert(err.error?.message || "Error updating department.");
        },
      });
  }

  // ---------- Actions ----------
  toggleActive(dept: Department): void {
    this.deptService.toggleActive(dept.id).subscribe({
      next: () => this.loadDepartments(),
      error: (err) => alert("Error toggling status."),
    });
  }

  deleteDepartment(dept: Department): void {
    if (
      confirm(`Delete department "${dept.name}"? Users will be unassigned.`)
    ) {
      this.deptService.deleteDepartment(dept.id).subscribe({
        next: () => this.loadDepartments(),
        error: (err) =>
          alert(err.error?.message || "Error deleting department."),
      });
    }
  }
}
