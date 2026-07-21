import { Component, OnInit } from "@angular/core";
import { AuthService } from "../../../services/auth.service";
import { UserListDto, CreateUserRequest } from "../../../models/user.model";

@Component({
  selector: "app-admin-users",
  templateUrl: "./admin-users.component.html",
  styleUrls: ["./admin-users.component.css"],
})
export class AdminUsersComponent implements OnInit {
  users: UserListDto[] = [];
  loading = false;
  showAddForm = false;

  newUser: CreateUserRequest = {
    username: "",
    password: "",
    fullName: "",
    email: "",
    role: "Employee",
    department: "",
    employeeCode: "",
  };

  roles = ["Employee", "HumanCapital", "Chef", "Admin", "Finance"];

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.authService.getUsers().subscribe({
      next: (data) => {
        this.users = data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      },
    });
  }

  createUser(): void {
    this.authService.createUser(this.newUser).subscribe({
      next: () => {
        this.showAddForm = false;
        this.loadUsers();
        alert("User created successfully.");
      },
      error: (err) => {
        alert(err.error?.message || "Error creating user.");
      },
    });
  }

  toggleActive(user: UserListDto): void {
    this.authService.toggleUserActive(user.id).subscribe({
      next: () => this.loadUsers(),
      error: (err) => alert("Error toggling user status."),
    });
  }

  changeRole(user: UserListDto, newRole: string): void {
    if (confirm(`Change role of ${user.fullName} to ${newRole}?`)) {
      this.authService.updateUserRole(user.id, newRole).subscribe({
        next: () => this.loadUsers(),
        error: (err) => alert("Error updating role."),
      });
    }
  }

  resetPassword(user: UserListDto): void {
    const newPassword = prompt(`Enter new password for ${user.fullName}:`);
    if (newPassword && newPassword.length >= 6) {
      this.authService.resetUserPassword(user.id, newPassword).subscribe({
        next: () => alert("Password reset successfully."),
        error: (err) => alert("Error resetting password."),
      });
    } else if (newPassword !== null) {
      alert("Password must be at least 6 characters.");
    }
  }
}
