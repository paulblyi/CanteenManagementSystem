import { Component } from "@angular/core";
import { Router } from "@angular/router"; // ← add import
import { AuthService } from "../../services/auth.service";

@Component({
  selector: "app-change-password",
  templateUrl: "./change-password.component.html",
  styleUrls: ["./change-password.component.css"],
})
export class ChangePasswordComponent {
  currentPassword = "";
  newPassword = "";
  confirmPassword = "";
  message = "";
  success = false;

  constructor(
    private authService: AuthService,
    private router: Router, // ← inject Router
  ) {}

  changePassword(): void {
    if (this.newPassword !== this.confirmPassword) {
      this.message = "Passwords do not match.";
      this.success = false;
      return;
    }
    if (this.newPassword.length < 6) {
      this.message = "Password must be at least 6 characters.";
      this.success = false;
      return;
    }
    this.authService
      .changePassword({
        currentPassword: this.currentPassword,
        newPassword: this.newPassword,
      })
      .subscribe({
        next: () => {
          this.message = "Password changed successfully!";
          this.success = true;
          this.currentPassword = "";
          this.newPassword = "";
          this.confirmPassword = "";
          // Optionally auto-redirect after success
          setTimeout(() => this.router.navigate(["/dashboard"]), 2000);
        },
        error: (err: any) => {
          this.message = err.error?.message || "Error changing password.";
          this.success = false;
        },
      });
  }

  goBack(): void {
    this.router.navigate(["/dashboard"]);
  }
}
