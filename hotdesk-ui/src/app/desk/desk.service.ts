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
  private readonly apiUrl = `${environment.apiUrl}/Desks`;

  constructor(private http: HttpClient) {}

  // GET - Retrieve all desks
  getDesks(): Observable<Desk[]> {
    return this.http.get<Desk[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET - Retrieve a specific desk by ID
  getDeskById(deskId: number): Observable<Desk> {
    return this.http.get<Desk>(`${this.apiUrl}/${deskId}`)
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

  // GET - Check desk availability for a specific date
  checkDeskAvailability(deskId: number, date: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${deskId}/availability?date=${encodeURIComponent(date)}`)
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
  updateDesk(deskId: number, desk: UpdateDeskRequest): Observable<Desk> {
    return this.http.put<Desk>(`${this.apiUrl}/${deskId}`, desk)
      .pipe(
        catchError(this.handleError)
      );
  }

  // DELETE - Remove a desk from the system permanently
  deleteDesk(deskId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${deskId}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // Private method to handle HTTP errors with user-friendly messages
  private handleError = (error: HttpErrorResponse): Observable<never> => {
    let errorMessage = 'An unexpected error occurred. Please try again later.';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side or network error occurred
      errorMessage = `Network Error: ${error.error.message}`;
    } else {
      // Backend returned an unsuccessful response code
      switch (error.status) {
        case 0:
          errorMessage = 'Unable to connect to the server. Please check your internet connection or try again later.';
          break;
        case 400:
          errorMessage = 'Invalid request. Please check your input and try again.';
          break;
        case 401:
          errorMessage = 'You are not authorized to perform this action. Please log in and try again.';
          break;
        case 403:
          errorMessage = 'You do not have permission to perform this action.';
          break;
        case 404:
          errorMessage = 'The requested desk was not found. It may have been deleted or moved.';
          break;
        case 409:
          errorMessage = 'A desk with this number already exists. Please use a different desk number.';
          break;
        case 422:
          errorMessage = 'The provided data is invalid. Please check all fields and try again.';
          break;
        case 500:
          errorMessage = 'Internal server error. Please contact support if this problem persists.';
          break;
        default:
          errorMessage = `Server Error (${error.status}): ${error.error?.message || error.message || 'Unknown error occurred'}`;
      }
    }
    
    console.error('DeskService Error:', {
      status: error.status,
      message: error.message,
      error: error.error
    });
    
    return throwError(() => new Error(errorMessage));
  };
}
