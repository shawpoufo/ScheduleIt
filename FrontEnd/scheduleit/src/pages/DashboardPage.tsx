import React from 'react';
import { API_BASE_URL } from '../config/api';
import { format } from 'date-fns';
import type { TodayStats } from '../types';
import { fetchJson } from '../services/http';

const DashboardPage: React.FC = () => {
  const [isLoading, setIsLoading] = React.useState<boolean>(false);
  const [error, setError] = React.useState<string | null>(null);
  const [stats, setStats] = React.useState<TodayStats | null>(null);

  React.useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const data = await fetchJson<TodayStats>(`${API_BASE_URL}/api/appointments/stats/today`);
        setStats(data);
      } catch (e: any) {
        setError(e?.message || 'Failed to load dashboard stats');
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, []);

  return (
    <div className="flex-1 overflow-x-hidden overflow-y-auto bg-gray-50 p-6">
      {error && (
        <div className="mb-4 rounded-md bg-rose-50 border border-rose-200 px-4 py-2 text-sm text-rose-700">{error}</div>
      )}
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>
      
      <h2 className="text-xl font-semibold mb-4">Overview</h2>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        <div className="bg-white p-6 rounded-lg border border-gray-200 shadow-sm">
          <p className="text-sm font-medium text-gray-600">Total Appointments</p>
          <p className="text-3xl font-bold mt-1">{stats ? stats.totalAppointments : '—'}</p>
        </div>
        <div className="bg-white p-6 rounded-lg border border-gray-200 shadow-sm">
          <p className="text-sm font-medium text-gray-600">Appointments Today</p>
          <p className="text-3xl font-bold mt-1">{stats ? stats.todayAppointments : '—'}</p>
        </div>
        <div className="bg-white p-6 rounded-lg border border-gray-200 shadow-sm">
          <p className="text-sm font-medium text-gray-600">Upcoming Today</p>
          <p className="text-3xl font-bold mt-1">{stats ? stats.upcomingToday.length : '—'}</p>
        </div>
      </div>

      <h2 className="text-xl font-semibold mb-4">Upcoming Today</h2>
      <div className="bg-white rounded-lg border border-gray-200 overflow-hidden shadow-sm">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase tracking-wider" scope="col">Start</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase tracking-wider" scope="col">End</th>
              <th className="px-6 py-3 text-left text-xs font-medium text-gray-600 uppercase tracking-wider" scope="col">Customer</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {isLoading && (<tr><td className="px-6 py-4 text-sm text-gray-500" colSpan={3}>Loading…</td></tr>)}
            {!isLoading && stats && stats.upcomingToday.length === 0 && (
              <tr><td className="px-6 py-4 text-sm text-gray-500" colSpan={3}>No upcoming appointments today</td></tr>
            )}
            {!isLoading && stats && stats.upcomingToday.map(u => (
              <tr key={u.appointmentId} className="hover:bg-gray-50">
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">{format(new Date(u.startUtc), 'PPpp')}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">{format(new Date(u.endUtc), 'PPpp')}</td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-700">{u.customerName || u.customerId}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>


    </div>
  );
};

export default DashboardPage;
