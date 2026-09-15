using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Infrastructure.Entities.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Infrastructure
{
    public class ProcessedEventConfiguration
        : IEntityTypeConfiguration<ProcessedEvent>
    {
        public void Configure(
            EntityTypeBuilder<ProcessedEvent> builder)
        {
            builder.ToTable("processed_events");

            builder.HasKey(x => x.EventId);

            builder.Property(x => x.ConsumerName)
                .HasMaxLength(100);

            builder.HasIndex(x => x.ConsumerName);
        }
    }
}
