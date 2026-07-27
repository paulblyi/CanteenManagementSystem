namespace CanteenApi.Models
{
    public static class MealTypes
    {
        public const string Breakfast = "Breakfast";
        public const string Lunch = "Lunch";
        public const string Dinner = "Dinner";
        public static readonly IReadOnlyList<string> All = new[] { Breakfast, Lunch, Dinner };
    }

    public static class TicketStatuses
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Redeemed = "Redeemed";
        public const string Cancelled = "Cancelled";
        public static readonly IReadOnlyList<string> All = new[] { Pending, Approved, Redeemed, Cancelled };
    }

    public static class BatchStatuses
    {
        public const string Active = "Active";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public static readonly IReadOnlyList<string> All = new[] { Active, Completed, Cancelled };
    }

    public static class ReconciliationStatuses
    {
        public const string Pending = "Pending";
        public const string Completed = "Completed";
        public const string Variance = "Variance";
        public static readonly IReadOnlyList<string> All = new[] { Pending, Completed, Variance };
    }

    public static class RedemptionStatuses
    {
        public const string Success = "Success";
        public const string Failed = "Failed";
        public static readonly IReadOnlyList<string> All = new[] { Success, Failed };
    }

    public static class BillingStatuses
    {
        public const string Pending = "Pending";
        public const string Paid = "Paid";
        public const string Overdue = "Overdue";
        public static readonly IReadOnlyList<string> All = new[] { Pending, Paid, Overdue };
    }
}