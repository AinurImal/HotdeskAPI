import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DeskService } from './desk.service';
import { Desk, CreateDeskRequest, UpdateDeskRequest } from './desk.model';

@Component({
  selector: 'app-desk',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './desk.html',
  styleUrl: './desk.css'
})
export class DeskComponent implements OnInit {
  desks: Desk[] = [];
  deskForm: FormGroup;
  isEditMode = false;
  editingDeskId: number | null = null;
  errorMessage: string = '';
  successMessage: string = '';
  isLoading = false;

  constructor(
    private deskService: DeskService,
    private formBuilder: FormBuilder
  ) {
    // Initialize the reactive form with validation rules
    this.deskForm = this.formBuilder.group({
      deskId: ['', [Validators.pattern(/^\d+$/)]],
      name: ['', [Validators.required, Validators.minLength(1)]],
      location: ['', [Validators.required, Validators.minLength(2)]],
      hasMonitor: [false],
      isAvailable: [true],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.loadDesks();
  }

  // Load all desks from the API
  loadDesks(): void {
    this.isLoading = true;
    this.clearMessages();
    
    this.deskService.getDesks().subscribe({
      next: (desks) => {
        this.desks = desks;
        this.isLoading = false;
        this.successMessage = `Successfully loaded ${desks.length} desk(s)`;
      },
      error: (error) => {
        this.errorMessage = error.message;
        this.isLoading = false;
        console.error('Error loading desks:', error);
      }
    });
  }

  // Submit form to either create new desk or update existing desk
  onSubmit(): void {
    if (this.deskForm.valid) {
      this.isLoading = true;
      this.clearMessages();

      if (this.isEditMode && this.editingDeskId !== null) {
        this.updateDesk();
      } else {
        this.createDesk();
      }
    } else {
      this.errorMessage = 'Please fill in all required fields correctly.';
      this.markFormGroupTouched(this.deskForm);
    }
  }

  // Create a new desk
  private createDesk(): void {
    const formValue = this.deskForm.value;
    const createRequest: CreateDeskRequest = {
      name: formValue.name,
      location: formValue.location,
      hasMonitor: formValue.hasMonitor,
      isAvailable: formValue.isAvailable,
      description: formValue.description
    };
    
    // Add deskId only if it's provided (not empty)
    if (formValue.deskId && formValue.deskId.toString().trim() !== '') {
      (createRequest as any).deskId = parseInt(formValue.deskId);
    }
    
    this.deskService.createDesk(createRequest).subscribe({
      next: (newDesk) => {
        this.desks.push(newDesk);
        this.resetForm();
        this.successMessage = `Desk "${newDesk.name}" created successfully!`;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.message;
        this.isLoading = false;
        console.error('Error creating desk:', error);
      }
    });
  }

  // Update an existing desk
  private updateDesk(): void {
    if (this.editingDeskId === null) return;

    const updateRequest: UpdateDeskRequest = {
      deskId: this.editingDeskId,
      ...this.deskForm.value
    };
    
    this.deskService.updateDesk(this.editingDeskId, updateRequest).subscribe({
      next: (updatedDesk) => {
        const index = this.desks.findIndex(d => d.deskId === this.editingDeskId);
        if (index !== -1) {
          this.desks[index] = updatedDesk;
        }
        this.resetForm();
        this.successMessage = `Desk "${updatedDesk.name}" updated successfully!`;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.message;
        this.isLoading = false;
        console.error('Error updating desk:', error);
      }
    });
  }

  // Prepare form for editing an existing desk
  editDesk(desk: Desk): void {
    this.isEditMode = true;
    this.editingDeskId = desk.deskId || null;
    this.clearMessages();
    
    this.deskForm.patchValue({
      deskId: desk.deskId || '',
      name: desk.name,
      location: desk.location,
      hasMonitor: desk.hasMonitor,
      isAvailable: desk.isAvailable,
      description: desk.description || ''
    });
  }

  // Delete a desk by ID
  deleteDesk(desk: Desk): void {
    if (!desk.deskId) {
      this.errorMessage = 'Cannot delete desk: Invalid desk ID';
      return;
    }

    if (confirm(`Are you sure you want to delete desk "${desk.name}"? This action cannot be undone.`)) {
      this.isLoading = true;
      this.clearMessages();
      
      this.deskService.deleteDesk(desk.deskId).subscribe({
        next: () => {
          this.desks = this.desks.filter(d => d.deskId !== desk.deskId);
          this.successMessage = `Desk "${desk.name}" deleted successfully!`;
          this.isLoading = false;
        },
        error: (error) => {
          this.errorMessage = error.message;
          this.isLoading = false;
          console.error('Error deleting desk:', error);
        }
      });
    }
  }

  // Get a specific desk by ID (for viewing details)
  getDeskById(deskId: number): void {
    this.isLoading = true;
    this.clearMessages();
    
    this.deskService.getDeskById(deskId).subscribe({
      next: (desk) => {
        this.successMessage = `Found desk: ${desk.name}`;
        this.isLoading = false;
      },
      error: (error) => {
        this.errorMessage = error.message;
        this.isLoading = false;
        console.error('Error getting desk by ID:', error);
      }
    });
  }

  // Reset form to initial state
  resetForm(): void {
    this.isEditMode = false;
    this.editingDeskId = null;
    this.deskForm.reset({
      deskId: '',
      name: '',
      location: '',
      hasMonitor: false,
      isAvailable: true,
      description: ''
    });
    this.clearMessages();
  }

  // Clear all messages
  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  // Mark all form fields as touched to show validation errors
  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach(key => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }

  // Helper method to check if a form field has errors and is touched
  isFieldInvalid(fieldName: string): boolean {
    const field = this.deskForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  // Get error message for a specific field
  getFieldError(fieldName: string): string {
    const field = this.deskForm.get(fieldName);
    if (field && field.errors && field.touched) {
      if (field.errors['required']) {
        return `${fieldName} is required`;
      }
      if (field.errors['minlength']) {
        return `${fieldName} must be at least ${field.errors['minlength'].requiredLength} characters`;
      }
      if (field.errors['min']) {
        return `${fieldName} must be at least ${field.errors['min'].min}`;
      }
      if (field.errors['pattern']) {
        return `${fieldName} must be a valid number`;
      }
    }
    return '';
  }
}



