using Domain.Common;

namespace Application.Abstractions
{
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(CancellationToken cancellationToken = default);
    }
}


