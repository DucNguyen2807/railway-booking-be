using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RailwayBooking.Infrastructure.Entities.Infrastructure;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Infrastructure
{
    public class OutboxEventConfiguration
        : IEntityTypeConfiguration<OutboxEvent>
    {
        public void Configure(
            EntityTypeBuilder<OutboxEvent> builder)
        {
            builder.ToTable("outbox_events");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventType)
                .HasMaxLength(200);

            builder.Property(x => x.AggregateType)
                .HasMaxLength(100);

            builder.Property(x => x.Payload)
                .HasColumnType("jsonb");

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasIndex(x => x.EventId)
                .IsUnique();

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.CreatedAt);

            builder.HasIndex(x =>
                new { x.AggregateType, x.AggregateId });
        }
    }
}
