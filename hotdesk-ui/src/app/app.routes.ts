import { Routes } from '@angular/router';
import { Home } from './home/home';
import { BookingComponent } from './booking/booking';
import { DeskComponent } from './desk/desk';
import { UserComponent } from './user/user';

export const routes: Routes = [
  { path: '', component: Home, pathMatch: 'full', title: 'Hotdesk - Home' },
  { path: 'home', component: Home, title: 'Hotdesk - Home' },
  { path: 'bookings', component: BookingComponent, title: 'Hotdesk - Booking' },
  { path: 'desks', component: DeskComponent, title: 'Hotdesk - Desk' },
  { path: 'user', component: UserComponent, title: 'Hotdesk - User' }
];





