using LaviniaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LaviniaApi.Data
{
    // API v1
    public class ElectionContext : DbContext
    {
        public ElectionContext()
        {
        }

        public ElectionContext(DbContextOptions<ElectionContext> options) : base(options)
        {
        }
        
        public DbSet<Country> Countries { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Country>()
                .HasKey(c => c.CountryCode);
            modelBuilder.Entity<Country>()
                .HasAlternateKey(c => c.CountryId);
            modelBuilder.Entity<County>()
                .HasAlternateKey(c => c.CountyId);
            modelBuilder.Entity<Election>()
                .HasAlternateKey(e => e.ElectionId);
            modelBuilder.Entity<ElectionType>()
                .HasAlternateKey(eT => eT.ElectionTypeId);
            modelBuilder.Entity<Result>()
                .HasAlternateKey(r => r.ResultId);
            modelBuilder.Entity<CountyData>()
                .HasAlternateKey(cD => cD.CountyDataId);
        }
    }
}