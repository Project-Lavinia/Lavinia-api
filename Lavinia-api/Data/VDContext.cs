using LaviniaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LaviniaApi.Repository
{
    public class VDContext : DbContext
    {
        public VDContext()
        {
        }

        public VDContext(DbContextOptions<VDContext> options) : base(options)
        {
        }

        public DbSet<VDModel> VDModels { get; set; }
    }
}