using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(x => x.Id);
            
            // Configure TimeSlot as owned type
            builder.OwnsOne(x => x.TimeSlot, timeSlot =>
            {
                timeSlot.Property(t => t.StartUtc).HasColumnName("StartUtc").IsRequired();
                timeSlot.Property(t => t.EndUtc).HasColumnName("EndUtc").IsRequired();
                
                // Add index on the owned type properties
                timeSlot.HasIndex(t => new { t.StartUtc, t.EndUtc }).IsUnique(false);
            });

            // Configure Status property as enum
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .IsRequired();
        }
    }
}


