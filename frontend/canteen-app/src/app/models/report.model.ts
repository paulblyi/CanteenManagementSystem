export interface DailyReport {
  date: Date;
  totalTicketsIssued: number;
  totalTicketsRedeemed: number;
  totalTicketsPending: number;
  totalTicketsCancelled: number;
  breakfastIssued: number;
  breakfastRedeemed: number;
  breakfastPending: number;
  lunchIssued: number;
  lunchRedeemed: number;
  lunchPending: number;
  dinnerIssued: number;
  dinnerRedeemed: number;
  dinnerPending: number;
  totalRevenue: number;
  breakfastRevenue: number;
  lunchRevenue: number;
  dinnerRevenue: number;
  departmentBreakdown: DepartmentReport[];
  topEmployees: EmployeeReport[];
}

export interface DepartmentReport {
  department: string;
  totalEmployees: number;
  ticketsIssued: number;
  ticketsRedeemed: number;
  totalCost: number;
  costPerEmployee: number;
}

export interface EmployeeReport {
  employeeId: number;
  employeeName: string;
  department: string;
  totalTickets: number;
  redeemedTickets: number;
  totalCost: number;
  favoriteMeal: string;
}

export interface DashboardStats {
  todayTicketsIssued: number;
  todayTicketsRedeemed: number;
  todayPendingTickets: number;
  todayRevenue: number;
  weekTicketsIssued: number;
  weekTicketsRedeemed: number;
  weekRevenue: number;
  monthTicketsIssued: number;
  monthTicketsRedeemed: number;
  monthRevenue: number;
  activeEmployees: number;
  totalEmployees: number;
  recentActivities: RecentActivity[];
  departmentStats: DepartmentStat[];
  mealTypeStats: MealTypeStat;
}

export interface RecentActivity {
  timestamp: Date;
  activityType: 'Request' | 'Approval' | 'Redemption';
  description: string;
  user: string;
}

export interface DepartmentStat {
  department: string;
  ticketsIssued: number;
  ticketsRedeemed: number;
  cost: number;
}

export interface MealTypeStat {
  breakfastCount: number;
  lunchCount: number;
  dinnerCount: number;
  breakfastRevenue: number;
  lunchRevenue: number;
  dinnerRevenue: number;
}
