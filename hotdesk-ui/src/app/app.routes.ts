import { Routes } from '@angular/router';
import { Home } from './home/home';
import { Booking } from './booking/booking';
import { Desk } from './desk/desk';
import { User } from './user/user';

export const routes: Routes = [
  { path: '', component: Home, pathMatch: 'full', title: 'Hotdesk - Home' },
  { path: 'home', component: Home, title: 'Hotdesk - Home' },
  { path: 'bookings', component: Booking, title: 'Hotdesk - Booking' },
  { path: 'desks', component: Desk, title: 'Hotdesk - Desk' },
  { path: 'user', component: User, title: 'Hotdesk - User' }
];





