using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RailwayBooking.Infrastructure.Entities.Trips;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Trips
{
    public class TrainTripConfiguration
    : IEntityTypeConfiguration<TrainTrip>
    {
        public void Configure(EntityTypeBuilder<TrainTrip> builder)
        {
            builder.ToTable("train_trips");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TripCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.DepartureStation)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.ArrivalStation)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(x => x.TripCode)
                .IsUnique();

            builder.HasIndex(x => x.DepartureTime);

            builder.HasIndex(x => x.Status);

            builder.HasOne(x => x.Train)
                .WithMany()
                .HasForeignKey(x => x.TrainId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

