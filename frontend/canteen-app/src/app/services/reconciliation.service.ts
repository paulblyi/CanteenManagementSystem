import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Reconciliation, VarianceDetail } from '../models/reconciliation.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ReconciliationService {
  private apiUrl = `${environment.apiUrl}/reconciliation`;

  constructor(private http: HttpClient) {}

  createReconciliation(date: Date): Observable<Reconciliation> {
    return this.http.post<Reconciliation>(`${this.apiUrl}/create`, { reconciliationDate: date });
  }

  completeReconciliation(id: number, notes?: string): Observable<Reconciliation> {
    return this.http.put<Reconciliation>(`${this.apiUrl}/complete/${id}`, { notes });
  }

  getReconciliation(id: number): Observable<Reconciliation> {
    return this.http.get<Reconciliation>(`${this.apiUrl}/${id}`);
  }

  getReconciliations(startDate?: Date, endDate?: Date): Observable<Reconciliation[]> {
    let params = '';
    if (startDate) params += `?startDate=${startDate.toISOString().split('T')[0]}`;
    if (endDate) params += `${params ? '&' : '?'}endDate=${endDate.toISOString().split('T')[0]}`;
    return this.http.get<Reconciliation[]>(`${this.apiUrl}${params}`);
  }

  getVarianceDetails(id: number): Observable<VarianceDetail[]> {
    return this.http.get<VarianceDetail[]>(`${this.apiUrl}/${id}/variances`);
  }

  reconcileDate(date: Date): Observable<Reconciliation> {
    return this.http.post<Reconciliation>(`${this.apiUrl}/reconcile-date`, { reconciliationDate: date });
  }

  autoReconcile(id: number): Observable<Reconciliation> {
    return this.http.post<Reconciliation>(`${this.apiUrl}/${id}/auto-reconcile`, {});
  }
}
