// -------------------- Authentication --------------------
export interface User {
  id: number;
  username: string;
  fullName: string;
  email?: string;
  role: "Employee" | "HumanCapital" | "Chef" | "Admin" | "Finance";
  department?: string;
  employeeCode?: string;
  isActive: boolean;
  createdAt: Date;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  userId: number;
  username: string;
  fullName: string;
  role: string;
  token: string;
}

export interface RegisterRequest {
  username: string;
  password: string;
  fullName: string;
  email?: string;
  role: string;
  department?: string;
  employeeCode?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

// -------------------- Admin User Management --------------------
export interface UserListDto {
  id: number;
  username: string;
  fullName: string;
  email?: string;
  role: string;
  department?: string;
  isActive: boolean;
  createdAt: Date;
}

export interface CreateUserRequest {
  username: string;
  password: string;
  fullName: string;
  email?: string;
  role: string;
  department?: string;
  employeeCode?: string;
}

export interface UpdateRoleRequest {
  role: string;
}

export interface ResetPasswordRequest {
  newPassword: string;
}
