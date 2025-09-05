using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);
            
            // Configure Email property
            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);
                
            // Configure Name property
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}


