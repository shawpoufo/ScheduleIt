import React, { useState } from 'react';
import NewAppointmentModal from './NewAppointmentModalEnhanced';
import Sidebar from './Sidebar';

interface Appointment {
  id: string;
  customerName: string;
  service: string;
  date: string;
  status: 'Booked' | 'Canceled' | 'Completed';
}

interface DashboardProps {
  totalAppointments?: number;
  activeCustomers?: number;
  upcomingAppointments?: number;
  recentAppointments?: Appointment[];
}

const Dashboard: React.FC<DashboardProps> = ({
  totalAppointments = 245,
  activeCustomers = 120,
  upcomingAppointments = 15,
  recentAppointments = [
    {
      id: '1',
      customerName: 'Sophia Clark',
      service: 'Haircut',
      date: '2024-07-20 10:00 AM',
      status: 'Booked'
    },
    {
      id: '2',
      customerName: 'Ethan Carter',
      service: 'Massage',
      date: '2024-07-21 02:00 PM',
      status: 'Booked'
    },
    {
      id: '3',
      customerName: 'Olivia Bennett',
      service: 'Facial',
      date: '2024-07-22 11:00 AM',
      status: 'Canceled'
    },
    {
      id: '4',
      customerName: 'Liam Foster',
      service: 'Manicure',
      date: '2024-07-23 09:00 AM',
      status: 'Booked'
    },
    {
      id: '5',
      customerName: 'Ava Reynolds',
      service: 'Haircut',
      date: '2024-07-24 03:00 PM',
      status: 'Booked'
    }
  ]
}) => {
  const [showNewAppt, setShowNewAppt] = useState(false);
  const [lastCreatedId, setLastCreatedId] = useState<string | null>(null);
  const getStatusBadge = (status: string) => {
    const baseClasses = "inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium";
    
    switch (status) {
      case 'Booked':
        return `${baseClasses} bg-green-100 text-green-600`;
      case 'Canceled':
        return `${baseClasses} bg-red-100 text-red-600`;
      case 'Completed':
        return `${baseClasses} bg-blue-100 text-blue-600`;
      default:
        return `${baseClasses} bg-gray-100 text-gray-600`;
    }
  };

  return (
    <div className="flex h-screen bg-gray-50 text-gray-900">
      {/* Sidebar */}
      <Sidebar />

      {/* Main Content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <header className="h-16 bg-white border-b border-gray-200 flex items-center justify-between px-6">
          <div className="relative">
            <svg className="w-5 h-5 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
              <path d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2"></path>
            </svg>
            <input 
              className="pl-10 pr-4 py-2 w-80 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-600" 
              placeholder="Search by name/email/ID" 
              type="text"
            />
          </div>
          
          <div className="flex items-center gap-4">
            <button
              className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm font-semibold hover:bg-blue-700 transition-colors"
              onClick={() => setShowNewAppt(true)}
            >
              New Appointment
            </button>
            <div className="relative">
              <button className="focus:outline-none">
                <img 
                  alt="User avatar" 
                  className="h-10 w-10 rounded-full object-cover" 
                  src="https://lh3.googleusercontent.com/aida-public/AB6AXuAm2QKLD3OnpbWyXzQ8Y_zwUg1Q0Y49WLGcUkz3D1dgEC30jDyNfwxOBcjqCOxf3rvmme7Pdt9wm39Fqzz92x3I_PCpxkxuaXxMokTXiDGgH0ovIB-gYSgFC8yT9RXXYqmSv7XRjukJ35Yy2npYSDvDbhB1uasdRr3S0bCOITldgURQdj3v_Vji2UEZiUKK5qfb8OYjMo25SyUKtV203TQ4KtzsvA3C9Pxx3_jS64qXTR7GP1qBQ6maOkIP4fRKAyclTrVjk52WhEf0"
                />
              </button>
            </div>
          </div>
        </header>

        {/* Main Content Area */}
        <main className="flex-1 overflow-x-hidden overflow-y-auto bg-gray-50 p-6">
          {lastCreatedId && (
            <div className="mb-4 rounded-md bg-green-50 border border-green-200 px-4 py-2 text-sm text-green-700">
              Appointment created successfully. ID: {lastCreatedId}
            </div>
          )}
          <h1 className="text-3xl font-bold mb-6">Dashboard</h1>
          
          {/* Overview Section */}
          <h2 className="text-xl font-semibold mb-4">Overview</h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
            <div className="bg-white p-6 rounded-lg border border-gray-200 shadow-sm">
              <p className="text-sm font-medium text-gray-600">Total Appointments</p>
              <p className="text-3xl font-bold mt-1">{totalAppointments}</p>
            </div>
            <div className="bg-white p-6 rounded-lg border border-gray-200 shadow-sm">
              <p className="text-sm font-medium text-gray-600">Active Customers</p>
              <p className="text-3xl font-bold mt-1">{activeCustomers}</p>
            </div>
            <div className="bg-white p-6 rounded-lg border border-gray-200 shadow-sm">
              <p className="text-sm font-medium text-gray-600">Upcoming Appointments</p>
              <p className="text-3xl font-bold mt-1">{upcomingAppointments}</p>
            </div>
          </div>

          {/* Recent Activity Section */}
          <h2 className="text-xl font-semibold mb-4">Recent Activity</h2>
          <div className="bg-white rounded-lg border border-gray-200 overflow-hidden shadow-sm">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase tracking-wider" scope="col">
                    Customer
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase tracking-wider" scope="col">
                    Service
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase tracking-wider" scope="col">
                    Date
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase tracking-wider" scope="col">
                    Status
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {recentAppointments.map((appointment) => (
                  <tr key={appointment.id} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {appointment.customerName}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {appointment.service}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-600">
                      {appointment.date}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <span className={getStatusBadge(appointment.status)}>
                        {appointment.status}
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </main>
      </div>
      <NewAppointmentModal
        open={showNewAppt}
        onClose={() => setShowNewAppt(false)}
        onCreated={(id) => setLastCreatedId(id || null)}
      />
    </div>
  );
};

export default Dashboard;
