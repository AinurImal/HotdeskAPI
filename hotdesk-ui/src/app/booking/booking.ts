import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BookingService } from './booking.service';
import { Booking, CreateBookingRequest, UpdateBookingRequest, BookingStatus } from './booking.model';
import { UserService } from '../user/user.service';
import { DeskService } from '../desk/desk.service';
import { User } from '../user/user.model';
import { Desk } from '../desk/desk.model';

@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './booking.html',
  styleUrl: './booking.css'
})
export class BookingComponent implements OnInit {
  bookingForm: FormGroup;
  bookings: Booking[] = [];
  users: User[] = [];
  desks: Desk[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  editingBookingId: number | null = null;
  
  // Booking status options
  bookingStatuses = [
    { value: BookingStatus.Pending, label: 'Pending' },
    { value: BookingStatus.Confirmed, label: 'Confirmed' },
    { value: BookingStatus.CheckedIn, label: 'Checked In' },
    { value: BookingStatus.Completed, label: 'Completed' },
    { value: BookingStatus.Cancelled, label: 'Cancelled' },
    { value: BookingStatus.NoShow, label: 'No Show' }
  ];

  constructor(
    private fb: FormBuilder,
    private bookingService: BookingService,
    private userService: UserService,
    private deskService: DeskService
  ) {
    this.bookingForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadBookings();
    this.loadUsers();
    this.loadDesks();
  }

  /**
   * Creates and returns the reactive form for booking management
   */
  private createForm(): FormGroup {
    return this.fb.group({
      bookingId: [''],
      userId: ['', [Validators.required]],
      deskId: ['', [Validators.required]],
      bookingDate: ['', [Validators.required]],
      startTime: ['', [Validators.required]],
      endTime: ['', [Validators.required]],
      status: [BookingStatus.Pending, [Validators.required]],
      notes: ['']
    });
  }

  /**
   * Loads all bookings from the API
   */
  loadBookings(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.bookingService.getBookings().subscribe({
      next: (bookings) => {
        this.bookings = bookings;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  /**
   * Loads all users for the dropdown
   */
  loadUsers(): void {
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
      },
      error: (error) => {
        console.error('Failed to load users:', error);
      }
    });
  }

  /**
   * Loads all desks for the dropdown
   */
  loadDesks(): void {
    this.deskService.getDesks().subscribe({
      next: (desks) => {
        this.desks = desks.filter(desk => desk.isAvailable);
      },
      error: (error) => {
        console.error('Failed to load desks:', error);
      }
    });
  }

  /**
   * Handles form submission for both create and update operations
   */
  onSubmit(): void {
    if (this.bookingForm.valid) {
      if (this.isEditMode) {
        this.updateBooking();
      } else {
        this.createBooking();
      }
    } else {
      this.markFormGroupTouched();
    }
  }

  /**
   * Creates a new booking
   */
  private createBooking(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.bookingForm.value;
    const newBooking: CreateBookingRequest = {
      userId: parseInt(formValue.userId),
      deskId: parseInt(formValue.deskId),
      bookingDate: new Date(formValue.bookingDate),
      startTime: formValue.startTime,
      endTime: formValue.endTime,
      notes: formValue.notes || undefined
    };

    this.bookingService.createBooking(newBooking).subscribe({
      next: (createdBooking) => {
        this.bookings.unshift(createdBooking);
        this.successMessage = `Booking created successfully for ${this.getUserName(createdBooking.userId)} at ${this.getDeskName(createdBooking.deskId)}!`;
        this.resetForm();
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  /**
   * Updates an existing booking
   */
  private updateBooking(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const formValue = this.bookingForm.value;
    const updatedBooking: UpdateBookingRequest = {
      id: this.editingBookingId!,
      userId: parseInt(formValue.userId),
      deskId: parseInt(formValue.deskId),
      bookingDate: new Date(formValue.bookingDate),
      startTime: formValue.startTime,
      endTime: formValue.endTime,
      status: parseInt(formValue.status),
      notes: formValue.notes || undefined
    };

    this.bookingService.updateBooking(updatedBooking).subscribe({
      next: (booking) => {
        const index = this.bookings.findIndex(b => b.id === this.editingBookingId);
        if (index !== -1) {
          this.bookings[index] = booking;
        }
        this.successMessage = `Booking updated successfully for ${this.getUserName(booking.userId)} at ${this.getDeskName(booking.deskId)}!`;
        this.resetForm();
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  /**
   * Prepares the form for editing an existing booking
   */
  editBooking(booking: Booking): void {
    this.isEditMode = true;
    this.editingBookingId = booking.id || null;
    this.errorMessage = '';
    this.successMessage = '';

    // Format the date for the input field
    const bookingDate = new Date(booking.bookingDate);
    const formattedDate = bookingDate.toISOString().split('T')[0];

    this.bookingForm.patchValue({
      bookingId: booking.id,
      userId: booking.userId,
      deskId: booking.deskId,
      bookingDate: formattedDate,
      startTime: booking.startTime,
      endTime: booking.endTime,
      status: booking.status,
      notes: booking.notes
    });
  }

  /**
   * Deletes a booking with confirmation
   */
  deleteBooking(booking: Booking): void {
    if (!booking.id) {
      return;
    }

    const userName = this.getUserName(booking.userId);
    const deskName = this.getDeskName(booking.deskId);
    
    const confirmed = confirm(`Are you sure you want to delete the booking for ${userName} at ${deskName}? This action cannot be undone.`);
    
    if (confirmed) {
      this.bookingService.deleteBooking(booking.id).subscribe({
        next: () => {
          this.bookings = this.bookings.filter(b => b.id !== booking.id);
          this.successMessage = `Booking for ${userName} at ${deskName} deleted successfully!`;
          this.errorMessage = '';
        },
        error: (error) => {
          this.errorMessage = error;
        }
      });
    }
  }

  /**
   * Resets the form to its initial state
   */
  resetForm(): void {
    this.isEditMode = false;
    this.editingBookingId = null;
    this.bookingForm.reset({
      bookingId: '',
      userId: '',
      deskId: '',
      bookingDate: '',
      startTime: '',
      endTime: '',
      status: BookingStatus.Pending,
      notes: ''
    });
    this.errorMessage = '';
  }

  /**
   * Marks all form fields as touched to trigger validation display
   */
  private markFormGroupTouched(): void {
    Object.keys(this.bookingForm.controls).forEach(key => {
      this.bookingForm.get(key)?.markAsTouched();
    });
  }

  /**
   * Checks if a form field is invalid and has been touched
   */
  isFieldInvalid(fieldName: string): boolean {
    const field = this.bookingForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  /**
   * Gets the error message for a specific form field
   */
  getFieldError(fieldName: string): string {
    const field = this.bookingForm.get(fieldName);
    if (field && field.errors && (field.dirty || field.touched)) {
      if (field.errors['required']) {
        return `${this.getFieldDisplayName(fieldName)} is required.`;
      }
      if (field.errors['pattern']) {
        return `${this.getFieldDisplayName(fieldName)} format is invalid.`;
      }
    }
    return '';
  }

  /**
   * Gets the display name for form fields
   */
  private getFieldDisplayName(fieldName: string): string {
    const fieldNames: { [key: string]: string } = {
      userId: 'User',
      deskId: 'Desk',
      bookingDate: 'Booking Date',
      startTime: 'Start Time',
      endTime: 'End Time',
      status: 'Status'
    };
    return fieldNames[fieldName] || fieldName;
  }

  /**
   * Gets user name by ID
   */
  getUserName(userId: number): string {
    const user = this.users.find(u => u.userId === userId.toString());
    return user ? user.fullName : `User ${userId}`;
  }

  /**
   * Gets desk name by ID
   */
  getDeskName(deskId: number): string {
    const desk = this.desks.find(d => d.deskId === deskId);
    return desk ? desk.name : `Desk ${deskId}`;
  }

  /**
   * Gets desk location by ID
   */
  getDeskLocation(deskId: number): string {
    const desk = this.desks.find(d => d.deskId === deskId);
    return desk ? desk.location : 'Unknown';
  }

  /**
   * Gets booking status label
   */
  getStatusLabel(status: BookingStatus): string {
    const statusOption = this.bookingStatuses.find(s => s.value === status);
    return statusOption ? statusOption.label : 'Unknown';
  }

  /**
   * Gets CSS class for booking status
   */
  getStatusClass(status: BookingStatus): string {
    switch (status) {
      case BookingStatus.Confirmed:
        return 'status-confirmed';
      case BookingStatus.CheckedIn:
        return 'status-checked-in';
      case BookingStatus.Completed:
        return 'status-completed';
      case BookingStatus.Cancelled:
        return 'status-cancelled';
      case BookingStatus.NoShow:
        return 'status-no-show';
      default:
        return 'status-pending';
    }
  }
}


