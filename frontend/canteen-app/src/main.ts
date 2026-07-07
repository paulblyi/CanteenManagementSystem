//import { platformBrowser } from "@angular/platform-browser";
import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";
import { AppModule } from "./app/app.module";

// Optional: import '@angular/compiler' if you need JIT during development.
// In production (AOT), this import is not required and can be removed.
// import '@angular/compiler';

// platformBrowser()
//  .bootstrapModule(AppModule)
//  .catch((err) => console.error(err));

platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .catch((err) => {
    debugger; // ← this will pause execution
    console.error("BOOTSTRAP ERROR:", err);
  });
