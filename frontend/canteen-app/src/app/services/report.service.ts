import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DailyReport, DashboardStats, DepartmentReport } from '../models/report.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  private apiUrl = `${environment.apiUrl}/reports`;

  constructor(private http: HttpClient) {}

  getDailyReport(date: Date): Observable<DailyReport> {
    const dateStr = date.toISOString().split('T')[0];
    return this.http.get<DailyReport>(`${this.apiUrl}/daily?date=${dateStr}`);
  }

  getMonthlyReport(year: number, month: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/monthly?year=${year}&month=${month}`);
  }

  getDashboardStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.apiUrl}/dashboard`);
  }

  getDepartmentReport(startDate?: Date, endDate?: Date): Observable<DepartmentReport[]> {
    let params = '';
    if (startDate) params += `?startDate=${startDate.toISOString().split('T')[0]}`;
    if (endDate) params += `${params ? '&' : '?'}endDate=${endDate.toISOString().split('T')[0]}`;
    return this.http.get<DepartmentReport[]>(`${this.apiUrl}/department${params}`);
  }

  exportReport(startDate: Date, endDate: Date): Observable<Blob> {
    const params = `?startDate=${startDate.toISOString().split('T')[0]}&endDate=${endDate.toISOString().split('T')[0]}`;
    return this.http.get(`${this.apiUrl}/export${params}`, { responseType: 'blob' });
  }
}
