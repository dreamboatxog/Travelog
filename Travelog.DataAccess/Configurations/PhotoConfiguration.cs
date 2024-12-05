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
    public class PhotoConfiguration:IEntityTypeConfiguration<PhotoEntity>
    {
        public void Configure(EntityTypeBuilder<PhotoEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FilePath).HasMaxLength(1024).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(512).IsRequired();

            builder.HasOne(x => x.Place)
                   .WithMany(p => p.Photos)
                   .HasForeignKey(x => x.PlaceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
