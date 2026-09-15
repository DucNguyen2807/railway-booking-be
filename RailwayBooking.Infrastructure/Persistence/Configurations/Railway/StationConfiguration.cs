using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Railway
{
    public class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.ToTable("stations");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Address)
                .HasMaxLength(250);

            builder.Property(x => x.Latitude)
                .HasColumnType("numeric(10,6)");

            builder.Property(x => x.Longitude)
                .HasColumnType("numeric(10,6)");

            builder.Property(x => x.Status)
                .IsRequired();

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.HasIndex(x => x.Status);
        }
    }
}
