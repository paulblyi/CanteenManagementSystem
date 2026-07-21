import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable, tap } from "rxjs";
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  User,
  ChangePasswordRequest,
  UserListDto,
  CreateUserRequest,
} from "../models/user.model";
import { environment } from "../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    const userData = localStorage.getItem("user");
    if (userData) {
      this.currentUserSubject.next(JSON.parse(userData));
    }
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap((response) => {
        localStorage.setItem("token", response.token);
        localStorage.setItem("user", JSON.stringify(response));
        this.currentUserSubject.next(response as any);
      }),
    );
  }

  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, request);
  }

  logout(): void {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem("token");
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role || user?.role === "Admin";
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    if (!user) return false;
    if (user.role === "Admin") return true;
    return roles.includes(user.role);
  }

  // Additional methods for user management
  getUsers(): Observable<UserListDto[]> {
    return this.http.get<UserListDto[]>(`${this.apiUrl}/admin/users`);
  }

  createUser(request: CreateUserRequest): Observable<UserListDto> {
    return this.http.post<UserListDto>(`${this.apiUrl}/admin/users`, request);
  }

  toggleUserActive(userId: number): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/admin/users/${userId}/toggle-active`,
      {},
    );
  }

  updateUserRole(userId: number, role: string): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/users/${userId}/role`, { role });
  }

  resetUserPassword(userId: number, newPassword: string): Observable<any> {
    return this.http.put(
      `${this.apiUrl}/admin/users/${userId}/reset-password`,
      { newPassword },
    );
  }

  changePassword(request: ChangePasswordRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/profile/change-password`, request);
  }
}
