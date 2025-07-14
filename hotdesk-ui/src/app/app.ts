import { Component } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { Home } from './home/home'; // <-- Import your Home component

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, RouterOutlet, Home], // <-- Add Home here
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'hotdesk-ui';
}

