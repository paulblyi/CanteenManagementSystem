import { Component, OnInit } from "@angular/core";
import { AuthService } from "../../../services/auth.service";
import { DepartmentService } from "../../../services/department.service";
import { UserListDto, CreateUserRequest } from "../../../models/user.model";
import { Department } from "../../../models/department.model";
import {
  ROLES,
  ALL_ROLES,
  ROLE_PORTAL_MAP,
  Role,
} from "../../../models/role.model";

@Component({
  selector: "app-admin-users",
  templateUrl: "./admin-users.component.html",
  styleUrls: ["./admin-users.component.css"],
})
export class AdminUsersComponent implements OnInit {
  // Data
  users: UserListDto[] = [];
  filteredUsers: UserListDto[] = [];
  departments: Department[] = [];
  loading = false;
  searchTerm = "";

  // Add user modal
  showAddUserModal = false;
  newUser: CreateUserRequest = {
    username: "",
    password: "",
    fullName: "",
    email: "",
    role: ROLES.EMPLOYEE,
    departmentId: undefined, // ✅ Use undefined instead of null
    employeeCode: "",
  };

  // For dropdown display: map each role to its friendly portal name
  roleOptions = ALL_ROLES.map((role) => ({
    value: role,
    label: ROLE_PORTAL_MAP[role as Role] || role,
  }));

  // For table display
  getPortalForRole(role: string): string {
    return ROLE_PORTAL_MAP[role as Role] || role;
  }

  constructor(
    private authService: AuthService,
    private departmentService: DepartmentService,
  ) {}

  ngOnInit(): void {
    this.loadDepartments();
    this.loadUsers();
  }

  // ---------- Data Loading ----------
  loadDepartments(): void {
    this.departmentService.getDepartments().subscribe({
      next: (data: Department[]) => (this.departments = data),
      error: (err) => console.error("Error loading departments:", err),
    });
  }

  loadUsers(): void {
    this.loading = true;
    this.authService.getUsers().subscribe({
      next: (data: UserListDto[]) => {
        this.users = data;
        this.filteredUsers = data;
        this.loading = false;
      },
      error: (err: any) => {
        console.error("Error loading users:", err);
        this.loading = false;
      },
    });
  }

  // ---------- Filtering ----------
  filterUsers(): void {
    const term = this.searchTerm.toLowerCase().trim();
    if (!term) {
      this.filteredUsers = this.users;
      return;
    }
    this.filteredUsers = this.users.filter(
      (user) =>
        user.username.toLowerCase().includes(term) ||
        user.fullName.toLowerCase().includes(term) ||
        (user.email && user.email.toLowerCase().includes(term)) ||
        (user.departmentName &&
          user.departmentName.toLowerCase().includes(term)),
    );
  }

  // ---------- Stats ----------
  getActiveUsersCount(): number {
    return this.users.filter((u) => u.isActive).length;
  }

  getAdminCount(): number {
    return this.users.filter((u) => u.role === ROLES.ADMIN).length;
  }

  getHCEmployeesCount(): number {
    return this.users.filter((u) => u.role === ROLES.HUMAN_CAPITAL).length;
  }

  // ---------- Modal Control ----------
  openAddUserModal(): void {
    this.showAddUserModal = true;
  }

  closeAddUserModal(): void {
    this.showAddUserModal = false;
    // Reset the form – use undefined for departmentId
    this.newUser = {
      username: "",
      password: "",
      fullName: "",
      email: "",
      role: ROLES.EMPLOYEE,
      departmentId: undefined,
      employeeCode: "",
    };
  }

  // ---------- CRUD Operations ----------
  createUser(): void {
    if (
      !this.newUser.username ||
      !this.newUser.password ||
      !this.newUser.fullName
    ) {
      alert("Please fill in all required fields.");
      return;
    }

    this.authService.createUser(this.newUser).subscribe({
      next: () => {
        this.closeAddUserModal();
        this.loadUsers();
        alert("User created successfully.");
      },
      error: (err: any) => {
        alert(err.error?.message || "Error creating user.");
      },
    });
  }

  toggleActive(user: UserListDto): void {
    this.authService.toggleUserActive(user.id).subscribe({
      next: () => this.loadUsers(),
      error: (err: any) => alert("Error toggling user status."),
    });
  }

  changeRole(user: UserListDto, newRole: string): void {
    if (
      confirm(
        `Change role of ${user.fullName} to ${this.getPortalForRole(newRole)}?`,
      )
    ) {
      this.authService.updateUserRole(user.id, newRole).subscribe({
        next: () => this.loadUsers(),
        error: (err: any) => alert("Error updating role."),
      });
    }
  }

  resetPassword(user: UserListDto): void {
    const newPassword = prompt(`Enter new password for ${user.fullName}:`);
    if (newPassword && newPassword.length >= 6) {
      this.authService.resetUserPassword(user.id, newPassword).subscribe({
        next: () => alert("Password reset successfully."),
        error: (err: any) => alert("Error resetting password."),
      });
    } else if (newPassword !== null) {
      alert("Password must be at least 6 characters.");
    }
  }
}
