export interface Booking {
  id?: number;
  userId: number;
  deskId: number;
  bookingDate: Date;
  startTime: string;
  endTime: string;
  status: BookingStatus;
  notes?: string;
  createdDate?: Date;
  updatedDate?: Date;
  // Navigation properties
  userName?: string;
  deskNumber?: string;
  deskLocation?: string;
}

export interface CreateBookingRequest {
  userId: number;
  deskId: number;
  bookingDate: Date;
  startTime: string;
  endTime: string;
  notes?: string;
}

export interface UpdateBookingRequest {
  id: number;
  userId: number;
  deskId: number;
  bookingDate: Date;
  startTime: string;
  endTime: string;
  status: BookingStatus;
  notes?: string;
}

export enum BookingStatus {
  Pending = 0,
  Confirmed = 1,
  CheckedIn = 2,
  Completed = 3,
  Cancelled = 4,
  NoShow = 5
}

export interface BookingAvailabilityRequest {
  deskId: number;
  bookingDate: Date;
  startTime: string;
  endTime: string;
}
