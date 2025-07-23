import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Booking, CreateBookingRequest, UpdateBookingRequest, BookingAvailabilityRequest } from './booking.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private readonly apiUrl = `${environment.apiUrl}/bookings`;

  constructor(private http: HttpClient) {}

  // GET - Retrieve all bookings
  getBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(this.apiUrl)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET - Retrieve bookings by user ID
  getBookingsByUserId(userId: number): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/user/${userId}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET - Retrieve bookings by desk ID
  getBookingsByDeskId(deskId: number): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/desk/${deskId}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET - Retrieve bookings by date
  getBookingsByDate(date: Date): Observable<Booking[]> {
    const dateStr = date.toISOString().split('T')[0];
    return this.http.get<Booking[]>(`${this.apiUrl}/date/${dateStr}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // GET - Retrieve a single booking by ID
  getBookingById(id: number): Observable<Booking> {
    return this.http.get<Booking>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // POST - Create a new booking
  createBooking(booking: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(this.apiUrl, booking)
      .pipe(
        catchError(this.handleError)
      );
  }

  // PUT - Update an existing booking
  updateBooking(booking: UpdateBookingRequest): Observable<Booking> {
    return this.http.put<Booking>(`${this.apiUrl}/${booking.id}`, booking)
      .pipe(
        catchError(this.handleError)
      );
  }

  // DELETE - Delete a booking by ID
  deleteBooking(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // POST - Check availability for a booking
  checkAvailability(request: BookingAvailabilityRequest): Observable<boolean> {
    return this.http.post<boolean>(`${this.apiUrl}/check-availability`, request)
      .pipe(
        catchError(this.handleError)
      );
  }

  // PUT - Check in to a booking
  checkIn(bookingId: number): Observable<Booking> {
    return this.http.put<Booking>(`${this.apiUrl}/${bookingId}/checkin`, {})
      .pipe(
        catchError(this.handleError)
      );
  }

  // PUT - Cancel a booking
  cancelBooking(bookingId: number): Observable<Booking> {
    return this.http.put<Booking>(`${this.apiUrl}/${bookingId}/cancel`, {})
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
