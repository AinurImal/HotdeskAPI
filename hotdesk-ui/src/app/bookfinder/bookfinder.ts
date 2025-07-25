import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BookFinderService } from './bookfinder.service';
import { BookFinder, BookFinderSearchRequest, SEARCH_TYPES } from './bookfinder.model';

@Component({
  selector: 'app-bookfinder',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './bookfinder.html',
  styleUrl: './bookfinder.css'
})
export class BookFinderComponent implements OnInit {
  searchForm: FormGroup;
  bookFinders: BookFinder[] = [];
  allBookFinders: BookFinder[] = []; // Store all bookings for reset functionality
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  hasSearched = false;
  
  // Pagination properties
  currentPage = 1;
  pageSize = 10;
  totalItems = 0;
  totalPages = 0;
  
  // Search type options
  searchTypes = SEARCH_TYPES;
  
  // Make Math available in template
  Math = Math;

  constructor(
    private fb: FormBuilder,
    private bookFinderService: BookFinderService
  ) {
    this.searchForm = this.createForm();
  }

  ngOnInit(): void {
    // Component initialized - waiting for user to search
  }

  /**
   * Creates and returns the reactive form for search functionality
   */
  private createForm(): FormGroup {
    return this.fb.group({
      searchValue: [''],
      searchBy: ['desk']
    });
  }

  /**
   * Loads all bookings with user and desk information
   */
  onLoadAll(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.hasSearched = true;
    
    this.bookFinderService.getAllBookFinders().subscribe({
      next: (bookings) => {
        this.bookFinders = bookings;
        this.allBookFinders = [...bookings]; // Store copy for reset
        this.totalItems = bookings.length;
        this.updatePagination();
        this.isLoading = false;
        this.successMessage = `Loaded ${bookings.length} booking(s) successfully.`;
        setTimeout(() => this.successMessage = '', 3000);
      },
      error: (error) => {
        this.errorMessage = 'Failed to load BookFinder results. Please try again.';
        this.isLoading = false;
      }
    });
  }

