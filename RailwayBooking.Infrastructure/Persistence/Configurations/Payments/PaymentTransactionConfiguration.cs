using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Infrastructure.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Payments
{
    public class PaymentTransactionConfiguration
        : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(
            EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.ToTable("payment_transactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PaymentProvider)
                .HasMaxLength(50);

            builder.Property(x => x.ProviderTransactionId)
                .HasMaxLength(100);

            builder.Property(x => x.IdempotencyKey)
                .HasMaxLength(200);

            builder.Property(x => x.Amount)
                .HasPrecision(12, 2);

            builder.Property(x => x.Currency)
                .HasMaxLength(10);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            //
            // JSONB support
            //
            builder.Property(x => x.RawResponse)
                .HasColumnType("jsonb");

            builder.HasIndex(x => x.OrderId);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.ProviderTransactionId);

            builder.HasIndex(x => x.IdempotencyKey)
                .IsUnique();
        }
    }
}
