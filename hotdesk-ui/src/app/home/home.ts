/*import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {

}*/
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  // Navigation expansion state
  isDeskNavExpanded = false; // Default to collapsed on home page

  // Toggle desk navigation expansion
  toggleDeskNav(): void {
    this.isDeskNavExpanded = !this.isDeskNavExpanded;
  }
}



