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
    public class IdempotencyRecordConfiguration
        : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(
            EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("idempotency_records");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.IdempotencyKey)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.ResponseBody)
                .HasColumnType("jsonb");

            builder.HasIndex(x => x.IdempotencyKey)
                .IsUnique();

            builder.HasIndex(x => x.ExpiredAt);
        }
    }
}
