import React from 'react';

type StatusKind = 'Booked' | 'Canceled' | 'Completed' | 'Scheduled' | 'InProgress' | 'NoShow';

interface StatusBadgeProps {
  status: StatusKind;
  className?: string;
}

const baseClasses = 'inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium';

const statusClassMap: Record<StatusKind, string> = {
  Booked: `${baseClasses} bg-green-100 text-green-600`,
  Canceled: `${baseClasses} bg-red-100 text-red-600`,
  Completed: `${baseClasses} bg-blue-100 text-blue-600`,
  Scheduled: 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-[var(--primary-color)] border-[var(--primary-color)] bg-[var(--accent-color)]',
  InProgress: 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-white border-[var(--primary-color)] bg-[var(--primary-color)]',
  NoShow: 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-gray-700 border-gray-400 bg-gray-200',
};

const StatusBadge: React.FC<StatusBadgeProps> = ({ status, className = '' }) => {
  const classes = statusClassMap[status] || baseClasses;
  return <span className={`${classes} ${className}`.trim()}>{status}</span>;
};

export default StatusBadge;


