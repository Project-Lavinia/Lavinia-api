using LaviniaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LaviniaApi.Data
{
    // API v2
    public class NOContext : DbContext
    {
        public NOContext()
        {
        }

        public NOContext(DbContextOptions<NOContext> options) : base(options)
        {
        }
        public DbSet<Party> Parties { get; set; }
        public DbSet<PartyVotes> PartyVotes { get; set; }
        public DbSet<DistrictMetrics> DistrictMetrics { get; set; }
        public DbSet<ElectionParameters> ElectionParameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Party>()
                .HasAlternateKey(p => new {p.Code});
            modelBuilder.Entity<PartyVotes>()
                .HasKey(p => new {p.ElectionYear, p.District, p.Party, p.ElectionType});
            modelBuilder.Entity<DistrictMetrics>()
                .HasKey(d => new {d.ElectionYear, d.District});
            modelBuilder.Entity<ElectionParameters>()
                .HasKey(e => new {e.ElectionYear, e.ElectionType});
        }
    }
}