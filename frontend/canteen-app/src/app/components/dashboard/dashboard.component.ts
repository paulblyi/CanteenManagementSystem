import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ReportService } from '../../services/report.service';
import { DashboardStats } from '../../models/report.model';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  user: User | null = null;
  stats: DashboardStats | null = null;
  loading = true;

  constructor(
    private authService: AuthService,
    private reportService: ReportService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getCurrentUser();
    this.loadDashboardStats();
  }

  loadDashboardStats(): void {
    this.loading = true;
    this.reportService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading dashboard stats:', err);
        this.loading = false;
      }
    });
  }

  hasRole(role: string): boolean {
    return this.authService.hasRole(role);
  }

  hasAnyRole(roles: string[]): boolean {
    return this.authService.hasAnyRole(roles);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getActivityIcon(type: string): string {
    switch (type) {
      case 'Request': return '📝';
      case 'Approval': return '✅';
      case 'Redemption': return '🍽️';
      default: return '📌';
    }
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return '#ffc107';
      case 'Approved': return '#28a745';
      case 'Redeemed': return '#007bff';
      case 'Cancelled': return '#dc3545';
      default: return '#6c757d';
    }
  }
}
