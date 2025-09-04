import React from 'react';

interface HeaderProps {
  onNewAppointment: () => void;
}

const Header: React.FC<HeaderProps> = ({ onNewAppointment }) => {
  return (
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
          className="rounded-lg px-4 py-2 bg-blue-600 text-white text-sm font-semibold transition-colors hover:bg-blue-700"
          onClick={onNewAppointment}
        >
          New Appointment
        </button>
      </div>
    </header>
  );
};

export default Header;
