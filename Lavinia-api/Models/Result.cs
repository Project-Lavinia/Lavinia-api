using System.ComponentModel.DataAnnotations.Schema;

namespace LaviniaApi.Models
{
    public class Result
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResultId { get; set; }

        public string PartyName { get; set; }
        public string PartyCode { get; set; }
        public int Votes { get; set; } = -1;
        public double Percentage { get; set; } = double.NaN;
    }
}