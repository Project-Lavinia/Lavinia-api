using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    // API v1
    public class CountyData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CountyDataId { get; set; }

        public int Year { get; set; } = -1;
        public string CountyName { get; set; }
        public int Population { get; set; } = -1;
        public double Areal { get; set; } = double.NaN;
        public int Seats { get; set; } = -1;
    }
}