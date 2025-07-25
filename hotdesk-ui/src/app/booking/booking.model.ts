export interface Booking {
  bookingId?: number;        // Auto-generated, not displayed in form
  deskId: number;           // Dropdown selection
  userName: string;         // Input by user (must match existing user)
  bookingDate: Date;        // Calendar input
  durationType: string;     // Dropdown (specifically state daily)
  checkedIn: boolean;       // Checkbox
  checkInTime?: string;     // Time input (optional, when checked in)
  createdDate?: Date;
  updatedDate?: Date;
  // Navigation properties for display
  deskName?: string;
  deskLocation?: string;
}

export interface CreateBookingRequest {
  deskId: number;           // Dropdown selection
  userName: string;         // Input by user
  bookingDate: Date;        // Calendar input
  durationType: string;     // Dropdown (daily)
  checkedIn: boolean;       // Checkbox
  checkInTime?: string;     // Time input (optional)
}

export interface UpdateBookingRequest {
  bookingId: number;        // Required for update
  deskId: number;           // Dropdown selection
  userName: string;         // Input by user
  bookingDate: Date;        // Calendar input
  durationType: string;     // Dropdown (daily)
  checkedIn: boolean;       // Checkbox
  checkInTime?: string;     // Time input (optional)
}

// Duration type options
export interface DurationType {
  value: string;
  label: string;
}

export const DURATION_TYPES: DurationType[] = [
  { value: 'daily', label: 'Daily' },
  { value: 'half-day-morning', label: 'Half Day (Morning)' },
  { value: 'half-day-afternoon', label: 'Half Day (Afternoon)' }
];

export interface BookingAvailabilityRequest {
  deskId: number;
  bookingDate: Date;
  durationType: string;
  endTime: string;
}
