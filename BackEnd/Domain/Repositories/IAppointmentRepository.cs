using Domain.Entities;

namespace Domain.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> HasOverlappingAppointmentsAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
        Task<List<Appointment>> GetInRangeAsync(DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
        Task<int> CountAllAsync(CancellationToken cancellationToken = default);
        Task<int> CountTodayAsync(DateTime todayUtc, CancellationToken cancellationToken = default);
        Task<List<Appointment>> GetUpcomingTodayAsync(DateTime nowUtc, int limit, CancellationToken cancellationToken = default);
        void Add(Appointment appointment);
        void Remove(Appointment appointment);
    }
}


