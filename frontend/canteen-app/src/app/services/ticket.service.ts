import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import {
  MealTicket,
  TicketRequest,
  TicketApproval,
  TicketRedemption,
} from "../models/ticket.model";
import { environment } from "../../environments/environment";
import { RedemptionLog } from "../models/redemption.model";

@Injectable({
  providedIn: "root",
})
export class TicketService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Employee endpoints
  requestTicket(request: TicketRequest): Observable<MealTicket> {
    return this.http.post<MealTicket>(
      `${this.apiUrl}/employee/request-ticket`,
      request,
    );
  }

  getMyTickets(): Observable<MealTicket[]> {
    return this.http.get<MealTicket[]>(`${this.apiUrl}/employee/my-tickets`);
  }

  getTicketByNumber(ticketNumber: string): Observable<MealTicket> {
    return this.http.get<MealTicket>(
      `${this.apiUrl}/employee/ticket/${ticketNumber}`,
    );
  }

  cancelTicket(ticketId: number): Observable<any> {
    return this.http.delete(
      `${this.apiUrl}/employee/cancel-ticket/${ticketId}`,
    );
  }

  // Human Capital endpoints
  getPendingTickets(department?: string): Observable<MealTicket[]> {
    const params = department ? `?department=${department}` : "";
    return this.http.get<MealTicket[]>(
      `${this.apiUrl}/humancapital/pending-tickets${params}`,
    );
  }

  approveTicket(approval: TicketApproval): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/humancapital/approve-ticket`,
      approval,
    );
  }

  // Chef endpoints
  redeemTicket(redemption: TicketRedemption): Observable<any> {
    return this.http.post(`${this.apiUrl}/chef/redeem-ticket`, redemption);
  }

  validateTicket(ticketNumber: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/chef/validate-ticket/${ticketNumber}`);
  }

  // Addtional recentRedemption endpoint for chef to get recently redeemed tickets
  getRecentRedemptions(count: number = 10): Observable<RedemptionLog[]> {
    return this.http.get<RedemptionLog[]>(
      `${this.apiUrl}/chef/recent-redemptions?count=${count}`,
    );
  }
}
