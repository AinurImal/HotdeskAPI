// Import Angular core components and lifecycle hooks
import { Component, OnInit } from '@angular/core';
// Import routing functionality for navigation
import { RouterModule } from '@angular/router';
// Import common Angular directives (ngIf, ngFor, etc.)
import { CommonModule } from '@angular/common';
// Import forms module for two-way data binding with ngModel
import { FormsModule } from '@angular/forms';
// Import our custom service for API operations
import { UserService } from './user.service';
// Import TypeScript interfaces, alias User to avoid naming conflict
import { User as UserModel, CreateUserRequest, UpdateUserRequest } from './user.model';

// Component decorator defining the user management component
@Component({
  selector: 'app-user',           // HTML tag to use this component
  standalone: true,               // Component doesn't need to be declared in module
  imports: [RouterModule, CommonModule, FormsModule],  // Required Angular modules
  templateUrl: './user.html',     // Path to HTML template
  styleUrl: './user.css'          // Path to component styles
})
export class UserComponent implements OnInit {
  // Array to store all users fetched from API
  users: UserModel[] = [];
  // Currently selected user for editing (null when none selected)
  selectedUser: UserModel | null = null;
  // Boolean flag to show loading spinner during API calls
  isLoading = false;
  // String to display error messages to user
  errorMessage = '';
  // Boolean flag to track if we're in edit mode vs create mode
  isEditing = false;

  // Form data object for creating new users
  newUser: CreateUserRequest = {
    fullName: '',     // Bound to full name input field
    userName: '',     // Bound to username input field
    phoneNumber: '',  // Bound to phone number input field
    email: ''         // Bound to email input field
  };

  // Form data object for updating existing users
  editUser: UpdateUserRequest = {
    userId: '',       // ID of user being edited
    fullName: '',     // Updated full name value
    userName: '',     // Updated username value
    phoneNumber: '',  // Updated phone number value
    email: ''         // Updated email value
  };

  // Constructor with dependency injection of UserService
  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  // Load all users
  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.userService.getUsers().subscribe({
      next: (users) => {
        this.users = users;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  // Create a new user
  createUser(): void {
    if (!this.newUser.fullName || !this.newUser.email || !this.newUser.userName || !this.newUser.phoneNumber) {
      this.errorMessage = 'Full Name, Username, Phone Number, and Email are required';
      return;
    }

    this.isLoading = true;
    this.userService.createUser(this.newUser).subscribe({
      next: (user) => {
        this.users.push(user);
        this.resetForm();
        this.isLoading = false;
        this.errorMessage = '';
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  // Start editing a user
  startEdit(user: UserModel): void {
    this.isEditing = true;
    this.editUser = {
      userId: user.userId!,
      fullName: user.fullName,
      userName: user.userName,
      phoneNumber: user.phoneNumber,
      email: user.email
    };
  }

  // Update user
  updateUser(): void {
    this.isLoading = true;
    this.userService.updateUser(this.editUser).subscribe({
      next: (updatedUser) => {
        const index = this.users.findIndex(u => u.userId === updatedUser.userId);
        if (index !== -1) {
          this.users[index] = updatedUser;
        }
        this.cancelEdit();
        this.isLoading = false;
        this.errorMessage = '';
      },
      error: (error) => {
        this.errorMessage = error;
        this.isLoading = false;
      }
    });
  }

  // Delete user
  deleteUser(id: string): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.isLoading = true;
      this.userService.deleteUser(id).subscribe({
        next: () => {
          this.users = this.users.filter(u => u.userId !== id);
          this.isLoading = false;
          this.errorMessage = '';
        },
        error: (error) => {
          this.errorMessage = error;
          this.isLoading = false;
        }
      });
    }
  }

  // Get user by ID (example of individual GET)
  getUserById(id: string): void {
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        this.selectedUser = user;
        console.log('Selected user:', user);
      },
      error: (error) => {
        this.errorMessage = error;
      }
    });
  }

  // Helper methods
  resetForm(): void {
    this.newUser = {
      fullName: '',
      userName: '',
      phoneNumber: '',
      email: ''
    };
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.editUser = {
      userId: '',
      fullName: '',
      userName: '',
      phoneNumber: '',
      email: ''
    };
  }
}






