export interface RedemptionLog {
  ticketNumber: string;
  employeeName: string;
  redeemedAt: Date | string; // string if coming from API as ISO, or Date
}
