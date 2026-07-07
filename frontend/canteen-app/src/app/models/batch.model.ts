import { MealTicket } from "./ticket.model";

export interface Batch {
  id: number;
  batchNumber: string;
  department?: string;
  ticketDate: Date;
  mealType: string;
  totalTickets: number;
  status: "Active" | "Completed" | "Cancelled";
  createdAt: Date;
  tickets: MealTicket[];
}

export interface BatchCreate {
  ticketDate: Date;
  mealType: string;
  employeeIds: number[];
  department?: string;
}
