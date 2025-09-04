import React, { useState } from 'react';
import { Outlet } from 'react-router-dom';
import Sidebar from './Sidebar';
import Header from './Header';
import NewAppointmentModal from './NewAppointmentModal';

const Layout: React.FC = () => {
  const [showNewAppt, setShowNewAppt] = useState(false);

  return (
    <div className="flex h-screen bg-gray-50 text-gray-900">
      {/* Sidebar */}
      <Sidebar />

      {/* Main Content */}
      <div className="flex-1 flex flex-col overflow-hidden min-h-0">
        {/* Header */}
        <Header onNewAppointment={() => setShowNewAppt(true)} />

        {/* Page Content */}
        <Outlet />

        {/* New Appointment Modal */}
        <NewAppointmentModal
          open={showNewAppt}
          onClose={() => setShowNewAppt(false)}
          onCreated={() => setShowNewAppt(false)}
        />
      </div>
    </div>
  );
};

export default Layout;
