using Domain.Entities;

namespace Domain.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> HasOverlappingAppointmentsAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
        void Add(Appointment appointment);
    }
}


