export type AppointmentStatus = 'Scheduled' | 'InProgress' | 'Completed' | 'Canceled' | 'NoShow';

export interface AppointmentEvent {
  id: string;
  title: string;
  start: Date;
  end: Date;
  status: AppointmentStatus;
  notes?: string;
}


