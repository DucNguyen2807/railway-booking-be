using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Infrastructure.Entities.Trips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Trips
{

    public class TripStationConfiguration
        : IEntityTypeConfiguration<TripStation>
    {
        public void Configure(EntityTypeBuilder<TripStation> builder)
        {
            builder.ToTable("trip_stations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.StationCode)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(x => x.Trip)
                .WithMany(x => x.Stations)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x =>
                new { x.TripId, x.StationOrder })
                .IsUnique();
        }
    }
}
