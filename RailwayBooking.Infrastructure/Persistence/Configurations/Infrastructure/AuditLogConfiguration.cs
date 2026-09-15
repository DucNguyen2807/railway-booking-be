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
    public class AuditLogConfiguration
        : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(
            EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_logs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EntityType)
                .HasMaxLength(100);

            builder.Property(x => x.Action)
                .HasMaxLength(100);

            builder.Property(x => x.OldValue)
                .HasColumnType("jsonb");

            builder.Property(x => x.NewValue)
                .HasColumnType("jsonb");

            builder.HasIndex(x =>
                new { x.EntityType, x.EntityId });

            builder.HasIndex(x => x.RequestId);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
