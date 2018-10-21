using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaviniaApi.Models
{
    // API v2
    public class ElectionParameters
    {
        public int ElectionYear { get; set; }
        public string ElectionType { get; set; }
        public string Algorithm { get; set; }
        public int Threshold { get; set; }
        public int AreaFactor { get; set; }
        public int DistrictSeats { get; set; }
        public int LevelingSeats { get; set; }
    }
}