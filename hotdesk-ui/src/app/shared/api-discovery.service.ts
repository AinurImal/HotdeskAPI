import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, forkJoin, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class ApiDiscoveryService {
  
  constructor(private http: HttpClient) {}

  findRunningApi(): Observable<{ port: number; protocol: string; working: boolean }[]> {
    const commonPorts = [
      { port: 5251, protocol: 'http' },
      { port: 5251, protocol: 'https' },
      { port: 5000, protocol: 'http' },
      { port: 5001, protocol: 'https' },
      { port: 7000, protocol: 'http' },
      { port: 7001, protocol: 'https' }
    ];

    const tests = commonPorts.map(config => 
      this.testEndpoint(config.protocol, config.port).pipe(
        map(working => ({ ...config, working })),
        catchError(() => of({ ...config, working: false }))
      )
    );

    return forkJoin(tests);
  }

  private testEndpoint(protocol: string, port: number): Observable<boolean> {
    const url = `${protocol}://localhost:${port}/api/Users`;
    return this.http.get(url).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }
}
