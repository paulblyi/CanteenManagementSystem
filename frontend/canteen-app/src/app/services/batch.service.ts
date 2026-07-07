import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Batch, BatchCreate } from '../models/batch.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BatchService {
  private apiUrl = `${environment.apiUrl}/humancapital`;

  constructor(private http: HttpClient) {}

  createBatch(request: BatchCreate): Observable<Batch> {
    return this.http.post<Batch>(`${this.apiUrl}/create-batch`, request);
  }

  getBatches(date?: Date): Observable<Batch[]> {
    const params = date ? `?date=${date.toISOString().split('T')[0]}` : '';
    return this.http.get<Batch[]>(`${this.apiUrl}/batches${params}`);
  }

  getBatchById(id: number): Observable<Batch> {
    return this.http.get<Batch>(`${this.apiUrl}/batches/${id}`);
  }

  cancelBatch(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/cancel-batch/${id}`, {});
  }
}
