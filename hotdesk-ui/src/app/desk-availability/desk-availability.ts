import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DeskService } from '../desk/desk.service';
import { Desk } from '../desk/desk.model';

@Component({
  selector: 'app-desk-availability',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './desk-availability.html',
  styleUrl: './desk-availability.css'
})
export class DeskAvailabilityComponent implements OnInit {
  desks: Desk[] = [];
  availableDesks: Desk[] = [];
  unavailableDesks: Desk[] = [];
  isLoading = false;
  errorMessage = '';
  searchTerm = '';
  selectedLocation = '';
  locations: string[] = [];
  showMonitorOnly = false;
  
  // Expand state for navigation
  isDeskNavExpanded = true; // Default to expanded since we're on a desk page

  constructor(private deskService: DeskService) {}

  ngOnInit(): void {
    this.loadDesks();
  }

  /**
   * Load all desks and categorize them by availability
   */
  loadDesks(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.deskService.getDesks().subscribe({
      next: (desks) => {
        this.desks = desks;
        this.categorizeDesks();
        this.extractLocations();
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = 'Failed to load desk availability. Please try again.';
        this.isLoading = false;
      }
    });
  }

  /**
   * Categorize desks by availability status
   */
  private categorizeDesks(): void {
    this.availableDesks = this.desks.filter(desk => desk.isAvailable);
    this.unavailableDesks = this.desks.filter(desk => !desk.isAvailable);
  }

  /**
   * Extract unique locations from desks
   */
  private extractLocations(): void {
    this.locations = [...new Set(this.desks.map(desk => desk.location))];
  }

  /**
   * Filter desks based on search criteria
   */
  getFilteredDesks(desks: Desk[]): Desk[] {
    return desks.filter(desk => {
      const matchesSearch = !this.searchTerm || 
        desk.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        desk.location.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      const matchesLocation = !this.selectedLocation || desk.location === this.selectedLocation;
      const matchesMonitor = !this.showMonitorOnly || desk.hasMonitor;
      
      return matchesSearch && matchesLocation && matchesMonitor;
    });
  }

  /**
   * Toggle desk navigation expansion
   */
  toggleDeskNav(): void {
    this.isDeskNavExpanded = !this.isDeskNavExpanded;
  }

  /**
   * Reset all filters
   */
  resetFilters(): void {
    this.searchTerm = '';
    this.selectedLocation = '';
    this.showMonitorOnly = false;
  }

  /**
   * Refresh desk data
   */
  refreshDesks(): void {
    this.loadDesks();
  }

  /**
   * Get desk status badge class
   */
  getDeskStatusClass(desk: Desk): string {
    return desk.isAvailable ? 'status-available' : 'status-unavailable';
  }

  /**
   * Get monitor status text
   */
  getMonitorStatus(desk: Desk): string {
    return desk.hasMonitor ? '🖥️ Monitor Available' : '📺 No Monitor';
  }
}
