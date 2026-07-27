import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService } from "../../services/auth.service";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.css"],
})
export class LoginComponent {
  username = "";
  password = "";
  error = "";
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  onSubmit(): void {
    if (!this.username || !this.password) {
      this.error = "Please enter username and password";
      return;
    }

    this.loading = true;
    this.error = "";

    this.authService
      .login({
        username: this.username,
        password: this.password,
      })
      .subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(["/dashboard"]);
        },
        error: (err) => {
          this.loading = false;
          this.error = err.error?.message || "Invalid username or password";
        },
      });
  }
}
