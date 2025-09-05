import React from 'react';
import type { AppointmentStatus } from '../types/appointments';

interface StatusBadgeProps {
  status: AppointmentStatus;
  className?: string;
}

const baseClasses = 'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium';

const statusClassMap: Record<AppointmentStatus, string> = {
  Scheduled: `${baseClasses} text-[var(--primary-color)] border border-[var(--primary-color)] bg-[var(--accent-color)]`,
  InProgress: `${baseClasses} text-white border border-[var(--primary-color)] bg-[var(--primary-color)]`,
  Completed: `${baseClasses} bg-blue-100 text-blue-600`,
  Canceled: `${baseClasses} bg-red-100 text-red-600`,
  NoShow: `${baseClasses} text-gray-700 border border-gray-400 bg-gray-200`,
};

const StatusBadge: React.FC<StatusBadgeProps> = ({ status, className = '' }) => {
  const classes = statusClassMap[status] || baseClasses;
  return <span className={`${classes} ${className}`.trim()}>{status}</span>;
};

export default StatusBadge;


