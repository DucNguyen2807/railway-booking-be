using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Infrastructure.Entities.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Inventory
{
    public class SeatHoldConfiguration
        : IEntityTypeConfiguration<SeatHold>
    {
        public void Configure(EntityTypeBuilder<SeatHold> builder)
        {
            builder.ToTable("seat_holds");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasIndex(x => x.HoldToken)
                .IsUnique();

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.ExpiredAt);

            builder.HasIndex(x => x.Status);
        }
    }
}
