export interface BookFinder {
  bookingId: number;        // Unique identifier for a specific booking
  userName: string;         // Name of the user who made the booking  
  userId: string;           // User ID of the person who made the booking
  phoneNumber: string;      // Phone number of the user for contact purposes
  deskName: string;         // Name of the desk that has been booked
  location: string;         // Location of the booked desk
  bookingDate: Date;        // Date when the booking was made or scheduled
  durationType: string;     // Duration of the booking (e.g., "daily", "half-day")
}

export interface BookFinderSearchRequest {
  id: string;               // Search value (desk id, user name, or booking id)
  searchBy: 'desk' | 'user' | 'booking'; // Search criteria type
}

// Search type options for UI
export interface SearchType {
  value: 'desk' | 'user' | 'booking';
  label: string;
  placeholder: string;
}

export const SEARCH_TYPES: SearchType[] = [
  { 
    value: 'desk', 
    label: 'Search by Desk ID', 
    placeholder: 'Enter desk ID (e.g., 1, 2, 3)' 
  },
  { 
    value: 'user', 
    label: 'Search by User Name', 
    placeholder: 'Enter username (e.g., john.doe)' 
  },
  { 
    value: 'booking', 
    label: 'Search by Booking ID', 
    placeholder: 'Enter booking ID (e.g., 123)' 
  }
];
