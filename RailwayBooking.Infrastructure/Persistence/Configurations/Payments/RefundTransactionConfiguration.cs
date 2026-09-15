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
    public class RefundTransactionConfiguration
        : IEntityTypeConfiguration<RefundTransaction>
    {
        public void Configure(
            EntityTypeBuilder<RefundTransaction> builder)
        {
            builder.ToTable("refund_transactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(12, 2);

            builder.Property(x => x.Reason)
                .HasMaxLength(500);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasIndex(x => x.PaymentTransactionId);
        }
    }
}
