export interface Desk {
  id?: number;
  deskNumber: string;
  floor: number;
  location: string;
  isAvailable: boolean;
  hasMonitor: boolean;
  hasKeyboard: boolean;
  hasMouse: boolean;
  description?: string;
  createdDate?: Date;
  updatedDate?: Date;
}

export interface CreateDeskRequest {
  deskNumber: string;
  floor: number;
  location: string;
  isAvailable: boolean;
  hasMonitor: boolean;
  hasKeyboard: boolean;
  hasMouse: boolean;
  description?: string;
}

export interface UpdateDeskRequest {
  id: number;
  deskNumber: string;
  floor: number;
  location: string;
  isAvailable: boolean;
  hasMonitor: boolean;
  hasKeyboard: boolean;
  hasMouse: boolean;
  description?: string;
}
