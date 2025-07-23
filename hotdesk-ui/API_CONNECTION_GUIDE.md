# HotdeskAPI Connection Guide

## Current Setup

Your Angular application is now configured to connect to your HotdeskAPI. Here's what has been implemented:

### 🔧 **Configured Services:**

1. **UserService** - `/api/users`
   - GET /users - Get all users
   - GET /users/{id} - Get user by ID
   - POST /users - Create user
   - PUT /users/{id} - Update user
   - DELETE /users/{id} - Delete user

2. **DeskService** - `/api/desks`
   - GET /desks - Get all desks
   - GET /desks/available - Get available desks
   - GET /desks/floor/{floor} - Get desks by floor
   - POST /desks - Create desk
   - PUT /desks/{id} - Update desk
   - DELETE /desks/{id} - Delete desk

3. **BookingService** - `/api/bookings`
   - GET /bookings - Get all bookings
   - GET /bookings/user/{userId} - Get bookings by user
   - GET /bookings/desk/{deskId} - Get bookings by desk
   - GET /bookings/date/{date} - Get bookings by date
   - POST /bookings - Create booking
   - PUT /bookings/{id} - Update booking
   - DELETE /bookings/{id} - Delete booking
   - POST /bookings/check-availability - Check availability
   - PUT /bookings/{id}/checkin - Check in
   - PUT /bookings/{id}/cancel - Cancel booking

### 🔗 **API Configuration:**

Current API URL: `https://localhost:5001/api`

To change the API URL, update `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api' // Change this to your API URL
};
```

### 🚀 **Starting Your HotdeskAPI:**

1. **Navigate to your API project folder:**
   ```bash
   cd c:\Users\Work Purpose\source\Csharp\HotdeskAPI
   ```

2. **Run the API using one of these commands:**
   ```bash
   # Using dotnet CLI
   dotnet run

   # Or if you have a specific project file
   dotnet run --project YourApiProject.csproj

   # Or using Visual Studio - just press F5
   ```

3. **Common API URLs:**
   - HTTPS: `https://localhost:5001` or `https://localhost:7001`
   - HTTP: `http://localhost:5000` or `http://localhost:7000`

### 🧪 **Testing the Connection:**

The User page now includes API testing tools:

1. **Test API Connection** - Tests all endpoints
2. **Test Different Ports** - Scans common ports to find your API
3. **Try Load Users** - Attempts to load data from /api/users

### 📊 **Expected Data Models:**

**User:**
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john@company.com",
  "department": "IT",
  "createdDate": "2025-01-01T00:00:00Z"
}
```

**Desk:**
```json
{
  "id": 1,
  "deskNumber": "A-001",
  "floor": 1,
  "location": "Open Area",
  "isAvailable": true,
  "hasMonitor": true,
  "hasKeyboard": true,
  "hasMouse": true,
  "description": "Window seat"
}
```

**Booking:**
```json
{
  "id": 1,
  "userId": 1,
  "deskId": 1,
  "bookingDate": "2025-01-01T00:00:00Z",
  "startTime": "09:00",
  "endTime": "17:00",
  "status": 1,
  "notes": "Full day booking"
}
```

### 🛠 **Troubleshooting:**

1. **"Cannot connect to API"** - Make sure your HotdeskAPI is running
2. **CORS errors** - Add CORS configuration to your API
3. **SSL certificate errors** - Use HTTP during development or configure HTTPS properly

### 📝 **Next Steps:**

1. Start your HotdeskAPI
2. Use the API testing tools on the User page
3. Update the API URL in environment.ts if needed
4. Test CRUD operations through the UI

The Angular app is ready to work with your HotdeskAPI once it's running!
