// -------------------- Authentication --------------------
export interface User {
  id: number;
  username: string;
  fullName: string;
  email?: string;
  role: "Employee" | "HumanCapital" | "Chef" | "Admin" | "Finance";
  departmentId?: number;
  departmentName?: string;
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
  department?: string; // Still string for registration (if needed)
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
  departmentId?: number; // ← added
  departmentName?: string; // ← added (instead of department string)
  isActive: boolean;
  createdAt: Date;
}

export interface CreateUserRequest {
  username: string;
  password: string;
  fullName: string;
  email?: string;
  role: string;
  departmentId?: number; // ← use departmentId (not department)
  employeeCode?: string;
}

export interface UpdateRoleRequest {
  role: string;
}

export interface ResetPasswordRequest {
  newPassword: string;
}
