export interface User {
  id: number;
  username: string;
  fullName: string;
  email?: string;
  role: 'Employee' | 'HumanCapital' | 'Chef' | 'Admin' | 'Finance';
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
