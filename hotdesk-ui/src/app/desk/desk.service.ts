import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Desk, CreateDeskRequest, UpdateDeskRequest } from './desk.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DeskService {
  private readonly apiUrl = `${environment.apiUrl}/desks`;

  constructor(private http: HttpClient) {}

  // GET - Retrieve all desks
  getDesks(): Observable<Desk[]> {
    return this.http.get<Desk[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET - Retrieve available desks only
  getAvailableDesks(): Observable<Desk[]> {
    return this.http.get<Desk[]>(`${this.apiUrl}/available`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET - Retrieve desks by floor
  getDesksByFloor(floor: number): Observable<Desk[]> {
    return this.http.get<Desk[]>(`${this.apiUrl}/floor/${floor}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET - Retrieve a single desk by ID
  getDeskById(id: number): Observable<Desk> {
    return this.http.get<Desk>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // POST - Create a new desk
  createDesk(desk: CreateDeskRequest): Observable<Desk> {
    return this.http.post<Desk>(this.apiUrl, desk)
      .pipe(
        catchError(this.handleError)
      );
  }

  // PUT - Update an existing desk
  updateDesk(desk: UpdateDeskRequest): Observable<Desk> {
    return this.http.put<Desk>(`${this.apiUrl}/${desk.id}`, desk)
      .pipe(
        catchError(this.handleError)
      );
  }

  // DELETE - Delete a desk by ID
  deleteDesk(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // Error handling
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Server Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    console.error(errorMessage);
    return throwError(() => errorMessage);
  }
}
