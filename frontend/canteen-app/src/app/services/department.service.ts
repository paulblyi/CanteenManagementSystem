import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import {
  Department,
  CreateDepartmentRequest,
  UpdateDepartmentRequest,
} from "../models/department.model";
import { environment } from "../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class DepartmentService {
  private apiUrl = `${environment.apiUrl}/department`;

  constructor(private http: HttpClient) {}

  getDepartments(includeInactive: boolean = false): Observable<Department[]> {
    return this.http.get<Department[]>(
      `${this.apiUrl}?includeInactive=${includeInactive}`,
    );
  }

  getDepartment(id: number): Observable<Department> {
    return this.http.get<Department>(`${this.apiUrl}/${id}`);
  }

  createDepartment(request: CreateDepartmentRequest): Observable<Department> {
    return this.http.post<Department>(this.apiUrl, request);
  }

  updateDepartment(
    id: number,
    request: UpdateDepartmentRequest,
  ): Observable<Department> {
    return this.http.put<Department>(`${this.apiUrl}/${id}`, request);
  }

  deleteDepartment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  toggleActive(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/${id}/toggle-active`, {});
  }
}
