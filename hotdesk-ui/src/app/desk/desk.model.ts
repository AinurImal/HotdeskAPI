// Interface representing a complete desk entity from the HotdeskAPI
export interface Desk {
  deskId?: number;      // Optional unique identifier for the desk (auto-generated)
  name: string;         // Desk name/identifier (required field)
  location: string;     // Location where the desk is situated (required field)
  hasMonitor: boolean;  // Whether the desk has a monitor available (required field)
  isAvailable: boolean; // Availability status of the desk (required field)
  description?: string; // Optional additional description or notes about the desk
  createdDate?: Date;   // Optional timestamp when the desk was created
  updatedDate?: Date;   // Optional timestamp when the desk was last updated
}

// Interface for creating a new desk - excludes auto-generated fields
export interface CreateDeskRequest {
  name: string;         // Desk name/identifier (required for creation)
  location: string;     // Location where the desk is situated (required for creation)
  hasMonitor: boolean;  // Whether the desk has a monitor available (required for creation)
  isAvailable: boolean; // Availability status of the desk (required for creation)
  description?: string; // Optional additional description or notes about the desk
}

// Interface for updating an existing desk - includes deskId for identification
export interface UpdateDeskRequest {
  deskId: number;       // Required unique identifier to specify which desk to update
  name: string;         // Updated desk name/identifier
  location: string;     // Updated location where the desk is situated
  hasMonitor: boolean;  // Updated monitor availability status
  isAvailable: boolean; // Updated availability status of the desk
  description?: string; // Updated additional description or notes about the desk
}

// Interface for desk availability response from HotdeskAPI
export interface DeskAvailabilityResponse {
  deskId: number;       // Desk identifier
  date: string;         // Date for which availability was checked (ISO format)
  isAvailable: boolean; // True if desk is available, false if booked
  message: string;      // Descriptive message about availability status
}
