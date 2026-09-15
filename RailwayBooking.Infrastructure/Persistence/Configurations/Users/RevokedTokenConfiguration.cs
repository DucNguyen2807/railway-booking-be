using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Users
{
    public class RevokedTokenConfiguration
        : IEntityTypeConfiguration<RevokedToken>
    {
        public void Configure(
            EntityTypeBuilder<RevokedToken> builder)
        {
            builder.ToTable("revoked_tokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Token)
                .IsRequired();

            builder.HasIndex(x => x.Token)
                .IsUnique();

            builder.Property(x => x.TokenType)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(x => x.RevokedTokens)
                .HasForeignKey(x => x.UserId);
        }
    }
}
