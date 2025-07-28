/*import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {

}*/
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// Import services for dashboard data
import { DeskService } from '../desk/desk.service';
import { UserService } from '../user/user.service';
import { BookingService } from '../booking/booking.service';

// Import models
import { Desk } from '../desk/desk.model';
import { User } from '../user/user.model';
import { Booking } from '../booking/booking.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
  // Navigation expansion state
  isDeskNavExpanded = false; // Default to collapsed on home page

  // Dashboard statistics
  totalDesks = 0;
  availableDesks = 0;
  totalBookings = 0;
  totalUsers = 0;

  // Recent activity data
  recentBookings: any[] = [];

  // System status
  apiStatus: 'online' | 'offline' = 'online';
  databaseStatus: 'online' | 'offline' = 'online';

  // Loading state
  isLoading = true;

  constructor(
    private deskService: DeskService,
    private userService: UserService,
    private bookingService: BookingService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  // Toggle desk navigation expansion
  toggleDeskNav(): void {
    this.isDeskNavExpanded = !this.isDeskNavExpanded;
  }

  // Load all dashboard data
  private loadDashboardData(): void {
    this.isLoading = true;
    
    // Load statistics in parallel
    Promise.all([
      this.loadDeskStats(),
      this.loadUserStats(),
      this.loadBookingStats(),
      this.loadRecentActivity()
    ]).then(() => {
      this.isLoading = false;
      this.apiStatus = 'online';
      this.databaseStatus = 'online';
    }).catch(error => {
      console.error('Error loading dashboard data:', error);
      this.isLoading = false;
      this.apiStatus = 'offline';
      this.databaseStatus = 'offline';
    });
  }

  // Load desk statistics
  private loadDeskStats(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.deskService.getDesks().subscribe({
        next: (desks: Desk[]) => {
          this.totalDesks = desks.length;
          this.availableDesks = desks.filter(desk => desk.isAvailable).length;
          resolve();
        },
        error: (error) => {
          console.error('Error loading desk stats:', error);
          resolve(); // Continue with other data even if this fails
        }
      });
    });
  }

  // Load user statistics
  private loadUserStats(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.userService.getUsers().subscribe({
        next: (users: User[]) => {
          this.totalUsers = users.length;
          resolve();
        },
        error: (error) => {
          console.error('Error loading user stats:', error);
          resolve(); // Continue with other data even if this fails
        }
      });
    });
  }

  // Load booking statistics
  private loadBookingStats(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.bookingService.getBookings().subscribe({
        next: (bookings: Booking[]) => {
          this.totalBookings = bookings.length;
          resolve();
        },
        error: (error) => {
          console.error('Error loading booking stats:', error);
          resolve(); // Continue with other data even if this fails
        }
      });
    });
  }

  // Load recent booking activity
  private loadRecentActivity(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.bookingService.getBookings().subscribe({
        next: (bookings: Booking[]) => {
          // Get the 5 most recent bookings and add desk names
          this.recentBookings = bookings
            .sort((a, b) => new Date(b.bookingDate).getTime() - new Date(a.bookingDate).getTime())
            .slice(0, 5)
            .map(booking => ({
              ...booking,
              deskName: `Desk ${booking.deskId}` // Simplified desk name
            }));
          resolve();
        },
        error: (error) => {
          console.error('Error loading recent activity:', error);
          this.recentBookings = [];
          resolve(); // Continue even if this fails
        }
      });
    });
  }

  // Refresh dashboard data
  refreshDashboard(): void {
    this.loadDashboardData();
  }
}