  /**
   * Handles search form submission
   */
  onSearch(): void {
    if (this.searchForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';
    this.hasSearched = true;

    const formValue = this.searchForm.value;
    const searchRequest: BookFinderSearchRequest = {
      id: formValue.searchValue?.trim() || '',
      searchBy: formValue.searchBy || 'desk'
    };

    this.bookFinderService.searchBookFinders(searchRequest).subscribe({
      next: (bookings) => {
        this.bookFinders = bookings;
        this.totalItems = bookings.length;
        this.updatePagination();
        this.isLoading = false;
        
        if (bookings.length === 0) {
          this.errorMessage = `No bookings found for ${this.getSearchTypeLabel(searchRequest.searchBy)} "${searchRequest.id}".`;
        } else {
          this.successMessage = `Found ${bookings.length} booking(s) for ${this.getSearchTypeLabel(searchRequest.searchBy)} "${searchRequest.id}".`;
          setTimeout(() => this.successMessage = '', 3000);
        }
      },
      error: (error) => {
        this.errorMessage = 'Search failed. Please try again.';
        this.isLoading = false;
      }
    });
  }

  /**
   * Clears the search form and results
   */
  onClear(): void {
    this.searchForm.reset({
      searchValue: '',
      searchBy: 'desk'
    });
    this.bookFinders = [];
    this.allBookFinders = [];
    this.totalItems = 0;
    this.updatePagination();
    this.errorMessage = '';
    this.successMessage = '';
    this.hasSearched = false;
  }

  /**
   * Deletes a booking with confirmation
   */
  deleteBooking(booking: BookFinder): void {
    if (!booking.bookingId) {
      this.errorMessage = 'Cannot delete booking: Booking ID is missing.';
      return;
    }

    const confirmMessage = `Are you sure you want to delete the booking for ${booking.userName} at ${booking.deskName}?`;
    
    if (confirm(confirmMessage)) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      this.bookFinderService.deleteBooking(booking.bookingId).subscribe({
        next: () => {
          // Remove from both arrays
          this.bookFinders = this.bookFinders.filter((b: BookFinder) => b.bookingId !== booking.bookingId);
          this.allBookFinders = this.allBookFinders.filter((b: BookFinder) => b.bookingId !== booking.bookingId);
          this.totalItems = this.bookFinders.length;
          this.updatePagination();
          
          this.isLoading = false;
          this.successMessage = `Booking for ${booking.userName} at ${booking.deskName} deleted successfully!`;
          setTimeout(() => this.successMessage = '', 3000);
        },
        error: (error) => {
          this.errorMessage = 'Failed to delete booking. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  /**
   * View details of a booking
   */
  viewDetails(bookFinder: BookFinder): void {
    alert(`Booking Details:\n\nUser: ${bookFinder.userName}\nDesk: ${bookFinder.deskName}\nLocation: ${bookFinder.location}\nDate: ${this.formatDate(bookFinder.bookingDate)}\nDuration: ${this.getDurationTypeLabel(bookFinder.durationType)}\nPhone: ${bookFinder.phoneNumber}`);
  }

  /**
   * Edit a booking
   */
  editBooking(bookFinder: BookFinder): void {
    this.errorMessage = 'Edit functionality will be implemented in future updates.';
    setTimeout(() => this.errorMessage = '', 3000);
  }

  /**
   * Export results
   */
  exportResults(): void {
    this.successMessage = 'Export functionality will be implemented in future updates.';
    setTimeout(() => this.successMessage = '', 3000);
  }

  /**
   * Refresh results
   */
  refreshResults(): void {
    this.onLoadAll();
  }

  /**
   * Focus on search input
   */
  focusSearch(): void {
    const searchInput = document.getElementById('searchValue') as HTMLInputElement;
    if (searchInput) {
      searchInput.focus();
    }
  }

  /**
   * Update pagination properties
   */
  private updatePagination(): void {
    this.totalPages = Math.ceil(this.totalItems / this.pageSize);
    if (this.currentPage > this.totalPages) {
      this.currentPage = Math.max(1, this.totalPages);
    }
  }

  /**
   * Go to a specific page
   */
  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  /**
   * Get page numbers for pagination
   */
  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxPagesToShow = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);
    
    if (endPage - startPage < maxPagesToShow - 1) {
      startPage = Math.max(1, endPage - maxPagesToShow + 1);
    }
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  /**
   * Get status class for styling
   */
  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'active':
      case 'confirmed':
        return 'status-active';
      case 'pending':
        return 'status-pending';
      case 'cancelled':
        return 'status-cancelled';
      case 'completed':
        return 'status-completed';
      default:
        return 'status-default';
    }
  }

  /**
   * Get status label
   */
  getStatusLabel(status: string): string {
    switch (status?.toLowerCase()) {
      case 'active':
        return 'Active';
      case 'confirmed':
        return 'Confirmed';
      case 'pending':
        return 'Pending';
      case 'cancelled':
        return 'Cancelled';
      case 'completed':
        return 'Completed';
      default:
        return 'Unknown';
    }
  }

  /**
   * Format time for display
   */
  formatTime(time: string): string {
    if (!time) return '';
    try {
      const date = new Date(`2000-01-01T${time}`);
      return date.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
      });
    } catch {
      return time;
    }
  }

  /**
   * Gets user-friendly label for search type
   */
  private getSearchTypeLabel(searchBy: string): string {
    const searchType = this.searchTypes.find(type => type.value === searchBy);
    return searchType ? searchType.label.toLowerCase().replace('search by ', '') : searchBy;
  }

  /**
   * Formats date for display
   */
  formatDate(date: Date | string): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  /**
   * Gets a more readable duration type label
   */
  getDurationTypeLabel(durationType: string): string {
    const durationMap: { [key: string]: string } = {
      'daily': 'Full Day',
      'half-day-morning': 'Half Day (Morning)',
      'half-day-afternoon': 'Half Day (Afternoon)',
      'hourly': 'Hourly'
    };
    return durationMap[durationType] || durationType;
  }
}
