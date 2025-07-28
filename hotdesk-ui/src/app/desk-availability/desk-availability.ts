import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DeskService } from '../desk/desk.service';
import { Desk } from '../desk/desk.model';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';

@Component({
  selector: 'app-desk-availability',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './desk-availability.html',
  styleUrl: './desk-availability.css'
})
export class DeskAvailabilityComponent implements OnInit {
  // Data properties
  desks: Desk[] = [];
  availableDesks: Desk[] = [];
  unavailableDesks: Desk[] = [];
  
  // Loading and error states
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  
  // Filter properties
  searchTerm = '';
  selectedLocation = '';
  selectedDate = '';
  locations: string[] = [];
  showMonitorOnly = false;
  
  // Navigation state
  isDeskNavExpanded = true; // Default to expanded since we're on a desk page

  constructor(private deskService: DeskService) {}

  ngOnInit(): void {
    // Set default date to today
    this.selectedDate = this.formatDateForInput(new Date());
    this.loadDeskAvailability();
  }

  /**
   * Load desk availability data using HotdeskAPI logic with date-specific availability
   * Uses both all desks and date-specific availability checks
   */
  loadDeskAvailability(): void {
    this.isLoading = true;
    this.clearMessages();
    
    // First get all desks
    this.deskService.getDesks().pipe(
      finalize(() => {
        this.isLoading = false;
      })
    ).subscribe({
      next: (allDesks) => {
        this.desks = this.validateDeskData(allDesks);
        console.log('All Desks:', this.desks);
        
        // Check availability for each desk on the selected date
        this.checkDesksAvailabilityForDate();
        this.extractLocations();
      },
      error: (error) => {
        this.handleApiError(error);
      }
    });
  }

  /**
   * Check availability for each desk on the selected date
   */
  private checkDesksAvailabilityForDate(): void {
    if (!this.selectedDate || this.desks.length === 0) {
      this.availableDesks = [];
      this.unavailableDesks = [...this.desks];
      return;
    }

    const dateForApi = this.formatDateForApi(this.selectedDate);
    console.log('Checking availability for date:', dateForApi);

    // Check each desk's availability for the selected date
    const availabilityChecks = this.desks.map(desk => {
      return this.deskService.checkDeskAvailability(desk.deskId!, dateForApi).pipe(
        catchError(error => {
          console.error(`Error checking availability for desk ${desk.deskId}:`, error);
          // On error, assume desk is unavailable for safety
          return of({ DeskId: desk.deskId, IsAvailable: false });
        })
      );
    });

    forkJoin(availabilityChecks).subscribe({
      next: (availabilityResults) => {
        console.log('Availability Results:', availabilityResults);
        
        this.availableDesks = [];
        this.unavailableDesks = [];

        this.desks.forEach(desk => {
          const availabilityResult = availabilityResults.find(result => result.DeskId === desk.deskId);
          if (availabilityResult && availabilityResult.IsAvailable) {
            this.availableDesks.push({ ...desk, isAvailable: true });
          } else {
            this.unavailableDesks.push({ ...desk, isAvailable: false });
          }
        });

        console.log('Available Desks:', this.availableDesks);
        console.log('Unavailable Desks:', this.unavailableDesks);

        this.successMessage = `Successfully loaded ${this.desks.length} desk(s) for ${this.formatDateForDisplay(this.selectedDate)}. ${this.availableDesks.length} available, ${this.unavailableDesks.length} unavailable.`;
      },
      error: (error) => {
        console.error('Error checking desk availability:', error);
        this.errorMessage = 'Failed to check desk availability for the selected date. Please try again.';
      }
    });
  }

  /**
   * Validate desk data structure following HotdeskAPI validation rules
   */
  private validateDeskData(desks: Desk[]): Desk[] {
    return desks.filter(desk => {
      // Validate required fields as per HotdeskAPI model
      if (!desk.name || desk.name.trim().length === 0) {
        console.warn('Desk with invalid name detected:', desk);
        return false;
      }
      
      if (!desk.location || desk.location.trim().length === 0) {
        console.warn('Desk with invalid location detected:', desk);
        return false;
      }
      
      // Validate deskId if present
      if (desk.deskId !== undefined && (desk.deskId <= 0 || !Number.isInteger(desk.deskId))) {
        console.warn('Desk with invalid ID detected:', desk);
        return false;
      }
      
      return true;
    });
  }

