import { User } from "./user.model";

export interface MealTicket {
  id: number;
  ticketNumber: string;
  employeeId?: number;
  employee?: User; 
  employeeName: string;
  department: string;
  departmentName?: string;  
  ticketDate: Date;
  mealType: 'Breakfast' | 'Lunch' | 'Dinner';
  status: 'Pending' | 'Approved' | 'Redeemed' | 'Cancelled';
  requestedAt: Date;
  approvedAt?: Date;
  redeemedAt?: Date;
  qrCode?: string;
  notes?: string;
  batchId?: number;
}

export interface TicketRequest {
  ticketDate: Date;
  mealType: string;
  notes?: string;
}

export interface TicketApproval {
  ticketId: number;
  status: 'Approved' | 'Rejected';
  notes?: string;
}

export interface TicketRedemption {
  ticketNumber: string;
  verificationMethod?: string;
}
