import React, { useState } from 'react';
import StatusBadge from '../components/StatusBadge';

interface Appointment {
  id: string;
  customerName: string;
  service: string;
  date: string;
  status: 'Booked' | 'Canceled' | 'Completed';
}

const DashboardPage: React.FC = () => {
  const [lastCreatedId, setLastCreatedId] = useState<string | null>(null);
  
  const totalAppointments = 245;
  const activeCustomers = 120;
  const upcomingAppointments = 15;
  
  const recentAppointments: Appointment[] = [
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
  ];

  // badge handled by StatusBadge component

  return (
    <div className="flex-1 overflow-x-hidden overflow-y-auto bg-gray-50 p-6">
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
                  <StatusBadge status={appointment.status as any} />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>


    </div>
  );
};

export default DashboardPage;
