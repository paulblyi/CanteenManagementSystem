export interface Reconciliation {
  id: number;
  reconciliationDate: Date;
  totalTicketsIssued: number;
  totalTicketsRedeemed: number;
  totalTicketsPending: number;
  variance: number;
  status: 'Pending' | 'Completed' | 'Variance';
  reconciledBy?: string;
  reconciledAt?: Date;
  notes?: string;
  breakfast: MealTypeBreakdown;
  lunch: MealTypeBreakdown;
  dinner: MealTypeBreakdown;
  departmentBreakdown: DepartmentBreakdown[];
}

export interface MealTypeBreakdown {
  issued: number;
  redeemed: number;
  pending: number;
  variance: number;
}

export interface DepartmentBreakdown {
  department: string;
  issued: number;
  redeemed: number;
  pending: number;
  variance: number;
}

export interface VarianceDetail {
  ticketNumber: string;
  employeeName: string;
  department: string;
  mealType: string;
  ticketDate: Date;
  status: string;
  varianceType: 'Missing' | 'Extra' | 'Duplicate';
}
