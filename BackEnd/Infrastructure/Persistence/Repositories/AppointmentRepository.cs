using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Common;

namespace Infrastructure.Persistence.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Appointments.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<bool> HasOverlappingAppointmentsAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
        {
            return await _context.Appointments
                .AnyAsync(a => a.Status != AppointmentStatus.Canceled && (a.TimeSlot.StartUtc < endTime && a.TimeSlot.EndUtc > startTime), cancellationToken);
        }

        public void Add(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
        }

        public void Remove(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
        }

        public async Task<List<Appointment>> GetInRangeAsync(DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default)
        {
            return await _context.Appointments
                .Where(a => a.TimeSlot.StartUtc < endUtc && a.TimeSlot.EndUtc > startUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Appointments.CountAsync(cancellationToken);
        }

        public async Task<int> CountTodayAsync(DateTime todayUtc, CancellationToken cancellationToken = default)
        {
            var startOfDay = todayUtc.Date;
            var endOfDay = startOfDay.AddDays(1);
            
            return await _context.Appointments
                .CountAsync(a => a.TimeSlot.StartUtc >= startOfDay && a.TimeSlot.StartUtc < endOfDay, cancellationToken);
        }

        public async Task<List<Appointment>> GetUpcomingTodayAsync(DateTime nowUtc, int limit, CancellationToken cancellationToken = default)
        {
            var startOfDay = nowUtc.Date;
            var endOfDay = startOfDay.AddDays(1);
            
            return await _context.Appointments
                .Where(a => a.TimeSlot.StartUtc >= nowUtc && a.TimeSlot.StartUtc >= startOfDay && a.TimeSlot.StartUtc < endOfDay)
                .OrderBy(a => a.TimeSlot.StartUtc)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
    }
}


