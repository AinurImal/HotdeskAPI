import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BookingService } from './booking.service';
import { Booking, CreateBookingRequest, UpdateBookingRequest, DURATION_TYPES } from './booking.model';
import { DeskService } from '../desk/desk.service';
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
  desks: Desk[] = [];
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  isEditMode = false;
  editingBookingId: number | null = null;
  
  // Duration type options
  durationTypes = DURATION_TYPES;

  constructor(
    private fb: FormBuilder,
    private bookingService: BookingService,
    private deskService: DeskService
  ) {
    this.bookingForm = this.createForm();
  }

  ngOnInit(): void {
    this.loadBookings();
    this.loadDesks();
  }

  /**
   * Creates and returns the reactive form for booking management
   */
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
      deskId: parseInt(formValue.deskId),
      userName: formValue.userName.trim(),
      bookingDate: new Date(formValue.bookingDate),
      durationType: formValue.durationType,
      checkedIn: formValue.checkedIn,
      checkInTime: formValue.checkedIn && formValue.checkInTime ? formValue.checkInTime : undefined
    };

    this.bookingService.createBooking(newBooking).subscribe({
      next: (createdBooking) => {
        this.bookings.unshift(createdBooking);
        this.successMessage = `Booking created successfully for ${createdBooking.userName} at ${this.getDeskName(createdBooking.deskId)}!`;
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
      bookingId: this.editingBookingId!,
      deskId: parseInt(formValue.deskId),
      userName: formValue.userName.trim(),
      bookingDate: new Date(formValue.bookingDate),
      durationType: formValue.durationType,
      checkedIn: formValue.checkedIn,
      checkInTime: formValue.checkedIn && formValue.checkInTime ? formValue.checkInTime : undefined
    };

    this.bookingService.updateBooking(updatedBooking).subscribe({
      next: (booking) => {
        const index = this.bookings.findIndex(b => b.bookingId === this.editingBookingId);
        if (index !== -1) {
          this.bookings[index] = booking;
        }
        this.successMessage = `Booking updated successfully for ${booking.userName} at ${this.getDeskName(booking.deskId)}!`;
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
    this.editingBookingId = booking.bookingId || null;
    this.errorMessage = '';
    this.successMessage = '';

    // Format the date for the input field
    const bookingDate = new Date(booking.bookingDate);
    const formattedDate = bookingDate.toISOString().split('T')[0];

    this.bookingForm.patchValue({
      bookingId: booking.bookingId,
      deskId: booking.deskId,
      userName: booking.userName,
      bookingDate: formattedDate,
      durationType: booking.durationType,
      checkedIn: booking.checkedIn,
      checkInTime: booking.checkInTime || ''
    });
  }

  /**
   * Deletes a booking with confirmation
   */
  deleteBooking(booking: Booking): void {
    if (!booking.bookingId) {
      return;
    }

    const confirmed = confirm(`Are you sure you want to delete the booking for ${booking.userName} at ${this.getDeskName(booking.deskId)}? This action cannot be undone.`);
    
    if (confirmed) {
      this.bookingService.deleteBooking(booking.bookingId).subscribe({
        next: () => {
          this.bookings = this.bookings.filter(b => b.bookingId !== booking.bookingId);
          this.successMessage = `Booking for ${booking.userName} at ${this.getDeskName(booking.deskId)} deleted successfully!`;
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
      deskId: '',
      userName: '',
      bookingDate: '',
      durationType: 'daily',
      checkedIn: false,
      checkInTime: ''
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
      if (field.errors['minlength']) {
        return `${this.getFieldDisplayName(fieldName)} must be at least ${field.errors['minlength'].requiredLength} characters.`;
      }
    }
    return '';
  }

  /**
   * Gets the display name for form fields
   */
  private getFieldDisplayName(fieldName: string): string {
    const fieldNames: { [key: string]: string } = {
      deskId: 'Desk',
      userName: 'User Name',
      bookingDate: 'Booking Date',
      durationType: 'Duration Type',
      checkInTime: 'Check-in Time'
    };
    return fieldNames[fieldName] || fieldName;
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
   * Gets duration type label
   */
  getDurationTypeLabel(durationType: string): string {
    const duration = this.durationTypes.find(d => d.value === durationType);
    return duration ? duration.label : durationType;
  }

  /**
   * Handles check-in checkbox change
   */
  onCheckedInChange(): void {
    const checkedIn = this.bookingForm.get('checkedIn')?.value;
    if (checkedIn && !this.bookingForm.get('checkInTime')?.value) {
      // Auto-fill current time when checking in
      const now = new Date();
      const currentTime = now.toTimeString().slice(0, 5); // HH:MM format
      this.bookingForm.patchValue({ checkInTime: currentTime });
    } else if (!checkedIn) {
      // Clear check-in time when unchecking
      this.bookingForm.patchValue({ checkInTime: '' });
    }
  }
}


