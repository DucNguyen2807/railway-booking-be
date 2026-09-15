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
    public class HoldItemConfiguration
        : IEntityTypeConfiguration<HoldItem>
    {
        public void Configure(EntityTypeBuilder<HoldItem> builder)
        {
            builder.ToTable("hold_items");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Hold)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.HoldId);

            builder.HasOne(x => x.Inventory)
                .WithMany()
                .HasForeignKey(x => x.InventoryId);

            builder.HasIndex(x =>
                new { x.HoldId, x.InventoryId })
                .IsUnique();
        }
    }
}
