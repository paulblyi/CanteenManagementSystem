import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "./guards/auth.guard";

import { LoginComponent } from "./components/login/login.component";
import { DashboardComponent } from "./components/dashboard/dashboard.component";
import { EmployeeComponent } from "./components/employee/employee.component";
import { HumanCapitalComponent } from "./components/humancapital/humancapital.component";
import { ChefComponent } from "./components/chef/chef.component";
import { ReconciliationComponent } from "./components/reconciliation/reconciliation.component";
import { ReportsComponent } from "./components/reports/reports.component";
import { AdminUsersComponent } from "./components/admin/admin-users/admin-users.component";
import { ChangePasswordComponent } from "./components/profile/change-password.component";
import { ROLES } from "./models/role.model";
import { DepartmentsComponent } from "./components/admin/departments/departments.component";

const routes: Routes = [
  { path: "", redirectTo: "/login", pathMatch: "full" },
  { path: "login", component: LoginComponent },

  // Protected routes
  {
    path: "dashboard",
    component: DashboardComponent,
    canActivate: [AuthGuard],
    data: { roles: ["Employee", "HumanCapital", "Chef", "Admin", "Finance"] },
  },
  {
    path: "employee",
    component: EmployeeComponent,
    canActivate: [AuthGuard],
    data: { roles: [ROLES.EMPLOYEE, ROLES.ADMIN] },
  },
  {
    path: "humancapital",
    component: HumanCapitalComponent,
    canActivate: [AuthGuard],
    data: { roles: [ROLES.HUMAN_CAPITAL, ROLES.ADMIN] },
  },
  {
    path: "chef",
    component: ChefComponent,
    canActivate: [AuthGuard],
    data: { roles: [ROLES.CHEF, ROLES.ADMIN] },
  },
  {
    path: "reconciliation",
    component: ReconciliationComponent,
    canActivate: [AuthGuard],
    data: { roles: [ROLES.FINANCE, ROLES.ADMIN] },
  },
  {
    path: "reports",
    component: ReportsComponent,
    canActivate: [AuthGuard],
    data: { roles: [ROLES.FINANCE, ROLES.ADMIN] },
  },

  // ★ Admin‑only routes ★
  {
    path: "admin/users",
    component: AdminUsersComponent,
    canActivate: [AuthGuard],
    data: { roles: [ROLES.ADMIN] },
  },
  {
    path: "admin/departments",
    component: DepartmentsComponent,
    canActivate: [AuthGuard],
    data: { roles: [ROLES.ADMIN] },
  },

  // User profile
  {
    path: "profile/change-password",
    component: ChangePasswordComponent,
    canActivate: [AuthGuard],
  },

  // WILDCARD – must be last
  { path: "**", redirectTo: "/dashboard" },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
