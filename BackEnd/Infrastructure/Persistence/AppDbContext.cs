using Domain.Entities;
using Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchEventsAsync(cancellationToken);
            return result;
        }

        public async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries<AggregateRoot>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = entities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            entities.ForEach(e => e.ClearDomainEvents());

            // Publish domain events directly via MediatR
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}

