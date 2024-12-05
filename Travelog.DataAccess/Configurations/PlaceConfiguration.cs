using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travelog.DataAccess.Entities;

namespace Travelog.DataAccess.Configurations
{
    public class PlaceConfiguration : IEntityTypeConfiguration<PlaceEntity>
    {
        public void Configure(EntityTypeBuilder<PlaceEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1024).IsRequired();
            builder.Property(x => x.Latitude).IsRequired();
            builder.Property(x => x.Longitude).IsRequired();

            builder.HasOne(x => x.User)
                   .WithMany(u => u.Places)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Photos)
                   .WithOne(p => p.Place)
                   .HasForeignKey(p => p.PlaceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
