using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LaviniaApi.Utilities;

namespace LaviniaApi.Models
{
    public class Election
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ElectionId { get; set; }

        [Required] public int Year { get; set; } = -1;

        [Required] public Algorithm Algorithm { get; set; }

        [Required] public double FirstDivisor { get; set; } = double.NaN;

        [Required] public double Threshold { get; set; } = double.NaN;

        [Required] public int Seats { get; set; } = -1;

        [Required] public int LevelingSeats { get; set; } = -1;

        [Required] public virtual List<County> Counties { get; set; }
    }
}