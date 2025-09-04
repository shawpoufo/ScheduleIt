import React, { useEffect, useRef, useState } from 'react';
import CustomerSearchInput from './CustomerSearchInput';
import customerService, { type Customer } from '../services/customerService';
import { API_BASE_URL } from '../config/api';

type NewAppointmentModalProps = {
  open: boolean;
  onClose: () => void;
  onCreated?: (appointmentId: string) => void;
  apiBaseUrl?: string;
};

export default function NewAppointmentModal({ 
  open, 
  onClose, 
  onCreated, 
  apiBaseUrl 
}: NewAppointmentModalProps) {
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);
  const [startLocal, setStartLocal] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const firstFieldRef = useRef<HTMLInputElement>(null);

  const baseUrl = apiBaseUrl || API_BASE_URL;

  useEffect(() => {
    if (open) {
      setError(null);
      // Focus first field when opening
      setTimeout(() => firstFieldRef.current?.focus(), 0);
    }
  }, [open]);

  const resetForm = () => {
    setSelectedCustomer(null);
    setStartLocal('');
    setError(null);
  };

  const close = () => {
    if (submitting) return;
    resetForm();
    onClose();
  };

  const handleCustomerSearch = async (query: string): Promise<Customer[]> => {
    try {
      const customers = await customerService.searchCustomers(query);
      return customers;
    } catch (error) {
      console.error('Failed to search customers:', error);
      return [];
    }
  };

  const validate = () => {
    if (!selectedCustomer) return 'Please select a customer';
    if (!startLocal) return 'Start date/time is required';
    
    // Ensure valid date
    const dt = new Date(startLocal);
    if (isNaN(dt.getTime())) return 'Start date/time is invalid';
    
    // Check if date is in the past
    if (dt < new Date()) return 'Appointment time cannot be in the past';
    
    return null;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (submitting) return;
    
    const validationError = validate();
    if (validationError) {
      setError(validationError);
      return;
    }

    setSubmitting(true);
    setError(null);
    
    try {
      // Convert local datetime to UTC ISO string
      const startUtcIso = new Date(startLocal).toISOString();
      
      const response = await fetch(`${baseUrl}/api/appointments`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 
          customerId: selectedCustomer!.id, 
          startUtc: startUtcIso 
        }),
      });

      if (!response.ok) {
        const text = await response.text().catch(() => '');
        throw new Error(text || `Request failed with status ${response.status}`);
      }

      const data = await response.json().catch(() => ({}));
      const appointmentId = data?.id as string | undefined;
      
      onCreated?.(appointmentId || '');
      close();
    } catch (err: any) {
      setError(err?.message || 'Failed to create appointment');
    } finally {
      setSubmitting(false);
    }
  };

  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center">
      {/* Overlay - Exact same styling */}
      <div
        className="absolute inset-0 bg-black/50"
        onClick={close}
        aria-hidden="true"
      />

      {/* Dialog - Exact same styling */}
      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="new-appointment-title"
        className="relative z-10 w-full max-w-lg mx-4"
      >
        <div className="bg-white rounded-lg shadow-lg border border-gray-200">
          {/* Header - Exact same styling */}
          <div className="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
            <h2 id="new-appointment-title" className="text-lg font-semibold text-gray-900">
              New Appointment
            </h2>
            <button
              type="button"
              onClick={close}
              className="inline-flex items-center justify-center h-8 w-8 rounded-md text-gray-500 hover:bg-gray-100 focus:outline-none"
              aria-label="Close"
            >
              <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
              </svg>
            </button>
          </div>

          {/* Form - Exact same styling with enhanced customer selection */}
          <form onSubmit={handleSubmit} className="px-6 py-4 space-y-4">
            {/* Error Display - Exact same styling */}
            {error && (
              <div className="rounded-md bg-red-50 border border-red-200 px-4 py-2 text-sm text-red-700">
                {error}
              </div>
            )}

            {/* Enhanced Customer Selection */}
            <div>
              <label htmlFor="customer-search" className="block text-sm font-medium text-gray-700">
                Customer
              </label>
              <CustomerSearchInput
                value={selectedCustomer}
                onChange={setSelectedCustomer}
                onSearch={handleCustomerSearch}
                placeholder="Search by name or email..."
                required
                disabled={submitting}
              />
              <p className="mt-1 text-xs text-gray-500">
                Search and select an existing customer.
              </p>
              
              {/* Selected Customer Display */}
              {selectedCustomer && (
                <div className="mt-2 p-3 bg-blue-50 border border-blue-200 rounded-lg">
                  <div className="flex items-center gap-2">
                    <svg className="h-4 w-4 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                    </svg>
                    <div>
                      <div className="text-sm font-medium text-blue-900">
                        {selectedCustomer.name}
                      </div>
                      <div className="text-xs text-blue-700">
                        {selectedCustomer.email}
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>

            {/* DateTime Field - Exact same styling */}
            <div>
              <label htmlFor="startLocal" className="block text-sm font-medium text-gray-700">
                Start (local time)
              </label>
              <input
                id="startLocal"
                type="datetime-local"
                value={startLocal}
                onChange={(e) => setStartLocal(e.target.value)}
                min={new Date().toISOString().slice(0, 16)} // Prevent past dates
                className="mt-1 block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                required
                disabled={submitting}
              />
              <p className="mt-1 text-xs text-gray-500">
                Will be sent to API as UTC (ISO 8601).
              </p>
            </div>

            {/* Action Buttons - Exact same styling */}
            <div className="pt-2 flex items-center justify-end gap-3">
              <button
                type="button"
                className="px-4 py-2 rounded-lg text-sm font-semibold text-gray-700 bg-gray-100 hover:bg-gray-200"
                onClick={close}
                disabled={submitting}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="bg-[var(--primary-color)] text-white px-4 py-2 rounded-lg text-sm font-semibold hover:brightness-95 transition-colors disabled:opacity-60"
                disabled={submitting || !selectedCustomer}
              >
                {submitting ? (
                  <div className="flex items-center gap-2">
                    <svg className="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Creating...
                  </div>
                ) : (
                  'Create Appointment'
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
