using LaviniaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LaviniaApi.Data
{
    public class ElectionContext : DbContext
    {
        public ElectionContext()
        {
        }

        public ElectionContext(DbContextOptions<ElectionContext> options) : base(options)
        {
        }

        // API v1
        public DbSet<Country> Countries { get; set; }

        // API v2
        public DbSet<PartyVotes> PartyVotes { get; set; }
        public DbSet<DistrictMetrics> DistrictMetrics { get; set; }
        public DbSet<ElectionParameters> ElectionParameters { get; set; }
        public DbSet<AlgorithmParameters> AlgorithmParameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // API v1
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
            modelBuilder.Entity<Party>()
                .HasAlternateKey(p => p.PartyId);
            modelBuilder.Entity<Result>()
                .HasAlternateKey(r => r.ResultId);
            modelBuilder.Entity<CountyData>()
                .HasAlternateKey(cD => cD.CountyDataId);

            // API v2
        }
    }
}