  /**
   * Extract unique locations from desks with validation
   */
  private extractLocations(): void {
    const validLocations = this.desks
      .map(desk => desk.location)
      .filter(location => location && location.trim().length > 0);
    
    this.locations = [...new Set(validLocations)].sort();
  }

  /**
   * Filter desks based on search criteria with validation
   */
  getFilteredDesks(desks: Desk[]): Desk[] {
    if (!Array.isArray(desks)) {
      console.error('Invalid desks array passed to filter function');
      return [];
    }
    
    return desks.filter(desk => {
      // Validate desk object
      if (!desk || typeof desk !== 'object') {
        return false;
      }
      
      // Search term validation and matching
      const searchMatch = !this.searchTerm || this.searchTerm.trim().length === 0 || 
        (desk.name && desk.name.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
        (desk.location && desk.location.toLowerCase().includes(this.searchTerm.toLowerCase())) ||
        (desk.description && desk.description.toLowerCase().includes(this.searchTerm.toLowerCase()));
      
      // Location filter validation
      const locationMatch = !this.selectedLocation || 
        (desk.location && desk.location === this.selectedLocation);
      
      // Monitor filter validation
      const monitorMatch = !this.showMonitorOnly || 
        (typeof desk.hasMonitor === 'boolean' && desk.hasMonitor);
      
      return searchMatch && locationMatch && monitorMatch;
    });
  }

  /**
   * Handle API errors with specific error types
   */
  private handleApiError(error: any): void {
    console.error('API Error in desk availability:', error);
    
    // Clear existing data on error
    this.desks = [];
    this.availableDesks = [];
    this.unavailableDesks = [];
    this.locations = [];
    
    // Set user-friendly error message based on error type
    if (error?.message?.includes('connection') || error?.message?.includes('network')) {
      this.errorMessage = 'Connection Error: Unable to connect to the HotdeskAPI server. Please check if the server is running and try again.';
    } else if (error?.message?.includes('timeout')) {
      this.errorMessage = 'Request timeout: The server is taking too long to respond. Please try again.';
    } else if (error?.message?.includes('Invalid API response')) {
      this.errorMessage = 'Data Error: Received invalid data from the server. Please contact support.';
    } else {
      this.errorMessage = error?.message || 'Failed to load desk availability. Please try again later.';
    }
  }

  /**
   * Toggle desk navigation expansion
   */
  toggleDeskNav(): void {
    this.isDeskNavExpanded = !this.isDeskNavExpanded;
  }

  /**
   * Reset all filters and reload data
   */
  resetFilters(): void {
    this.searchTerm = '';
    this.selectedLocation = '';
    this.selectedDate = this.formatDateForInput(new Date());
    this.showMonitorOnly = false;
    this.clearMessages();
    this.loadDeskAvailability();
  }

  /**
   * Refresh desk data with loading state
   */
  refreshDesks(): void {
    this.loadDeskAvailability();
  }

  /**
   * Get monitor status text with validation
   */
  getMonitorStatus(desk: Desk): string {
    if (!desk || typeof desk.hasMonitor !== 'boolean') {
      return 'Unknown monitor status';
    }
    return desk.hasMonitor ? '🖥️ Monitor Available' : '❌ No Monitor';
  }

  /**
   * Clear all messages
   */
  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  /**
   * Get desk count by status for stats
   */
  getDeskCountByStatus(status: 'total' | 'available' | 'unavailable'): number {
    switch (status) {
      case 'total':
        return this.desks.length;
      case 'available':
        return this.availableDesks.length;
      case 'unavailable':
        return this.unavailableDesks.length;
      default:
        return 0;
    }
  }

  /**
   * Check if data is loaded and valid
   */
  isDataLoaded(): boolean {
    return !this.isLoading && this.desks.length > 0;
  }

  /**
   * Check if filters are applied
   */
  hasActiveFilters(): boolean {
    return !!(this.searchTerm || this.selectedLocation || this.showMonitorOnly);
  }

  /**
   * Get desks with monitors
   */
  getDesksWithMonitors(): Desk[] {
    return this.desks.filter(desk => desk.hasMonitor);
  }

  /**
   * Format date for HTML input (YYYY-MM-DD)
   */
  formatDateForInput(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  /**
   * Format date for API (dd/MM/yyyy)
   */
  formatDateForApi(dateString: string): string {
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  }

  /**
   * Format date for display (readable format)
   */
  formatDateForDisplay(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-GB', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  /**
   * Handle date change event
   */
  onDateChange(): void {
    if (this.selectedDate) {
      this.loadDeskAvailability();
    }
  }
}
