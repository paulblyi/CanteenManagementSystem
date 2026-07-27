namespace CanteenApi.Models
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string HumanCapital = "HumanCapital";
        public const string Employee = "Employee";
        public const string Chef = "Chef";
        public const string Finance = "Finance";

        // Optional: get all roles as a list
        public static readonly IReadOnlyList<string> AllRoles = new[]
        {
            Admin,
            HumanCapital,
            Employee,
            Chef,
            Finance
        };

        // Portal mapping (optional)
        public static string GetPortalForRole(string role)
        {
            return role switch
            {
                Admin => "Admin Portal",
                HumanCapital => "Human Capital Portal",
                Employee => "Employee Portal",
                Chef => "Chef Portal",
                Finance => "Finance & Reconciliation Portal",
                _ => "Unknown"
            };
        }
    }
}