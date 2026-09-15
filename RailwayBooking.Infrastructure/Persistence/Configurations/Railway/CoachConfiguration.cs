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
    public class CoachConfiguration
            : IEntityTypeConfiguration<Coach>
    {
        public void Configure(EntityTypeBuilder<Coach> builder)
        {
            builder.ToTable("coaches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CoachNumber)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(x => x.ClassType)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.SeatCapacity)
                .IsRequired();

            builder.HasOne(x => x.Train)
                .WithMany(x => x.Coaches)
                .HasForeignKey(x => x.TrainId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x =>
                new { x.TrainId, x.CoachNumber })
                .IsUnique();
        }
    }
}
