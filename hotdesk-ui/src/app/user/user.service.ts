// Import Angular core dependency injection decorator
import { Injectable } from '@angular/core';
// Import HTTP client for making API requests and error response handling
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
// Import RxJS observables for asynchronous data handling
import { Observable, throwError } from 'rxjs';
// Import RxJS operator for error handling in observable streams
import { catchError } from 'rxjs/operators';
// Import TypeScript interfaces for type safety
import { User, CreateUserRequest, UpdateUserRequest } from './user.model';
// Import environment configuration for API URL
import { environment } from '../../environments/environment';

// Injectable decorator makes this service available for dependency injection
@Injectable({
  providedIn: 'root'  // Service is registered at root level (singleton)
})
export class UserService {
  // Private readonly property storing the complete API endpoint URL
  private readonly apiUrl = `${environment.apiUrl}/Users`; // Note: Capital 'U' to match your API

  // Constructor injection of Angular's HttpClient service
  constructor(private http: HttpClient) {}

  // GET - Retrieve all users from the API
  getUsers(): Observable<User[]> {
    // Make HTTP GET request and return observable of User array
    return this.http.get<User[]>(this.apiUrl)
      .pipe(
        // Pipe the response through error handling
        catchError(this.handleError)
      );
  }

  // GET - Retrieve a single user by their unique ID
  getUserById(id: string): Observable<User> {
    // Make HTTP GET request to specific user endpoint with ID parameter
    return this.http.get<User>(`${this.apiUrl}/${id}`)
      .pipe(
        // Handle any errors that occur during the request
        catchError(this.handleError)
      );
  }

  // POST - Create a new user in the system
  createUser(user: CreateUserRequest): Observable<User> {
    // Make HTTP POST request with user data in request body
    return this.http.post<User>(this.apiUrl, user)
      .pipe(
        // Handle any errors that occur during user creation
        catchError(this.handleError)
      );
  }

  // PUT - Update an existing user's information
  updateUser(user: UpdateUserRequest): Observable<User> {
    // Make HTTP PUT request to update user, userId in URL and full user object in body
    return this.http.put<User>(`${this.apiUrl}/${user.userId}`, user)
      .pipe(
        // Handle any errors that occur during user update
        catchError(this.handleError)
      );
  }

  // DELETE - Remove a user from the system permanently
  deleteUser(id: string): Observable<void> {
    // Make HTTP DELETE request to remove user by ID, returns void on success
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(
        // Handle any errors that occur during user deletion
        catchError(this.handleError)
      );
  }

  // Private method for centralized error handling across all HTTP requests
  private handleError(error: HttpErrorResponse): Observable<never> {
    // Initialize default error message
    let errorMessage = 'An unknown error occurred!';
    
    // Check if error is client-side (network, parsing, etc.)
    if (error.error instanceof ErrorEvent) {
      // Client-side error - network issues, parsing errors, etc.
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      // Server-side error - API returned error status code
      errorMessage = `Server Error Code: ${error.status}\nMessage: ${error.message}`;
    }
    
    // Log error to console for debugging purposes
    console.error(errorMessage);
    // Return observable that immediately emits an error
    return throwError(() => errorMessage);
  }
}
