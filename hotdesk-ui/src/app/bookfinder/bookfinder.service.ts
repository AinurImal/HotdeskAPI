import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { BookFinder, BookFinderSearchRequest } from './bookfinder.model';

@Injectable({
  providedIn: 'root'
})
export class BookFinderService {
  private apiUrl = '/api/BookFinders';

  constructor(private http: HttpClient) { }

  /**
   * Gets all bookings with user and desk information
   */
  getAllBookFinders(): Observable<BookFinder[]> {
    return this.http.get<BookFinder[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Searches for bookings based on criteria (desk ID, user name, or booking ID)
   */
  searchBookFinders(searchRequest: BookFinderSearchRequest): Observable<BookFinder[]> {
    const params = new HttpParams().set('searchBy', searchRequest.searchBy);
    
    return this.http.get<BookFinder[]>(`${this.apiUrl}/${searchRequest.id}`, { params }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Deletes a booking by booking ID
   */
  deleteBooking(bookingId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${bookingId}`).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Handles HTTP errors and provides user-friendly error messages
   */
  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unexpected error occurred. Please try again later.';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 400:
          errorMessage = error.error?.message || 'Invalid request. Please check your input.';
          break;
        case 404:
          errorMessage = error.error?.message || 'No bookings found for the search criteria.';
          break;
        case 500:
          errorMessage = 'Server error. Please try again later.';
          break;
        default:
          errorMessage = error.error?.message || `Server returned code: ${error.status}`;
      }
    }
    
    console.error('BookFinder Service Error:', error);
    return throwError(() => errorMessage);
  }
}
