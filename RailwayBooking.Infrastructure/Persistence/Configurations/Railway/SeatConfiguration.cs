using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Railway
{

    public class SeatConfiguration
        : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("seats");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SeatNumber)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.SeatType)
                .HasMaxLength(30);

            builder.HasOne(x => x.Coach)
                .WithMany(x => x.Seats)
                .HasForeignKey(x => x.CoachId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x =>
                new { x.CoachId, x.SeatNumber })
                .IsUnique();
        }
    }
}