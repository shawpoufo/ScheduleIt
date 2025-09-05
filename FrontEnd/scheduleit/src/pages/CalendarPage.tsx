import React, { useMemo, useState } from 'react';
import { Calendar, dateFnsLocalizer, Views } from 'react-big-calendar';
import type { View } from 'react-big-calendar';
import 'react-big-calendar/lib/css/react-big-calendar.css';
import { format, parse, startOfWeek, getDay, addMinutes, startOfMonth, endOfMonth, endOfWeek, startOfDay, endOfDay } from 'date-fns';
import { enUS } from 'date-fns/locale';
import { API_BASE_URL } from '../config/api';

type AppointmentStatus = 'Scheduled' | 'Canceled' | 'Completed' | 'InProgress' | 'NoShow';

interface AppointmentEvent {
  id: string;
  title: string;
  start: Date;
  end: Date;
  status: AppointmentStatus;
  notes?: string;
}

const locales = {
  'en-US': enUS,
};

const localizer = dateFnsLocalizer({
  format,
  parse,
  startOfWeek: (date: Date) => startOfWeek(date, { weekStartsOn: 0 }),
  getDay,
  locales,
});


const CalendarPage: React.FC = () => {
  const [selected, setSelected] = useState<AppointmentEvent | null>(null);
  const [statusSaving, setStatusSaving] = useState<boolean>(false);
  const [pendingStatus, setPendingStatus] = useState<AppointmentStatus | null>(null);
  const [statusError, setStatusError] = useState<string | null>(null);
  const [isDeleting, setIsDeleting] = useState<boolean>(false);
  const [deleteError, setDeleteError] = useState<string | null>(null);
  const statusOptions: AppointmentStatus[] = ['Scheduled','InProgress','Completed','Canceled','NoShow'];
  const [view, setView] = useState<View>(Views.MONTH);
  const [date, setDate] = useState<Date>(new Date());
  const [events, setEvents] = useState<AppointmentEvent[]>([]);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [loadError, setLoadError] = useState<string | null>(null);

  type Range = { startUtc: string; endUtc: string };
  const computeRange = (v: View, d: Date): Range => {
    switch (v) {
      case Views.DAY: {
        const s = startOfDay(d);
        const e = endOfDay(d);
        return { startUtc: s.toISOString(), endUtc: e.toISOString() };
      }
      case Views.WEEK: {
        const s = startOfWeek(d, { weekStartsOn: 0 });
        const e = endOfWeek(d, { weekStartsOn: 0 });
        return { startUtc: s.toISOString(), endUtc: e.toISOString() };
      }
      case Views.AGENDA:
      case Views.MONTH:
      default: {
        const s = startOfWeek(startOfMonth(d), { weekStartsOn: 0 });
        const e = endOfWeek(endOfMonth(d), { weekStartsOn: 0 });
        return { startUtc: s.toISOString(), endUtc: e.toISOString() };
      }
    }
  };

  type ApiAppointment = { appointmentId: string; customerId: string; startUtc: string; endUtc: string; status: AppointmentStatus; notes: string };

  const fetchAppointments = async (v: View, d: Date) => {
    const { startUtc, endUtc } = computeRange(v, d);
    setIsLoading(true);
    setLoadError(null);
    try {
      const res = await fetch(`${API_BASE_URL}/api/appointments/range?startUtc=${encodeURIComponent(startUtc)}&endUtc=${encodeURIComponent(endUtc)}`);
      if (!res.ok) throw new Error('Failed to load appointments');
      const data: ApiAppointment[] = await res.json();
      const mapped: AppointmentEvent[] = data.map(a => ({
        id: a.appointmentId,
        title: 'Appointment',
        start: new Date(a.startUtc),
        end: new Date(a.endUtc),
        status: a.status,
        notes: a.notes,
      }));
      setEvents(mapped);
    } catch (e: any) {
      setLoadError(e?.message || 'Unable to load appointments');
      setEvents([]);
    } finally {
      setIsLoading(false);
    }
  };

  React.useEffect(() => {
    fetchAppointments(view, date);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [view, date]);

  React.useEffect(() => {
    setPendingStatus(selected?.status ?? null);
    setStatusError(null);
    setDeleteError(null);
  }, [selected]);

  // Create appointment modal state
  const [createOpen, setCreateOpen] = useState<boolean>(false);
  const [createStart, setCreateStart] = useState<Date | null>(null);
  const [createEnd, setCreateEnd] = useState<Date | null>(null);
  const [createNotes, setCreateNotes] = useState<string>('');
  const [customerQuery, setCustomerQuery] = useState<string>('');
  const [customerResults, setCustomerResults] = useState<Array<{ id: string; name: string; email: string }>>([]);
  const [selectedCustomer, setSelectedCustomer] = useState<{ id: string; name: string; email: string } | null>(null);
  const [isSearching, setIsSearching] = useState<boolean>(false);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  
  // New customer creation state
  const [showCreateCustomer, setShowCreateCustomer] = useState<boolean>(false);
  const [newCustomerName, setNewCustomerName] = useState<string>('');
  const [newCustomerEmail, setNewCustomerEmail] = useState<string>('');
  const [isCreatingCustomer, setIsCreatingCustomer] = useState<boolean>(false);
  const [customerCreationError, setCustomerCreationError] = useState<string | null>(null);

  // Autocomplete search (debounced)
  React.useEffect(() => {
    const term = customerQuery.trim();
    if (term.length < 2) {
      setCustomerResults([]);
      return;
    }
    const t = setTimeout(async () => {
      try {
        setIsSearching(true);
        const res = await fetch(`${API_BASE_URL}/api/customers?search=${encodeURIComponent(term)}`);
        if (!res.ok) throw new Error('Search failed');
        const list = await res.json();
        setCustomerResults(Array.isArray(list) ? list : []);
      } catch (e) {
        setCustomerResults([]);
      } finally {
        setIsSearching(false);
      }
    }, 300);
    return () => clearTimeout(t);
  }, [customerQuery]);

  // Create new customer function
  const createNewCustomer = async () => {
    if (!newCustomerName.trim() || !newCustomerEmail.trim()) {
      setCustomerCreationError('Name and email are required');
      return;
    }
    
    setIsCreatingCustomer(true);
    setCustomerCreationError(null);
    
    try {
      const res = await fetch(`${API_BASE_URL}/api/customers`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          name: newCustomerName.trim(),
          email: newCustomerEmail.trim(),
        }),
      });
      
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        throw new Error(data?.error || 'Failed to create customer');
      }
      
      // Create customer object from response
      const newCustomer = {
        id: data.id || crypto.randomUUID(),
        name: newCustomerName.trim(),
        email: newCustomerEmail.trim(),
      };
      
      // Set as selected customer and close creation form
      setSelectedCustomer(newCustomer);
      setShowCreateCustomer(false);
      setNewCustomerName('');
      setNewCustomerEmail('');
      setCustomerQuery(newCustomer.name);
      setCustomerResults([newCustomer]); // Add the new customer to results so the "No customers found" section disappears
    } catch (e: any) {
      setCustomerCreationError(e?.message || 'Failed to create customer');
    } finally {
      setIsCreatingCustomer(false);
    }
  };

  const eventPropGetter = useMemo(() => {
    return (event: AppointmentEvent) => {
      const baseStyle: React.CSSProperties = {
        borderRadius: '0.375rem',
        borderWidth: 1,
        borderStyle: 'solid',
        fontWeight: 500,
      };

      switch (event.status) {
        case 'Scheduled':
          return {
            style: {
              ...baseStyle,
              backgroundColor: 'var(--accent-color)',
              color: 'var(--primary-color)',
              borderColor: 'var(--primary-color)',
            },
          };
        case 'InProgress':
          return {
            style: {
              ...baseStyle,
              backgroundColor: 'var(--primary-color)',
              color: '#ffffff',
              borderColor: 'var(--primary-color)',
            },
          };
        case 'Completed':
          return {
            style: {
              ...baseStyle,
              backgroundColor: '#dcfce7',
              color: 'var(--success-color)',
              borderColor: 'var(--success-color)',
            },
          };
        case 'Canceled':
          return {
            style: {
              ...baseStyle,
              backgroundColor: '#fee2e2',
              color: 'var(--danger-color)',
              borderColor: 'var(--danger-color)',
            },
          };
        case 'NoShow':
          return {
            style: {
              ...baseStyle,
              backgroundColor: '#e5e7eb',
              color: '#374151',
              borderColor: '#6b7280',
            },
          };
        default:
          return { style: baseStyle };
      }
    };
  }, []);

  const EventContent = ({ event }: { event: AppointmentEvent }) => {
    return (
      <div className="leading-tight">
        <div className="text-[11px] opacity-80">
          {format(event.start, 'p')} – {format(event.end, 'p')}
        </div>
        <div className="text-xs font-medium truncate">{event.title}</div>
      </div>
    );
  };

  return (
    <div className="calendar-page flex-1 flex flex-col min-h-0 min-w-0 overflow-hidden bg-gray-50 p-6">
      <h1 className="mb-6 text-3xl font-bold">Calendar</h1>
      <div className="rounded-lg border border-gray-200 bg-white flex flex-col min-h-0 min-w-0 w-full flex-1">
        <div className="flex-1 min-h-0 min-w-0 p-4">
          <div className="h-full w-full overflow-auto relative">
            <Calendar<AppointmentEvent>
              localizer={localizer}
              events={events}
              view={view}
              onView={(v: View) => setView(v)}
              date={date}
              onNavigate={(d: Date) => setDate(d)}
              startAccessor="start"
              endAccessor="end"
              defaultView={Views.MONTH}
              views={[Views.MONTH, Views.WEEK, Views.DAY, Views.AGENDA]}
              popup
              eventPropGetter={eventPropGetter}
              onSelectEvent={(event: AppointmentEvent) => setSelected(event)}
              selectable
              onSelectSlot={(slot: any) => {
                setDate(slot.start);
                setView(Views.DAY);
                
                // Only open the creation modal if we're not in month view
                if (view !== Views.MONTH) {
                  setCreateStart(slot.start);
                  setCreateEnd(slot.end ?? addMinutes(slot.start, 30));
                  setSelectedCustomer(null);
                  setCustomerQuery('');
                  setCustomerResults([]);
                  setSubmitError(null);
                  setShowCreateCustomer(false);
                  setNewCustomerName('');
                  setNewCustomerEmail('');
                  setCustomerCreationError(null);
                  setCreateNotes('');
                  setCreateOpen(true);
                }
              }}
              step={30}
              timeslots={2}
              components={{
                event: EventContent as any,
              }}
            />
            {isLoading && (
              <div className="absolute inset-0 flex items-center justify-center bg-white/80 backdrop-blur-sm z-10">
                <div className="flex items-center space-x-2 text-sm text-gray-600">
                  <div className="animate-spin rounded-full h-4 w-4 border-2 border-gray-300 border-t-[var(--primary-color)]"></div>
                  <span>Loading appointments…</span>
                </div>
              </div>
            )}
            {loadError && !isLoading && (
              <div className="mt-3 text-sm text-[var(--danger-color)]">{loadError}</div>
            )}
          </div>
        </div>
      </div>

      {selected && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center"
          aria-modal="true"
          role="dialog"
        >
          <div
            className="absolute inset-0 bg-black/30"
            onClick={() => setSelected(null)}
          />
          <div className="relative z-10 w-full max-w-md rounded-lg border border-gray-200 bg-white shadow-xl">
            <div className="flex items-start justify-between border-b border-gray-100 p-4">
              <div>
                <h2 className="text-lg font-semibold text-gray-900">Appointment Details</h2>
                <p className="mt-1 text-sm text-gray-500">Click outside to close</p>
              </div>
              <button
                className="ml-4 rounded-md p-2 text-gray-500 hover:bg-gray-100 hover:text-gray-700"
                onClick={() => setSelected(null)}
                aria-label="Close"
              >
                ✕
              </button>
            </div>
            <div className="space-y-3 p-4">
              <div className="flex items-center justify-between">
                <h3 className="font-medium text-gray-900">{selected.title}</h3>
                <span
                  className={(() => {
                    switch (selected.status) {
                      case 'Scheduled':
                        return 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-[var(--primary-color)] border-[var(--primary-color)] bg-[var(--accent-color)]';
                      case 'InProgress':
                        return 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-white border-[var(--primary-color)] bg-[var(--primary-color)]';
                      case 'Completed':
                        return 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-[var(--success-color)] border-[var(--success-color)] bg-emerald-100';
                      case 'Canceled':
                        return 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-[var(--danger-color)] border-[var(--danger-color)] bg-rose-100';
                      case 'NoShow':
                        return 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-gray-700 border-gray-400 bg-gray-200';
                      default:
                        return 'inline-flex items-center rounded-md border px-2 py-1 text-xs font-medium text-gray-700 border-gray-300 bg-gray-100';
                    }
                  })()}
                >
                  {selected.status}
                </span>
              </div>

              <div className="text-sm text-gray-700">
                <div className="flex items-center justify-between py-1">
                  <span className="text-gray-500">Start</span>
                  <span className="font-medium">{format(selected.start, 'PPpp')}</span>
                </div>
                <div className="flex items-center justify-between py-1">
                  <span className="text-gray-500">End</span>
                  <span className="font-medium">{format(selected.end, 'PPpp')}</span>
                </div>
                {selected.notes && (
                  <div className="pt-2">
                    <span className="text-gray-500 text-sm">Notes</span>
                    <div className="mt-1 text-sm text-gray-700 bg-gray-50 p-2 rounded border">
                      {selected.notes}
                    </div>
                  </div>
                )}
              </div>

              <div className="pt-3 space-y-2">
                <div>
                  <label className="block text-xs text-gray-500">Update status</label>
                  <select
                    className="mt-1 w-full rounded-md border border-gray-300 px-2 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                    value={pendingStatus ?? selected.status}
                    onChange={(e) => setPendingStatus(e.target.value as AppointmentStatus)}
                    disabled={statusSaving}
                  >
                    {statusOptions.map(s => (
                      <option key={s} value={s}>{s}</option>
                    ))}
                  </select>
                  {statusError && (
                    <div className="mt-2 text-xs text-[var(--danger-color)]">{statusError}</div>
                  )}
                  {deleteError && (
                    <div className="mt-2 text-xs text-[var(--danger-color)]">{deleteError}</div>
                  )}
                </div>
                <button
                  className="inline-flex w-full items-center justify-center rounded-md bg-[var(--primary-color)] px-3 py-2 text-sm font-semibold text-white shadow hover:opacity-95 disabled:opacity-50 disabled:cursor-not-allowed"
                  disabled={statusSaving || !pendingStatus || pendingStatus === selected.status}
                  onClick={async () => {
                    if (!selected || !pendingStatus || pendingStatus === selected.status) return;
                    setStatusSaving(true);
                    setStatusError(null);
                    try {
                      const r = await fetch(`${API_BASE_URL}/api/appointments/${selected.id}/status`, {
                        method: 'PATCH',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({ appointmentId: selected.id, status: pendingStatus }),
                      });
                      const data = await r.json().catch(() => ({} as any));
                      if (!r.ok) {
                        const msg = (data && (data.error || data.title)) || 'Failed to update status';
                        throw new Error(msg);
                      }
                      const newStatus = (data?.status as AppointmentStatus) || pendingStatus;
                      setEvents((prev) => prev.map(ev => ev.id === selected.id ? { ...ev, status: newStatus } : ev));
                      setSelected(null);
                    } catch (err: any) {
                      setStatusError(err?.message || 'Unable to update status');
                    } finally {
                      setStatusSaving(false);
                    }
                  }}
                >
                  {statusSaving ? 'Saving…' : 'Save changes'}
                </button>
                <button
                  className="inline-flex w-full items-center justify-center rounded-md border border-[var(--danger-color)] bg-rose-50 px-3 py-2 text-sm font-semibold text-[var(--danger-color)] hover:bg-rose-100 disabled:opacity-50 disabled:cursor-not-allowed"
                  disabled={isDeleting || selected.status !== 'Scheduled'}
                  onClick={async () => {
                    if (!selected) return;
                    const confirmed = window.confirm('Delete this appointment? This action cannot be undone.');
                    if (!confirmed) return;
                    setIsDeleting(true);
                    setDeleteError(null);
                    try {
                      const r = await fetch(`${API_BASE_URL}/api/appointments/${selected.id}`, { method: 'DELETE' });
                      const data = await r.json().catch(() => ({} as any));
                      if (!r.ok) {
                        const msg = (data && (data.error || data.title)) || 'Failed to delete appointment';
                        throw new Error(msg);
                      }
                      setEvents((prev) => prev.filter(ev => ev.id !== selected.id));
                      setSelected(null);
                    } catch (err: any) {
                      setDeleteError(err?.message || 'Unable to delete appointment');
                    } finally {
                      setIsDeleting(false);
                    }
                  }}
                >
                  {isDeleting ? 'Deleting…' : 'Delete appointment'}
                </button>
                <button
                  className="inline-flex w-full items-center justify-center rounded-md border border-gray-300 bg-white px-3 py-2 text-sm font-semibold text-gray-700 hover:bg-gray-50"
                  onClick={() => setSelected(null)}
                >
                  Close
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      {createOpen && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center"
          aria-modal="true"
          role="dialog"
        >
          <div
            className="absolute inset-0 bg-black/30"
            onClick={() => {
              setCreateOpen(false);
              setShowCreateCustomer(false);
              setNewCustomerName('');
              setNewCustomerEmail('');
              setCustomerCreationError(null);
            }}
          />
          <div className="relative z-10 w-full max-w-lg rounded-lg border border-gray-200 bg-white shadow-xl">
            <div className="flex items-start justify-between border-b border-gray-100 p-4">
              <div>
                <h2 className="text-lg font-semibold text-gray-900">Book Appointment</h2>
                <p className="mt-1 text-sm text-gray-500">Select a customer and confirm</p>
              </div>
              <button
                className="ml-4 rounded-md p-2 text-gray-500 hover:bg-gray-100 hover:text-gray-700"
                onClick={() => {
                  setCreateOpen(false);
                  setShowCreateCustomer(false);
                  setNewCustomerName('');
                  setNewCustomerEmail('');
                  setCustomerCreationError(null);
                }}
                aria-label="Close"
              >
                ✕
              </button>
            </div>
            <div className="space-y-4 p-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Start</label>
                <div className="mt-1 text-sm font-medium text-gray-900">
                  {createStart ? format(createStart, 'PPpp') : '-'}
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">End</label>
                <div className="mt-1 text-sm font-medium text-gray-900">
                  {createEnd ? format(createEnd, 'PPpp') : '-'}
                </div>
              </div>

              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-xs text-gray-500">Adjust start</label>
                  <input
                    type="datetime-local"
                    className="mt-1 w-full rounded-md border border-gray-300 px-2 py-1 text-sm"
                    value={createStart ? new Date(createStart).toISOString().slice(0,16) : ''}
                    onChange={(e) => setCreateStart(new Date(e.target.value))}
                  />
                </div>
                <div>
                  <label className="block text-xs text-gray-500">Adjust end</label>
                  <input
                    type="datetime-local"
                    className="mt-1 w-full rounded-md border border-gray-300 px-2 py-1 text-sm"
                    value={createEnd ? new Date(createEnd).toISOString().slice(0,16) : ''}
                    onChange={(e) => setCreateEnd(new Date(e.target.value))}
                  />
                </div>
              </div>

              <div className="text-xs text-gray-500">Minimum 30 minutes, maximum 12 hours.</div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Customer</label>
                <input
                  type="text"
                  className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                  placeholder="Search by name or email"
                  value={customerQuery}
                  onChange={(e) => {
                    setCustomerQuery(e.target.value);
                    setSelectedCustomer(null);
                  }}
                />
                <div className="mt-2 max-h-44 overflow-auto rounded-md border border-gray-200">
                  {isSearching && (
                    <div className="p-2 text-sm text-gray-500">Searching…</div>
                  )}
                  {!isSearching && customerQuery.trim().length >= 2 && customerResults.length === 0 && !showCreateCustomer && (
                    <div className="p-2">
                      <div className="text-sm text-gray-500 mb-2">No customers found</div>
                      <button
                        type="button"
                        className="w-full px-3 py-2 text-left text-sm text-[var(--primary-color)] hover:bg-[var(--accent-color)] border border-[var(--primary-color)] rounded-md"
                        onClick={() => {
                          setShowCreateCustomer(true);
                          setNewCustomerName(customerQuery);
                          setCustomerCreationError(null);
                        }}
                      >
                        + Create new customer: "{customerQuery}"
                      </button>
                    </div>
                  )}
                  {!isSearching && customerResults.map((c) => (
                    <button
                      key={c.id}
                      type="button"
                      className={`w-full px-3 py-2 text-left text-sm hover:bg-gray-50 ${selectedCustomer?.id === c.id ? 'bg-[var(--accent-color)]' : ''}`}
                      onClick={() => setSelectedCustomer(c)}
                    >
                      <div className="font-medium text-gray-900">{c.name}</div>
                      <div className="text-xs text-gray-500">{c.email}</div>
                    </button>
                  ))}
                </div>
                {selectedCustomer && (
                  <div className="mt-2 inline-flex items-center rounded-md border border-[var(--primary-color)] bg-[var(--accent-color)] px-2 py-1 text-xs font-medium text-[var(--primary-color)]">
                    Selected: {selectedCustomer.name}
                  </div>
                )}

                {showCreateCustomer && (
                  <div className="mt-3 p-3 border border-gray-200 rounded-md bg-gray-50">
                    <div className="text-sm font-medium text-gray-900 mb-3">Create New Customer</div>
                    
                    <div className="space-y-3">
                      <div>
                        <label className="block text-xs font-medium text-gray-700 mb-1">Name</label>
                        <input
                          type="text"
                          className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                          value={newCustomerName}
                          onChange={(e) => setNewCustomerName(e.target.value)}
                          placeholder="Enter customer name"
                          disabled={isCreatingCustomer}
                        />
                      </div>
                      
                      <div>
                        <label className="block text-xs font-medium text-gray-700 mb-1">Email</label>
                        <input
                          type="email"
                          className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                          value={newCustomerEmail}
                          onChange={(e) => setNewCustomerEmail(e.target.value)}
                          placeholder="Enter customer email"
                          disabled={isCreatingCustomer}
                        />
                      </div>
                      
                      {customerCreationError && (
                        <div className="text-sm text-[var(--danger-color)]">{customerCreationError}</div>
                      )}
                      
                      <div className="flex items-center gap-2">
                        <button
                          type="button"
                          className="inline-flex items-center rounded-md bg-[var(--primary-color)] px-3 py-2 text-sm font-semibold text-white hover:opacity-95 disabled:opacity-50"
                          disabled={!newCustomerName.trim() || !newCustomerEmail.trim() || isCreatingCustomer}
                          onClick={createNewCustomer}
                        >
                          {isCreatingCustomer ? 'Creating...' : 'Create Customer'}
                        </button>
                        
                        <button
                          type="button"
                          className="inline-flex items-center rounded-md border border-gray-300 bg-white px-3 py-2 text-sm font-semibold text-gray-700 hover:bg-gray-50"
                          onClick={() => {
                            setShowCreateCustomer(false);
                            setNewCustomerName('');
                            setNewCustomerEmail('');
                            setCustomerCreationError(null);
                          }}
                          disabled={isCreatingCustomer}
                        >
                          Cancel
                        </button>
                      </div>
                    </div>
                  </div>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700">Notes (Optional)</label>
                <textarea
                  className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-[var(--primary-color)]"
                  placeholder="Add any notes or description for this appointment..."
                  rows={3}
                  value={createNotes}
                  onChange={(e) => setCreateNotes(e.target.value)}
                />
              </div>

              {submitError && (
                <div className="text-sm text-[var(--danger-color)]">{submitError}</div>
              )}

              <div className="pt-2 flex items-center justify-end gap-2">
                <button
                  className="inline-flex items-center rounded-md px-3 py-2 text-sm font-semibold text-gray-700 hover:bg-gray-100"
                  onClick={() => {
                    setCreateOpen(false);
                    setShowCreateCustomer(false);
                    setNewCustomerName('');
                    setNewCustomerEmail('');
                    setCustomerCreationError(null);
                    setCreateNotes('');
                  }}
                  disabled={isSubmitting}
                >
                  Cancel
                </button>
                <button
                  className="inline-flex items-center rounded-md bg-[var(--primary-color)] px-3 py-2 text-sm font-semibold text-white hover:opacity-95 disabled:opacity-50"
                  disabled={!selectedCustomer || !createStart || !createEnd || isSubmitting}
                  onClick={async () => {
                    if (!selectedCustomer || !createStart || !createEnd) return;
                    setIsSubmitting(true);
                    setSubmitError(null);
                    try {
                      const res = await fetch(`${API_BASE_URL}/api/appointments`, {
                        method: 'POST',
                        headers: { 'Content-Type': 'application/json' },
                        body: JSON.stringify({
                          customerId: selectedCustomer.id,
                          startUtc: createStart.toISOString(),
                          endUtc: createEnd.toISOString(),
                          notes: createNotes.trim() || null,
                        }),
                      });
                      const data = await res.json().catch(() => ({}));
                      if (!res.ok) throw new Error((data && data.error) || 'Failed to book appointment');
                      const newId = data?.id ?? crypto.randomUUID();
                      setEvents((prev) => [
                        ...prev,
                        {
                          id: String(newId),
                          title: `Appointment – ${selectedCustomer.name}`,
                          start: createStart,
                          end: createEnd,
                          status: 'Scheduled',
                          notes: createNotes.trim() || undefined,
                        },
                      ]);
                      setCreateOpen(false);
                      setSelectedCustomer(null);
                      setCustomerQuery('');
                      setCustomerResults([]);
                      setShowCreateCustomer(false);
                      setNewCustomerName('');
                      setNewCustomerEmail('');
                      setCustomerCreationError(null);
                      setCreateNotes('');
                    } catch (e: any) {
                      setSubmitError(e?.message || 'Something went wrong');
                    } finally {
                      setIsSubmitting(false);
                    }
                  }}
                >
                  {isSubmitting ? 'Booking…' : 'Book Appointment'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default CalendarPage;
