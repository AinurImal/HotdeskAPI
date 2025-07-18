import { HttpClientModule } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app'; // <-- Add this import

bootstrapApplication(App, {
  providers: [
    importProvidersFrom(HttpClientModule)
  ]
});


