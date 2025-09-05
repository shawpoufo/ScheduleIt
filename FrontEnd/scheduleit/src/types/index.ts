export interface Customer {
  id: string;
  name: string;
  email: string;
}

export interface TodayUpcoming {
  appointmentId: string;
  customerId: string;
  startUtc: string;
  endUtc: string;
  customerName: string;
}

export interface TodayStats {
  totalAppointments: number;
  todayAppointments: number;
  upcomingToday: TodayUpcoming[];
}

export * from './appointments';


