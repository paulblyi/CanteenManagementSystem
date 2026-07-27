export const ROLES = {
  ADMIN: "Admin",
  HUMAN_CAPITAL: "HumanCapital",
  EMPLOYEE: "Employee",
  CHEF: "Chef",
  FINANCE: "Finance",
} as const;

export type Role = (typeof ROLES)[keyof typeof ROLES];

export const ALL_ROLES: Role[] = [
  ROLES.ADMIN,
  ROLES.HUMAN_CAPITAL,
  ROLES.EMPLOYEE,
  ROLES.CHEF,
  ROLES.FINANCE,
];

export const ROLE_PORTAL_MAP: Record<Role, string> = {
  [ROLES.ADMIN]: "Admin Portal",
  [ROLES.HUMAN_CAPITAL]: "Human Capital Portal",
  [ROLES.EMPLOYEE]: "Employee Portal",
  [ROLES.CHEF]: "Chef Portal",
  [ROLES.FINANCE]: "Finance & Reconciliation Portal",
};

export const ROLE_DESCRIPTIONS: Record<Role, string> = {
  [ROLES.ADMIN]: "Full system administration (users, roles, system settings)",
  [ROLES.HUMAN_CAPITAL]:
    "Manage meal tickets, create batches, approve requests",
  [ROLES.EMPLOYEE]: "Request meal tickets, view history",
  [ROLES.CHEF]: "Validate and redeem meal tickets",
  [ROLES.FINANCE]: "View reconciliations, reports, billing",
};
