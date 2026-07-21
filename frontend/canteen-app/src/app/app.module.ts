import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { CommonModule } from "@angular/common";

import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";

// Components
import { LoginComponent } from "./components/login/login.component";
import { DashboardComponent } from "./components/dashboard/dashboard.component";
import { EmployeeComponent } from "./components/employee/employee.component";
import { HumanCapitalComponent } from "./components/humancapital/humancapital.component";
import { ChefComponent } from "./components/chef/chef.component";
import { ReconciliationComponent } from "./components/reconciliation/reconciliation.component";
import { ReportsComponent } from "./components/reports/reports.component";

// Interceptors
import { AuthInterceptor } from "./interceptors/auth.interceptor";

// Error Handlers
import { ErrorHandler } from "@angular/core";
import { GlobalErrorHandler } from "./error-handler";

// Optional but Recommended
import { APP_BASE_HREF } from "@angular/common";
import { ChangePasswordComponent } from "./components/profile/change-password.component";

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DashboardComponent,
    EmployeeComponent,
    HumanCapitalComponent,
    ChefComponent,
    ReconciliationComponent,
    ReportsComponent,
    ChangePasswordComponent,
  ],

  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    // CommonModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    {
      provide: ErrorHandler,
      useClass: GlobalErrorHandler,
    },
    {
      provide: APP_BASE_HREF,
      useValue: "/",
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
