# Angular Web API Integration Documentation

## Overview
This project demonstrates a complete Angular application integrated with a HotdeskAPI backend, implementing full CRUD (Create, Read, Update, Delete) operations for user management.

## Architecture

### Frontend: Angular 20
- **Framework**: Angular 20 with standalone components
- **HTTP Client**: Modern fetch API with interceptor support
- **Routing**: Angular Router for navigation
- **Forms**: Template-driven forms with two-way data binding
- **Development Server**: Angular CLI dev server with proxy configuration

### Backend: HotdeskAPI
- **Server**: .NET API running on http://localhost:5251
- **Endpoints**: RESTful API with /api/Users endpoints
- **Data Format**: JSON requests/responses
- **Operations**: GET, POST, PUT, DELETE methods

## File Structure and Purpose

### Core Model Files
```
src/app/user/user.model.ts
```
**Purpose**: TypeScript interfaces defining data structures
- `User`: Complete user entity with optional fields (userId, createdDate)
- `CreateUserRequest`: New user creation (excludes auto-generated fields)
- `UpdateUserRequest`: User updates (includes userId for identification)

### Service Layer
```
src/app/user/user.service.ts
```
**Purpose**: HTTP service handling all API communication
- **Dependency Injection**: Injectable service at root level
- **HTTP Operations**: GET, POST, PUT, DELETE methods
- **Error Handling**: Centralized error processing with detailed logging
- **Type Safety**: Strongly typed with TypeScript interfaces

### Component Layer
```
src/app/user/user.ts
src/app/user/user.html
src/app/user/user.css
```
**Purpose**: User interface and business logic
- **Component**: Standalone Angular component
- **Template**: Clean HTML forms and data tables
- **Styling**: Component-specific CSS
- **State Management**: Local component state for forms and data

### Configuration Files

#### Environment Configuration
```
src/environments/environment.ts
```
**Purpose**: Environment-specific settings
- **API URL**: Base URL for API endpoints (`/api` for proxy)
- **Production Flag**: Development vs production mode

#### Application Configuration
```
src/app/app.config.ts
```
**Purpose**: Main application bootstrap configuration
- **HTTP Client**: Modern fetch API with interceptor support
- **Routing**: Application route configuration
- **Hydration**: Server-side rendering support
- **Error Handling**: Global error listeners

#### Proxy Configuration
```
proxy.conf.json
```
**Purpose**: Development proxy to solve CORS issues
- **Route Mapping**: `/api/*` → `http://localhost:5251/api/*`
- **CORS Solution**: Same-origin requests through proxy
- **Development Only**: Not used in production builds

## CORS Solution Explained

### The Problem
- Angular dev server: `http://localhost:4200`
- HotdeskAPI server: `http://localhost:5251`
- Browser blocks cross-origin requests (CORS policy)
- Results in "Error Code 0" failures

### The Solution
1. **Proxy Configuration**: Intercepts `/api/*` requests
2. **Request Flow**: Angular → Proxy → HotdeskAPI
3. **Response Flow**: HotdeskAPI → Proxy → Angular
4. **Browser Perspective**: All requests appear same-origin

### Implementation
```json
{
  "/api/*": {
    "target": "http://localhost:5251",
    "secure": false,
    "changeOrigin": true,
    "logLevel": "debug"
  }
}
```

## API Operations

### GET - Retrieve Users
```typescript
getUsers(): Observable<User[]>
```
- **Endpoint**: GET /api/Users
- **Purpose**: Fetch all users from database
- **Returns**: Array of User objects

### POST - Create User
```typescript
createUser(user: CreateUserRequest): Observable<User>
```
- **Endpoint**: POST /api/Users
- **Purpose**: Create new user in database
- **Input**: CreateUserRequest object
- **Returns**: Created User with generated ID

### PUT - Update User
```typescript
updateUser(user: UpdateUserRequest): Observable<User>
```
- **Endpoint**: PUT /api/Users/{userId}
- **Purpose**: Update existing user information
- **Input**: UpdateUserRequest with userId
- **Returns**: Updated User object

### DELETE - Remove User
```typescript
deleteUser(id: string): Observable<void>
```
- **Endpoint**: DELETE /api/Users/{id}
- **Purpose**: Permanently delete user from database
- **Input**: User ID string
- **Returns**: Void on successful deletion

## Error Handling

### Service Level
- **HttpErrorResponse**: Catches HTTP errors
- **Client vs Server**: Differentiates error sources
- **Logging**: Console error logging for debugging
- **Observable Errors**: Proper RxJS error propagation

### Component Level
- **Error Display**: User-friendly error messages
- **Loading States**: Visual feedback during operations
- **Form Validation**: Input validation and feedback

## Development Workflow

### Setup
1. Ensure HotdeskAPI is running on port 5251
2. Start Angular dev server: `npm start`
3. Proxy automatically handles API routing

### Testing CRUD Operations
1. **View Users**: Navigate to /user page
2. **Create User**: Fill create form and submit
3. **Edit User**: Click edit button, modify data, save
4. **Delete User**: Click delete button (with confirmation)

### Debugging
- **Console Logs**: Check browser developer tools
- **Network Tab**: Monitor HTTP requests/responses
- **Proxy Logs**: Terminal shows proxy request forwarding

## Best Practices Implemented

### TypeScript
- **Strong Typing**: All interfaces properly defined
- **Type Safety**: Compile-time error checking
- **IntelliSense**: Enhanced development experience

### Angular Architecture
- **Standalone Components**: Modern Angular approach
- **Dependency Injection**: Proper service injection
- **Reactive Programming**: RxJS observables for async operations

### Error Handling
- **Centralized**: Single error handling method
- **User Feedback**: Clear error messages
- **Debugging**: Detailed console logging

### Code Organization
- **Separation of Concerns**: Models, services, components separated
- **Single Responsibility**: Each file has clear purpose
- **Maintainability**: Well-commented, readable code

## Future Enhancements

### Authentication
- JWT token handling
- Login/logout functionality
- Protected routes

### Validation
- Server-side validation integration
- Client-side form validation enhancement
- Real-time validation feedback

### UI/UX
- Loading spinners and progress indicators
- Confirmation dialogs for destructive actions
- Toast notifications for operations

### Performance
- Pagination for large datasets
- Caching strategies
- Optimistic updates

This documentation provides a complete understanding of the Angular web API integration, enabling team members to maintain, extend, and explain the implementation effectively.
