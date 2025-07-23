// Import Angular core application configuration types
import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
// Import routing functionality for navigation between pages
import { provideRouter } from '@angular/router';
// Import HTTP client for making API requests with modern fetch API and interceptor support
import { provideHttpClient, withFetch, withInterceptorsFromDi } from '@angular/common/http';

// Import application routes configuration
import { routes } from './app.routes';
// Import client-side hydration for server-side rendering support
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

// Main application configuration object - defines all providers and services
export const appConfig: ApplicationConfig = {
  providers: [
    // Provide global error handling for uncaught browser errors
    provideBrowserGlobalErrorListeners(),
    // Enable zone.js change detection with event coalescing for better performance
    provideZoneChangeDetection({ eventCoalescing: true }),
    // Configure application routing with defined routes
    provideRouter(routes), 
    // Enable client-side hydration with event replay for SSR applications
    provideClientHydration(withEventReplay()),
    // Configure HTTP client with modern fetch API and interceptor support
    // withFetch(): Uses browser's native fetch API instead of XMLHttpRequest
    // withInterceptorsFromDi(): Enables dependency injection for HTTP interceptors
    provideHttpClient(withFetch(), withInterceptorsFromDi())
  ]
};
