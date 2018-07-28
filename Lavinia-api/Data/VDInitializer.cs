using System.Linq;
using LaviniaApi.Models;
using LaviniaApi.Repository;
using LaviniaApi.Utilities;

namespace LaviniaApi.Data
{
    public static class VDInitializer
    {
        public static void Initialize(VDContext context)
        {
            context.Database.EnsureCreated();
            if (!context.VDModels.Any())
            {
                VDModel[] entities = CsvUtilities.CsvToVdArray("Data/States/NO/ParliamentaryElection/2017.csv");
                context.VDModels.AddRange(entities);
                context.SaveChanges();
            }
        }
    }
}