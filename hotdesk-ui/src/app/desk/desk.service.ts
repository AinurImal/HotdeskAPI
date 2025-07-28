import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Desk, CreateDeskRequest, UpdateDeskRequest, DeskAvailabilityResponse } from './desk.model';
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

  // GET - Check desk availability for a specific date with validation
  checkDeskAvailability(deskId: number, date: string): Observable<DeskAvailabilityResponse> {
    // Validate input parameters
    const validationError = this.validateAvailabilityRequest(deskId, date);
    if (validationError) {
      console.error('Desk availability validation failed:', validationError);
      return throwError(() => new Error(validationError));
    }

    // Format date properly for API (Monday morning standard format)
    const formattedDate = this.formatDateForApi(date);
    
    return this.http.get<DeskAvailabilityResponse>(`${this.apiUrl}/${deskId}/availability?date=${encodeURIComponent(formattedDate)}`)
      .pipe(
        catchError((error) => {
          console.warn(`Desk availability check failed for desk ${deskId} on ${formattedDate}:`, error);
          
          // Monday morning error handling - provide specific error messages
          if (error.status === 404) {
            return throwError(() => new Error('Desk not found or availability endpoint not available.'));
          } else if (error.status === 400) {
            return throwError(() => new Error('Invalid date format or desk ID provided.'));
          } else if (error.status === 422) {
            return throwError(() => new Error('Date validation failed on server side.'));
          }
          
          // For other errors, return system status fallback
          return of({ 
            deskId: deskId, 
            isAvailable: true, 
            message: 'API check failed, using system status' 
          } as DeskAvailabilityResponse);
        })
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

  // Monday morning validation methods for desk availability
  
  /**
   * Validate desk availability request parameters
   * @param deskId - The desk ID to validate
   * @param date - The date string to validate (YYYY-MM-DD format)
   * @returns Error message if validation fails, null if validation passes
   */
  private validateAvailabilityRequest(deskId: number, date: string): string | null {
    // Validate desk ID
    if (!deskId || !Number.isInteger(deskId) || deskId <= 0) {
      return 'Invalid desk ID. Desk ID must be a positive integer.';
    }

    // Validate date
    if (!date || typeof date !== 'string' || date.trim() === '') {
      return 'Date is required. Please select a valid date.';
    }

    // Validate date format (YYYY-MM-DD)
    const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateRegex.test(date)) {
      return 'Invalid date format. Please use YYYY-MM-DD format.';
    }

    // Validate that date is a valid date
    const parsedDate = new Date(date);
    if (isNaN(parsedDate.getTime())) {
      return 'Invalid date. Please select a valid date.';
    }

    // Business rule validation - cannot check availability for past dates
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    parsedDate.setHours(0, 0, 0, 0);
    
    if (parsedDate < today) {
      return 'Cannot check availability for past dates. Please select today or a future date.';
    }

    // Business rule validation - cannot check too far in the future (6 months max)
    const maxFutureDate = new Date();
    maxFutureDate.setMonth(maxFutureDate.getMonth() + 6);
    maxFutureDate.setHours(23, 59, 59, 999);
    
    if (parsedDate > maxFutureDate) {
      return 'Cannot check availability more than 6 months in advance. Please select a nearer date.';
    }

    return null; // No validation errors
  }

  /**
   * Format date for API call (Monday morning standard)
   * @param date - Date string in YYYY-MM-DD format
   * @returns Formatted date string for API (dd/MM/yyyy)
   */
  private formatDateForApi(date: string): string {
    try {
      const parsedDate = new Date(date);
      if (isNaN(parsedDate.getTime())) {
        throw new Error('Invalid date provided for formatting');
      }
      
      // Format as dd/MM/yyyy for API consistency
      const day = String(parsedDate.getDate()).padStart(2, '0');
      const month = String(parsedDate.getMonth() + 1).padStart(2, '0');
      const year = parsedDate.getFullYear();
      
      return `${day}/${month}/${year}`;
    } catch (error) {
      console.error('Date formatting error:', error);
      throw new Error('Failed to format date for API call');
    }
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
