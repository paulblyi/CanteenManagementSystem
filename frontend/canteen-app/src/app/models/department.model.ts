export interface Department {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: Date;
  userCount?: number;
}

export interface CreateDepartmentRequest {
  name: string;
  description?: string;
}

export interface UpdateDepartmentRequest {
  name: string;
  description?: string;
  isActive: boolean;
}
