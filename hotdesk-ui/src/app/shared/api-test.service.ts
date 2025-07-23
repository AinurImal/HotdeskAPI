import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface ApiTestResult {
  endpoint: string;
  method: string;
  success: boolean;
  status?: number;
  data?: any;
  error?: string;
  responseTime?: number;
}

@Injectable({
  providedIn: 'root'
})
export class ApiTestService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Test API connectivity
  testApiConnection(): Observable<ApiTestResult[]> {
    const tests: Observable<ApiTestResult>[] = [
      this.testEndpoint('GET', '/Users', 'Users'),
      this.testEndpoint('GET', '/Desks', 'Desks'),
      this.testEndpoint('GET', '/Bookings', 'Bookings'),
      // We'll skip individual user test until we have a valid user ID
    ];

    return new Observable(observer => {
      const results: ApiTestResult[] = [];
      let completed = 0;

      tests.forEach(test => {
        test.subscribe({
          next: (result) => {
            results.push(result);
            completed++;
            if (completed === tests.length) {
              observer.next(results);
              observer.complete();
            }
          },
          error: (error) => {
            results.push(error);
            completed++;
            if (completed === tests.length) {
              observer.next(results);
              observer.complete();
            }
          }
        });
      });
    });
  }

  private testEndpoint(method: string, endpoint: string, description: string): Observable<ApiTestResult> {
    const startTime = Date.now();
    const url = `${this.baseUrl}${endpoint}`;

    return this.http.request(method, url).pipe(
      map(data => ({
        endpoint: `${method} ${endpoint} (${description})`,
        method,
        success: true,
        status: 200,
        data: Array.isArray(data) ? `Array with ${data.length} items` : data,
        responseTime: Date.now() - startTime
      })),
      catchError((error: HttpErrorResponse) => {
        const result: ApiTestResult = {
          endpoint: `${method} ${endpoint} (${description})`,
          method,
          success: false,
          status: error.status,
          error: error.status === 0 ? 
            'Cannot connect to API - Is the server running?' : 
            `${error.status} - ${error.message}`,
          responseTime: Date.now() - startTime
        };
        return throwError(() => result);
      })
    );
  }

  // Test different API base URLs
  testDifferentPorts(): Observable<{ port: number; success: boolean; error?: string }[]> {
    const ports = [5000, 5001, 5251, 7000, 7001, 8080];
    const results: { port: number; success: boolean; error?: string }[] = [];

    return new Observable(observer => {
      let completed = 0;

      ports.forEach(port => {
        const testUrl = `http://localhost:${port}/api/Users`;
        
        this.http.get(testUrl).subscribe({
          next: () => {
            results.push({ port, success: true });
            completed++;
            if (completed === ports.length) {
              observer.next(results);
              observer.complete();
            }
          },
          error: (error: HttpErrorResponse) => {
            results.push({ 
              port, 
              success: false, 
              error: error.status === 0 ? 'No connection' : `${error.status} - ${error.message}`
            });
            completed++;
            if (completed === ports.length) {
              observer.next(results);
              observer.complete();
            }
          }
        });
      });
    });
  }
}
