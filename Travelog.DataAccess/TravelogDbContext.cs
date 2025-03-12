using Microsoft.EntityFrameworkCore;
using Travelog.DataAccess.Configurations;
using Travelog.DataAccess.Entities;
using Travelog.DataAccess.Models;


namespace Travelog.DataAccess
{
    public class TravelogDbContext:DbContext
    {
        public TravelogDbContext(DbContextOptions<TravelogDbContext> options)
            :base(options)  
        {
            
        }

        public DbSet<UserEntity> Users  { get; set; }
       
        public DbSet<PlaceEntity> Places { get; set; }
        public DbSet<PhotoEntity> Photos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PlaceConfiguration());
            modelBuilder.ApplyConfiguration(new PhotoConfiguration());
        }
    }

    
}
