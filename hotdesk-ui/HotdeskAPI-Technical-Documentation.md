# HotdeskAPI - Comprehensive Technical Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture Overview](#architecture-overview)
3. [Technology Stack](#technology-stack)
4. [Project Structure](#project-structure)
5. [Component Analysis](#component-analysis)
6. [Service Layer Architecture](#service-layer-architecture)
7. [Data Models & Interfaces](#data-models--interfaces)
8. [API Integration](#api-integration)
9. [Routing & Navigation](#routing--navigation)
10. [Global Templates & Styling](#global-templates--styling)
11. [Development Workflow](#development-workflow)
12. [Best Practices Implementation](#best-practices-implementation)
13. [Deployment & Configuration](#deployment--configuration)

---

## 1. Project Overview

**HotdeskAPI** is a comprehensive desk booking and management system built with modern web technologies. The application provides a complete solution for managing office hotdesks, including user management, desk availability tracking, booking functionality, and real-time availability checking.

### Key Features
- **Dashboard**: Real-time overview of desk availability and system statistics
- **User Management**: Create, read, update, and delete user accounts
- **Desk Management**: Complete CRUD operations for desk entities
- **Desk Availability**: Real-time availability checking with filtering capabilities
- **Booking System**: Create and manage desk bookings with duration types
- **BookFinder**: Advanced search functionality for finding bookings
- **Responsive Design**: Mobile-first approach with dashboard-style layouts

### Business Value
- Streamlines office space management
- Reduces desk booking conflicts
- Provides real-time availability data
- Enhances workplace efficiency
- Supports hybrid work arrangements

---

## 2. Architecture Overview

### Frontend Architecture (Angular 20)
```
┌─────────────────────────────────────────────────────────────┐
│                    Angular Frontend                         │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │ Components  │  │  Services   │  │   Models    │        │
│  │             │  │             │  │             │        │
│  │ • Home      │  │ • UserSvc   │  │ • User      │        │
│  │ • User      │  │ • DeskSvc   │  │ • Desk      │        │
│  │ • Desk      │  │ • BookSvc   │  │ • Booking   │        │
│  │ • Booking   │  │ • BookFind  │  │ • BookFind  │        │
│  │ • DeskAvail │  │             │  │             │        │
│  │ • BookFind  │  │             │  │             │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
│                            │                               │
│                    ┌───────▼───────┐                      │
│                    │  HTTP Client  │                      │
│                    │   (Proxy)     │                      │
│                    └───────────────┘                      │
└─────────────────────────────────────────────────────────────┘
                             │
                    ┌────────▼────────┐
                    │  CORS Proxy     │
                    │ /api → :5251    │
                    └─────────────────┘
                             │
┌─────────────────────────────▼───────────────────────────────┐
│                 HotdeskAPI Backend                          │
│                    .NET Core API                           │
│                   Port: 5251                               │
└─────────────────────────────────────────────────────────────┘
```

### Data Flow Architecture
```
User Interaction → Component → Service → HTTP Client → Proxy → HotdeskAPI
                                                                     │
Response Data ← Component ← Service ← HTTP Client ← Proxy ← ─────────┘
```

---

## 3. Technology Stack

### Frontend Technologies
- **Angular 20**: Latest version with standalone components
- **TypeScript 5.8.2**: Strong typing and modern JavaScript features
- **RxJS 7.8.0**: Reactive programming for async operations
- **HTML5 & CSS3**: Modern web standards
- **Express 5.1.0**: Development server and SSR support

### Build & Development Tools
- **Angular CLI 20.0.5**: Project scaffolding and build system
- **Prettier**: Code formatting
- **Karma & Jasmine**: Testing framework
- **Webpack**: Module bundling (via Angular CLI)
- **Zone.js**: Change detection framework

### Backend Integration
- **.NET Core API**: RESTful web services
- **JSON**: Data exchange format
- **CORS Proxy**: Development-time cross-origin handling

---

## 4. Project Structure

```
hotdesk-ui/
├── src/
│   ├── app/
│   │   ├── booking/                    # Booking management component
│   │   │   ├── booking.ts             # Component logic
│   │   │   ├── booking.html           # Template
│   │   │   ├── booking.css            # Component styles
│   │   │   ├── booking.service.ts     # API service
│   │   │   └── booking.model.ts       # Data models
│   │   │
│   │   ├── bookfinder/                # Booking search component
│   │   │   ├── bookfinder.ts          # Component logic
│   │   │   ├── bookfinder.html        # Template
│   │   │   ├── bookfinder.css         # Component styles
│   │   │   ├── bookfinder.service.ts  # API service
│   │   │   └── bookfinder.model.ts    # Data models
│   │   │
│   │   ├── desk/                      # Desk management component
│   │   │   ├── desk.ts                # Component logic
│   │   │   ├── desk.html              # Template
│   │   │   ├── desk.css               # Component styles
│   │   │   ├── desk.service.ts        # API service
│   │   │   └── desk.model.ts          # Data models
│   │   │
│   │   ├── desk-availability/         # Desk availability component
│   │   │   ├── desk-availability.ts   # Component logic
│   │   │   ├── desk-availability.html # Template
│   │   │   └── desk-availability.css  # Component styles
│   │   │
│   │   ├── home/                      # Dashboard component
│   │   │   ├── home.ts                # Component logic
│   │   │   ├── home.html              # Template
│   │   │   └── home.css               # Component styles
│   │   │
│   │   ├── user/                      # User management component
│   │   │   ├── user.ts                # Component logic
│   │   │   ├── user.html              # Template
│   │   │   ├── user.css               # Component styles
│   │   │   ├── user.service.ts        # API service
│   │   │   └── user.model.ts          # Data models
│   │   │
│   │   ├── app.ts                     # Root component
│   │   ├── app.html                   # Root template
│   │   ├── app.css                    # Global styles
│   │   ├── app.config.ts              # App configuration
│   │   ├── app.routes.ts              # Routing configuration
│   │   └── main.ts                    # Bootstrap entry point
│   │
│   ├── environments/
│   │   └── environment.ts             # Environment configuration
│   │
│   ├── index.html                     # Main HTML file
│   ├── main.ts                        # Application entry point
│   ├── server.ts                      # SSR server configuration
│   └── styles.css                     # Global styles
│
├── proxy.conf.js                      # Development proxy configuration
├── package.json                       # NPM dependencies
├── angular.json                       # Angular CLI configuration
├── tsconfig.json                      # TypeScript configuration
└── README.md                          # Project documentation
```

---

## 5. Component Analysis

### 5.1 Home Component (Dashboard)

**Purpose**: Main dashboard providing system overview and statistics

**Key Features**:
- Real-time statistics display
- Recent booking activity
- System status monitoring
- Navigation hub

**Component Structure**:
```typescript
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
  // Dashboard statistics
  totalDesks = 0;
  availableDesks = 0;
  totalBookings = 0;
  totalUsers = 0;
  
  // Recent activity data
  recentBookings: any[] = [];
  
  // System status
  apiStatus: 'online' | 'offline' = 'online';
  databaseStatus: 'online' | 'offline' = 'online';
  
  // Loading state
  isLoading = true;
}
```

**Data Flow**:
1. Component initializes and loads dashboard data
2. Parallel API calls to multiple services (DeskService, UserService, BookingService)
3. Statistics aggregated and displayed in dashboard cards
4. Recent activity fetched and displayed
5. Error handling for offline scenarios

**API Integration**:
- `DeskService.getDesks()` - Load desk statistics
- `UserService.getUsers()` - Load user statistics  
- `BookingService.getBookings()` - Load booking statistics and recent activity

### 5.2 User Component

**Purpose**: Complete user management with CRUD operations

**Key Features**:
- User listing with search and filtering
- Create new users with validation
- Edit existing user information
- Delete users with confirmation
- Real-time form validation

**Component Structure**:
```typescript
@Component({
  selector: 'app-user',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './user.html',
  styleUrl: './user.css'
})
export class UserComponent implements OnInit {
  // Array to store all users fetched from API
  users: UserModel[] = [];
  
  // Currently selected user for editing
  selectedUser: UserModel | null = null;
  
  // Form data objects
  newUser: CreateUserRequest = {
    fullName: '', userName: '', phoneNumber: '', email: ''
  };
  
  editUser: UpdateUserRequest = {
    userId: '', fullName: '', userName: '', phoneNumber: '', email: ''
  };
  
  // Component state
  isLoading = false;
  errorMessage = '';
  isEditing = false;
}
```

**Form Handling**:
- Template-driven forms with two-way data binding
- Real-time validation for required fields
- Email format validation
- Phone number format validation

**API Integration**:
- `UserService.getUsers()` - Load all users
- `UserService.createUser()` - Create new user
- `UserService.updateUser()` - Update existing user
- `UserService.deleteUser()` - Delete user
- `UserService.getUserById()` - Get specific user details

### 5.3 Desk Component

**Purpose**: Comprehensive desk management system

**Key Features**:
- Desk inventory management
- Create/Edit/Delete desk operations
- Availability status management
- Monitor equipment tracking
- Location-based organization

**Component Structure**:
```typescript
@Component({
  selector: 'app-desk',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './desk.html',
  styleUrl: './desk.css'
})
export class DeskComponent implements OnInit {
  // Reactive form for desk management
  deskForm: FormGroup;
  
  // Desk data storage
  desks: Desk[] = [];
  
  // Component state management
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  editingDeskId: number | null = null;
}
```

**Form Validation**:
```typescript
private createForm(): FormGroup {
  return this.fb.group({
    deskId: [''],
    name: ['', [Validators.required, Validators.minLength(1)]],
    location: ['', [Validators.required, Validators.minLength(2)]],
    isAvailable: [true],
    hasMonitor: [false],
    description: ['']
  });
}
```

**API Integration**:
- `DeskService.getDesks()` - Load all desks
- `DeskService.createDesk()` - Create new desk
- `DeskService.updateDesk()` - Update existing desk
- `DeskService.deleteDesk()` - Delete desk

### 5.4 Desk Availability Component

**Purpose**: Real-time desk availability tracking with advanced filtering

**Key Features**:
- Date-based availability checking
- Location filtering
- Monitor equipment filtering
- Search functionality
- Dashboard-style statistics display

**Component Structure**:
```typescript
@Component({
  selector: 'app-desk-availability',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './desk-availability.html',
  styleUrl: './desk-availability.css'
})
export class DeskAvailabilityComponent implements OnInit {
  // Filter properties
  selectedDate: string = '';
  searchTerm: string = '';
  selectedLocation: string = '';
  showMonitorOnly: boolean = false;
  
  // Data storage
  desks: Desk[] = [];
  availableDesks: Desk[] = [];
  unavailableDesks: Desk[] = [];
  locations: string[] = [];
  
  // Component state
  isLoading = false;
  errorMessage = '';
  successMessage = '';
}
```

**Filter Logic**:
```typescript
getFilteredDesks(desks: Desk[]): Desk[] {
  return desks.filter(desk => {
    // Search filter validation
    const searchMatch = !this.searchTerm || 
      (desk.name && desk.name.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
      (desk.description && desk.description.toLowerCase().includes(this.searchTerm.toLowerCase()));
    
    // Location filter validation
    const locationMatch = !this.selectedLocation || 
      (desk.location && desk.location === this.selectedLocation);
    
    // Monitor filter validation
    const monitorMatch = !this.showMonitorOnly || 
      (typeof desk.hasMonitor === 'boolean' && desk.hasMonitor);
    
    return searchMatch && locationMatch && monitorMatch;
  });
}
```

**API Integration**:
- `DeskService.getDesks()` - Load all desks
- `DeskService.checkDeskAvailability()` - Check specific desk availability

### 5.5 Booking Component

**Purpose**: Complete booking management system

**Key Features**:
- Create new bookings with validation
- Edit existing bookings
- Duration type selection (daily, weekly, monthly)
- Check-in/check-out functionality
- Desk selection with availability filtering

**Component Structure**:
```typescript
@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './booking.html',
  styleUrl: './booking.css'
})
export class BookingComponent implements OnInit {
  // Reactive form for booking management
  bookingForm: FormGroup;
  
  // Data storage
  bookings: Booking[] = [];
  desks: Desk[] = [];
  
  // Component state
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  editingBookingId: number | null = null;
  
  // Configuration
  durationTypes = DURATION_TYPES;
}
```

**Form Validation**:
```typescript
private createForm(): FormGroup {
  return this.fb.group({
    bookingId: [''],
    deskId: ['', [Validators.required]],
    userName: ['', [Validators.required, Validators.minLength(2)]],
    bookingDate: ['', [Validators.required]],
    durationType: ['daily', [Validators.required]],
    checkedIn: [false],
    checkInTime: ['']
  });
}
```

**API Integration**:
- `BookingService.getBookings()` - Load all bookings
- `BookingService.createBooking()` - Create new booking
- `BookingService.updateBooking()` - Update existing booking
- `BookingService.deleteBooking()` - Delete booking
- `DeskService.getDesks()` - Load available desks

### 5.6 BookFinder Component

**Purpose**: Advanced search and filtering for booking management

**Key Features**:
- Multi-criteria search functionality
- Pagination support
- Export capabilities
- Advanced filtering options
- Search type selection (user, desk, date)

**Component Structure**:
```typescript
@Component({
  selector: 'app-bookfinder',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './bookfinder.html',
  styleUrl: './bookfinder.css'
})
export class BookFinderComponent implements OnInit {
  // Reactive search form
  searchForm: FormGroup;
  
  // Data storage
  bookFinders: BookFinder[] = [];
  allBookFinders: BookFinder[] = [];
  
  // Pagination properties
  currentPage = 1;
  pageSize = 10;
  totalItems = 0;
  totalPages = 0;
  
  // Search configuration
  searchTypes = SEARCH_TYPES;
  
  // Component state
  isLoading = false;
  errorMessage = '';
  hasSearched = false;
}
```

**Search Implementation**:
```typescript
onSearch(): void {
  if (this.searchForm.valid) {
    const searchRequest: BookFinderSearchRequest = {
      searchBy: this.searchForm.value.searchBy,
      searchValue: this.searchForm.value.searchValue.trim(),
      dateFrom: this.searchForm.value.dateFrom,
      dateTo: this.searchForm.value.dateTo
    };
    
    this.bookFinderService.searchBookings(searchRequest).subscribe({
      next: (results) => {
        this.bookFinders = results;
        this.allBookFinders = [...results];
        this.hasSearched = true;
        this.updatePagination();
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }
}
```

**API Integration**:
- `BookFinderService.getAllBookings()` - Load all bookings
- `BookFinderService.searchBookings()` - Search with criteria
- `BookFinderService.deleteBooking()` - Delete booking

---

## 6. Service Layer Architecture

### 6.1 Service Design Pattern

All services follow a consistent pattern:
- Injectable singleton pattern
- HttpClient dependency injection
- Observable-based async operations
- Centralized error handling
- Type-safe API communication

### 6.2 UserService

**Purpose**: Handles all user-related API operations

```typescript
@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly apiUrl = `${environment.apiUrl}/Users`;

  constructor(private http: HttpClient) {}

  // GET - Retrieve all users
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  // POST - Create new user
  createUser(user: CreateUserRequest): Observable<User> {
    return this.http.post<User>(this.apiUrl, user)
      .pipe(catchError(this.handleError));
  }

  // PUT - Update existing user
  updateUser(user: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${user.userId}`, user)
      .pipe(catchError(this.handleError));
  }

  // DELETE - Remove user
  deleteUser(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }
}
```

**Error Handling Strategy**:
```typescript
private handleError = (error: HttpErrorResponse): Observable<never> => {
  let errorMessage = 'An unexpected error occurred.';
  
  if (error.error instanceof ErrorEvent) {
    // Client-side error
    errorMessage = `Network Error: ${error.error.message}`;
  } else {
    // Server-side error
    switch (error.status) {
      case 0: errorMessage = 'Connection Error: Unable to reach server'; break;
      case 400: errorMessage = 'Invalid Data: Please check your input'; break;
      case 404: errorMessage = 'User Not Found: User may have been deleted'; break;
      case 409: errorMessage = 'Duplicate Entry: User already exists'; break;
      case 500: errorMessage = 'Server Error: Please try again later'; break;
    }
  }
  
  return throwError(() => new Error(errorMessage));
};
```

### 6.3 DeskService

**Purpose**: Manages desk-related operations with advanced availability checking

**Key Features**:
- CRUD operations for desks
- Real-time availability checking
- Monday morning validation framework
- Business rule enforcement

```typescript
@Injectable({
  providedIn: 'root'
})
export class DeskService {
  private readonly apiUrl = `${environment.apiUrl}/Desks`;

  // Availability checking with validation
  checkDeskAvailability(deskId: number, date: string): Observable<DeskAvailabilityResponse> {
    const validationError = this.validateAvailabilityRequest(deskId, date);
    if (validationError) {
      return throwError(() => new Error(validationError));
    }

    const formattedDate = this.formatDateForApi(date);
    
    return this.http.get<DeskAvailabilityResponse>(
      `${this.apiUrl}/${deskId}/availability?date=${encodeURIComponent(formattedDate)}`
    ).pipe(
      catchError((error) => {
        // Specific error handling for availability checks
        if (error.status === 404) {
          return throwError(() => new Error('Desk not found'));
        }
        // Fallback response for failed checks
        return of({ 
          deskId, 
          isAvailable: true, 
          message: 'API check failed, using system status' 
        });
      })
    );
  }
}
```

**Validation Framework**:
```typescript
private validateAvailabilityRequest(deskId: number, date: string): string | null {
  // Desk ID validation
  if (!deskId || !Number.isInteger(deskId) || deskId <= 0) {
    return 'Invalid desk ID. Must be a positive integer.';
  }

  // Date format validation
  const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
  if (!dateRegex.test(date)) {
    return 'Invalid date format. Use YYYY-MM-DD format.';
  }

  // Business rule validation
  const parsedDate = new Date(date);
  const today = new Date();
  
  if (parsedDate < today) {
    return 'Cannot check availability for past dates.';
  }

  return null; // Valid
}
```

### 6.4 BookingService

**Purpose**: Complete booking lifecycle management

```typescript
@Injectable({
  providedIn: 'root'
})
export class BookingService {
  private readonly apiUrl = '/api/Bookings';

  // Comprehensive booking operations
  getBookings(): Observable<Booking[]> { /* ... */ }
  getBookingsByUserName(userName: string): Observable<Booking[]> { /* ... */ }
  getBookingsByDeskId(deskId: number): Observable<Booking[]> { /* ... */ }
  getBookingsByDate(date: Date): Observable<Booking[]> { /* ... */ }
  createBooking(booking: CreateBookingRequest): Observable<Booking> { /* ... */ }
  updateBooking(booking: UpdateBookingRequest): Observable<Booking> { /* ... */ }
  deleteBooking(id: number): Observable<void> { /* ... */ }
}
```

### 6.5 BookFinderService

**Purpose**: Advanced search and filtering capabilities

```typescript
@Injectable({
  providedIn: 'root'
})
export class BookFinderService {
  private readonly apiUrl = '/api/BookFinder';

  // Advanced search functionality
  searchBookings(request: BookFinderSearchRequest): Observable<BookFinder[]> {
    let params = new HttpParams()
      .set('searchBy', request.searchBy)
      .set('searchValue', request.searchValue);

    if (request.dateFrom) {
      params = params.set('dateFrom', request.dateFrom);
    }
    if (request.dateTo) {
      params = params.set('dateTo', request.dateTo);
    }

    return this.http.get<BookFinder[]>(this.apiUrl, { params })
      .pipe(catchError(this.handleError));
  }
}
```

---

## 7. Data Models & Interfaces

### 7.1 Type Safety Architecture

The application uses comprehensive TypeScript interfaces for type safety:

### 7.2 User Models

```typescript
// Complete user entity
export interface User {
  userId?: string;      // Optional auto-generated ID
  fullName: string;     // Required full name
  userName: string;     // Required unique username  
  phoneNumber: string;  // Required phone number
  email: string;        // Required email address
  createdDate?: Date;   // Optional creation timestamp
  updatedDate?: Date;   // Optional update timestamp
}

// New user creation
export interface CreateUserRequest {
  fullName: string;     // Required for creation
  userName: string;     // Required for creation
  phoneNumber: string;  // Required for creation
  email: string;        // Required for creation
}

// User update operations
export interface UpdateUserRequest {
  userId: string;       // Required for identification
  fullName: string;     // Updated full name
  userName: string;     // Updated username
  phoneNumber: string;  // Updated phone number
  email: string;        // Updated email address
}
```

### 7.3 Desk Models

```typescript
// Complete desk entity
export interface Desk {
  deskId?: number;      // Optional auto-generated ID
  name: string;         // Required desk identifier
  location: string;     // Required location
  hasMonitor: boolean;  // Required monitor availability
  isAvailable: boolean; // Required availability status
  description?: string; // Optional description
  createdDate?: Date;   // Optional creation timestamp
  updatedDate?: Date;   // Optional update timestamp
}

// Desk availability response
export interface DeskAvailabilityResponse {
  deskId: number;       // Desk identifier
  date: string;         // Check date
  isAvailable: boolean; // Availability status
  message: string;      // Status message
}
```

### 7.4 Booking Models

```typescript
// Complete booking entity
export interface Booking {
  bookingId?: number;     // Optional auto-generated ID
  deskId: number;         // Required desk reference
  userName: string;       // Required user reference
  bookingDate: Date;      // Required booking date
  durationType: string;   // Required duration (daily/weekly/monthly)
  checkedIn: boolean;     // Required check-in status
  checkInTime?: string;   // Optional check-in time
  createdDate?: Date;     // Optional creation timestamp
  updatedDate?: Date;     // Optional update timestamp
}

// Duration type constants
export const DURATION_TYPES = [
  { value: 'daily', label: 'Daily (8 hours)' },
  { value: 'weekly', label: 'Weekly (5 days)' },
  { value: 'monthly', label: 'Monthly (22 days)' }
];
```

### 7.5 BookFinder Models

```typescript
// BookFinder search result
export interface BookFinder {
  bookingId: number;        // Booking identifier
  deskId: number;           // Desk identifier
  deskName: string;         // Desk name for display
  deskLocation: string;     // Desk location
  userName: string;         // User name
  userEmail: string;        // User email
  bookingDate: Date;        // Booking date
  durationType: string;     // Duration type
  checkedIn: boolean;       // Check-in status
  checkInTime?: string;     // Check-in time
  status: string;           // Booking status
}

// Search request structure
export interface BookFinderSearchRequest {
  searchBy: string;         // Search criteria type
  searchValue: string;      // Search value
  dateFrom?: string;        // Optional date range start
  dateTo?: string;          // Optional date range end
}

// Search type constants
export const SEARCH_TYPES = [
  { value: 'user', label: 'Search by User Name' },
  { value: 'desk', label: 'Search by Desk Name' },
  { value: 'location', label: 'Search by Location' },
  { value: 'date', label: 'Search by Date Range' }
];
```

---

## 8. API Integration

### 8.1 Backend API Structure

The HotdeskAPI backend provides RESTful endpoints:

```
Base URL: http://localhost:5251/api

Endpoints:
├── /api/Users                    # User management
│   ├── GET    /                  # Get all users
│   ├── GET    /{id}              # Get user by ID
│   ├── POST   /                  # Create new user
│   ├── PUT    /{id}              # Update user
│   └── DELETE /{id}              # Delete user
│
├── /api/Desks                    # Desk management
│   ├── GET    /                  # Get all desks
│   ├── GET    /{id}              # Get desk by ID
│   ├── GET    /available         # Get available desks
│   ├── GET    /{id}/availability # Check availability
│   ├── POST   /                  # Create new desk
│   ├── PUT    /{id}              # Update desk
│   └── DELETE /{id}              # Delete desk
│
├── /api/Bookings                 # Booking management
│   ├── GET    /                  # Get all bookings
│   ├── GET    /{id}              # Get booking by ID
│   ├── GET    /user/{userName}   # Get by user
│   ├── GET    /desk/{deskId}     # Get by desk
│   ├── GET    /date/{date}       # Get by date
│   ├── POST   /                  # Create booking
│   ├── PUT    /{id}              # Update booking
│   └── DELETE /{id}              # Delete booking
│
└── /api/BookFinder               # Search functionality
    ├── GET    /                  # Get all results
    ├── GET    /search            # Search with criteria
    └── DELETE /{id}              # Delete booking
```

### 8.2 CORS & Proxy Configuration

**Development Proxy** (`proxy.conf.js`):
```javascript
const PROXY_CONFIG = [
  {
    context: ['/api/**'],
    target: 'http://localhost:5251',
    secure: false,
    changeOrigin: true,
    logLevel: 'debug',
    onProxyReq: function(proxyReq, req, res) {
      console.log('Proxy Request:', req.method, req.url);
    },
    onProxyRes: function(proxyRes, req, res) {
      console.log('Proxy Response:', proxyRes.statusCode, req.url);
      // Add CORS headers
      res.setHeader('Access-Control-Allow-Origin', '*');
      res.setHeader('Access-Control-Allow-Methods', 'GET, POST, PUT, DELETE, OPTIONS');
      res.setHeader('Access-Control-Allow-Headers', 'Content-Type, Authorization');
    }
  }
];
```

**Environment Configuration**:
```typescript
export const environment = {
  production: false,
  apiUrl: '/api'  // Uses proxy for CORS handling
};
```

### 8.3 HTTP Client Configuration

**App Configuration** (`app.config.ts`):
```typescript
export const appConfig: ApplicationConfig = {
  providers: [
    // Modern HTTP client with fetch API
    provideHttpClient(withFetch(), withInterceptorsFromDi()),
    
    // Global error handling
    provideBrowserGlobalErrorListeners(),
    
    // Routing configuration
    provideRouter(routes),
    
    // SSR support
    provideClientHydration(withEventReplay())
  ]
};
```

### 8.4 Request/Response Flow

**Typical API Call Flow**:
```typescript
// 1. Component initiates API call
loadUsers(): void {
  this.isLoading = true;
  this.errorMessage = '';
  
  // 2. Service makes HTTP request
  this.userService.getUsers().subscribe({
    next: (users) => {
      // 3. Success handling
      this.users = users;
      this.isLoading = false;
    },
    error: (error) => {
      // 4. Error handling
      this.errorMessage = error.message;
      this.isLoading = false;
    }
  });
}
```

**Service Layer Implementation**:
```typescript
getUsers(): Observable<User[]> {
  return this.http.get<User[]>(this.apiUrl)
    .pipe(
      catchError(this.handleError)  // Centralized error handling
    );
}
```

---

## 9. Routing & Navigation

### 9.1 Route Configuration

**Application Routes** (`app.routes.ts`):
```typescript
export const routes: Routes = [
  { path: '', component: Home, pathMatch: 'full', title: 'Hotdesk - Home' },
  { path: 'home', component: Home, title: 'Hotdesk - Home' },
  { path: 'user', component: UserComponent, title: 'Hotdesk - User Management' },
  { path: 'desk', component: DeskComponent, title: 'Hotdesk - Desk Management' },
  { path: 'desk-availability', component: DeskAvailabilityComponent, title: 'Hotdesk - Desk Availability' },
  { path: 'booking', component: BookingComponent, title: 'Hotdesk - Booking Management' },
  { path: 'bookfinder', component: BookFinderComponent, title: 'Hotdesk - Booking Finder' }
];
```

### 9.2 Navigation Structure

**Sidebar Navigation Template**:
```html
<nav class="sidebar">
  <div class="sidebar-header">
    <span class="sidebar-title">Hotdesk</span>
  </div>
  <ul>
    <li><a routerLink="/home" routerLinkActive="active">🏠 Home</a></li>
    <li><a routerLink="/user" routerLinkActive="active">👤 Users</a></li>
    
    <!-- Expandable Desk Navigation -->
    <li class="nav-item-expandable">
      <div class="nav-header" (click)="toggleDeskNav()">
        <span class="nav-link">🪑 Desks</span>
        <span class="expand-icon" [class.expanded]="isDeskNavExpanded">▼</span>
      </div>
      <ul *ngIf="isDeskNavExpanded" class="sub-nav">
        <li><a routerLink="/desk" routerLinkActive="active">⚙️ Desk Management</a></li>
        <li><a routerLink="/desk-availability" routerLinkActive="active">📊 Desk Availability</a></li>
      </ul>
    </li>
    
    <li><a routerLink="/booking" routerLinkActive="active">📅 Bookings</a></li>
    <li><a routerLink="/bookfinder" routerLinkActive="active">🔍 Booking Finder</a></li>
  </ul>
</nav>
```

### 9.3 Navigation State Management

**Component Navigation Logic**:
```typescript
export class UserComponent implements OnInit {
  // Navigation expansion state
  isDeskNavExpanded = false;

  // Toggle method
  toggleDeskNav(): void {
    this.isDeskNavExpanded = !this.isDeskNavExpanded;
  }
}
```

### 9.4 Route Guards & Protection

Currently using basic routing without guards, but structure supports:
- Authentication guards
- Authorization guards
- Data preloading
- Route deactivation guards

---

## 10. Global Templates & Styling

### 10.1 Layout Architecture

**Consistent Layout Pattern**:
```html
<div class="home-layout">
  <nav class="sidebar">
    <!-- Navigation content -->
  </nav>
  <main class="content-area">
    <!-- Page content -->
  </main>
</div>
```

### 10.2 CSS Architecture

**Global Styles** (`styles.css`):
```css
/* Reset and base styles */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background: #f5f5f5;
  color: #333;
  line-height: 1.6;
}

/* Global utility classes */
.loading { /* ... */ }
.error-message { /* ... */ }
.success-message { /* ... */ }
```

**Component-Specific Styling**:
Each component has its own CSS file with scoped styles.

### 10.3 Design System

**Color Palette**:
```css
:root {
  --primary-color: #232946;      /* Dark navy blue */
  --secondary-color: #eebbc3;    /* Light pink */
  --accent-color: #b8c1ec;       /* Light blue */
  --success-color: #28a745;      /* Green */
  --warning-color: #ffc107;      /* Yellow */
  --danger-color: #dc3545;       /* Red */
  --light-gray: #f8f9fa;         /* Light background */
  --medium-gray: #6c757d;        /* Medium text */
  --dark-gray: #495057;          /* Dark text */
}
```

**Typography System**:
```css
h1 { font-size: 2.5rem; font-weight: 700; }
h2 { font-size: 2rem; font-weight: 600; }
h3 { font-size: 1.5rem; font-weight: 600; }
body { font-size: 1rem; line-height: 1.6; }
small { font-size: 0.875rem; }
```

**Button System**:
```css
.btn {
  padding: 8px 16px;
  border-radius: 4px;
  font-weight: 500;
  transition: all 0.3s ease;
}

.btn-primary { background: var(--primary-color); color: white; }
.btn-secondary { background: var(--medium-gray); color: white; }
.btn-outline { background: transparent; border: 1px solid var(--primary-color); }
```

### 10.4 Dashboard Card System

**Card Component Pattern**:
```css
.stat-card {
  background: linear-gradient(135deg, #fff 0%, #f8f9fa 100%);
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
  transition: transform 0.2s ease;
}

.stat-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0,0,0,0.08);
}
```

### 10.5 Form Styling

**Consistent Form Controls**:
```css
.form-control {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.form-control:focus {
  outline: none;
  border-color: var(--primary-color);
  box-shadow: 0 0 0 2px rgba(35, 41, 70, 0.1);
}
```

### 10.6 Responsive Design

**Breakpoint System**:
```css
/* Mobile First Approach */
/* Base styles for mobile */

@media (min-width: 768px) {
  /* Tablet styles */
}

@media (min-width: 1024px) {
  /* Desktop styles */
}

@media (min-width: 1200px) {
  /* Large desktop styles */
}
```

**Grid Systems**:
```css
.stats-section {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
}

@media (max-width: 768px) {
  .stats-section {
    grid-template-columns: 1fr;
  }
}
```

---

## 11. Development Workflow

### 11.1 Development Environment Setup

**Prerequisites**:
- Node.js 18+ 
- Angular CLI 20+
- HotdeskAPI running on port 5251

**Setup Steps**:
```bash
# 1. Install dependencies
npm install

# 2. Start development server with proxy
npm start

# 3. Development server runs on http://localhost:4200
# 4. API calls proxy to http://localhost:5251
```

### 11.2 Build Process

**Development Build**:
```bash
ng serve --proxy-config proxy.conf.js
```

**Production Build**:
```bash
ng build --configuration production
```

**SSR Build**:
```bash
ng build --ssr
npm run serve:ssr:hotdesk-ui
```

### 11.3 Testing Strategy

**Unit Testing**:
```bash
ng test
```

**E2E Testing**:
- Karma + Jasmine configuration
- Component testing
- Service testing
- Integration testing

### 11.4 Code Quality

**TypeScript Configuration**:
```json
{
  "compilerOptions": {
    "strict": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true
  }
}
```

**Prettier Configuration**:
```json
{
  "overrides": [
    {
      "files": "*.html",
      "options": {
        "parser": "angular"
      }
    }
  ]
}
```

---

## 12. Best Practices Implementation

### 12.1 Angular Best Practices

**Standalone Components**:
- Modern Angular 20 approach
- Reduced bundle size
- Simplified dependency management

**Reactive Programming**:
- RxJS Observables for async operations
- Proper error handling with catchError
- Memory leak prevention with unsubscribe

**Type Safety**:
- Comprehensive TypeScript interfaces
- Strict typing configuration
- Compile-time error checking

### 12.2 Code Organization

**Single Responsibility Principle**:
- Each component has one clear purpose
- Services handle specific business logic
- Models define data contracts

**Separation of Concerns**:
- Components handle UI logic
- Services handle data operations
- Models define data structures

### 12.3 Error Handling Strategy

**Centralized Error Handling**:
```typescript
private handleError = (error: HttpErrorResponse): Observable<never> => {
  // Specific error messages based on HTTP status
  // Logging for debugging
  // User-friendly error messages
  return throwError(() => new Error(errorMessage));
};
```

**Component-Level Error Display**:
```html
<div *ngIf="errorMessage" class="error-message">
  <div class="error-icon">❌</div>
  <div class="error-content">
    <h4>Operation Failed</h4>
    <p>{{ errorMessage }}</p>
  </div>
</div>
```

### 12.4 Performance Optimization

**Lazy Loading**:
- Route-based code splitting (ready for implementation)
- Component lazy loading

**Change Detection**:
- OnPush strategy for performance-critical components
- Zone.js optimization with event coalescing

**Bundle Optimization**:
- Tree shaking with modern build tools
- Modern fetch API instead of XMLHttpRequest

### 12.5 Security Implementation

**Input Validation**:
- Client-side form validation
- Type safety with TypeScript
- Sanitization of user inputs

**HTTP Security**:
- HTTPS in production
- CORS handling through proxy
- XSS protection with Angular's built-in sanitization

---

## 13. Deployment & Configuration

### 13.1 Environment Configuration

**Development Environment**:
```typescript
export const environment = {
  production: false,
  apiUrl: '/api'  // Proxy configuration
};
```

**Production Environment**:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.yourdomain.com/api'  // Direct API URL
};
```

### 13.2 Build Configurations

**Angular CLI Configuration** (`angular.json`):
```json
{
  "configurations": {
    "production": {
      "budgets": [
        {
          "type": "initial",
          "maximumWarning": "500kb",
          "maximumError": "1mb"
        }
      ],
      "optimization": true,
      "outputHashing": "all",
      "sourceMap": false,
      "namedChunks": false,
      "aot": true,
      "extractLicenses": true,
      "vendorChunk": false,
      "buildOptimizer": true
    }
  }
}
```

### 13.3 Server-Side Rendering

**SSR Configuration** (`server.ts`):
```typescript
import { AngularNodeAppEngine, createNodeRequestHandler } from '@angular/ssr/node';
import express from 'express';

const app = express();
const angularApp = new AngularNodeAppEngine();

// Serve static files
app.use(express.static(browserDistFolder));

// Handle all routes with Angular Universal
app.use('/', (req, res, next) => {
  angularApp.handle(req)
    .then(response => response ? writeResponseToNodeResponse(response, res) : next())
    .catch(next);
});
```

### 13.4 Production Deployment Steps

1. **Build Production Bundle**:
   ```bash
   ng build --configuration production
   ```

2. **Deploy Static Files**:
   - Upload `dist/` folder to web server
   - Configure server to serve `index.html` for all routes

3. **API Configuration**:
   - Update environment.ts with production API URL
   - Configure CORS on production API server

4. **SSL/HTTPS Setup**:
   - Configure SSL certificates
   - Update security headers

### 13.5 Monitoring & Logging

**Error Tracking**:
- Browser console logging
- Error boundary implementation
- Performance monitoring

**Analytics**:
- User interaction tracking
- Performance metrics
- Error rate monitoring

---

## Conclusion

The HotdeskAPI Angular application demonstrates modern web development practices with:

- **Scalable Architecture**: Component-based design with clear separation of concerns
- **Type Safety**: Comprehensive TypeScript implementation
- **Modern Angular**: Latest Angular 20 features with standalone components
- **Robust API Integration**: RESTful API communication with error handling
- **Responsive Design**: Mobile-first approach with dashboard-style layouts
- **Professional UI/UX**: Consistent design system and user experience
- **Development Best Practices**: Code organization, testing, and deployment strategies

This documentation provides a complete technical reference for understanding, maintaining, and extending the HotdeskAPI application. The modular architecture supports future enhancements and scaling requirements while maintaining code quality and performance standards.

---

**Document Version**: 1.0  
**Last Updated**: July 29, 2025  
**Created By**: Technical Documentation Generator  
**Project**: HotdeskAPI Angular Frontend
