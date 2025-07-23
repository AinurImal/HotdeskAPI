// Interface representing a complete user entity from the HotdeskAPI
export interface User {
  userId?: string;      // Optional unique identifier for the user (UUID format)
  fullName: string;     // User's complete full name (required field)
  userName: string;     // Unique username for login/identification (required field)
  phoneNumber: string;  // User's contact phone number (required field)
  email: string;        // User's email address (required field)
  createdDate?: Date;   // Optional timestamp when the user was created
}

// Interface for creating a new user - excludes auto-generated fields
export interface CreateUserRequest {
  fullName: string;     // User's complete full name (required for creation)
  userName: string;     // Unique username for the new user (required for creation)
  phoneNumber: string;  // User's contact phone number (required for creation)
  email: string;        // User's email address (required for creation)
}

// Interface for updating an existing user - includes userId for identification
export interface UpdateUserRequest {
  userId: string;       // Required unique identifier to specify which user to update
  fullName: string;     // Updated full name for the user
  userName: string;     // Updated username for the user
  phoneNumber: string;  // Updated phone number for the user
  email: string;        // Updated email address for the user
}
