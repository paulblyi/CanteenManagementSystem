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

const routes: Routes = [
  { path: "", redirectTo: "/login", pathMatch: "full" },
  { path: "login", component: LoginComponent },
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
    data: { roles: ["Employee", "Admin"] },
  },
  {
    path: "humancapital",
    component: HumanCapitalComponent,
    canActivate: [AuthGuard],
    data: { roles: ["HumanCapital", "Admin"] },
  },
  {
    path: "chef",
    component: ChefComponent,
    canActivate: [AuthGuard],
    data: { roles: ["Chef", "Admin"] },
  },
  {
    path: "reconciliation",
    component: ReconciliationComponent,
    canActivate: [AuthGuard],
    data: { roles: ["Finance", "Admin"] },
  },
  {
    path: "reports",
    component: ReportsComponent,
    canActivate: [AuthGuard],
    data: { roles: ["Finance", "Admin"] },
  },
  { path: "**", redirectTo: "/dashboard" },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
