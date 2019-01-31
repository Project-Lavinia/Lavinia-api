using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Utilities;

namespace LaviniaApi.Models
{
    // API v2
    public class ElectionParameters
    {
        public int ElectionYear { get; set; }
        public string ElectionType { get; set; }
        public AlgorithmParameters Algorithm { get; set; }
        public double Threshold { get; set; }
        public double AreaFactor { get; set; }
        public List<ListElement<int>> DistrictSeats { get; set; }
        public int LevelingSeats { get; set; }
        public int TotalVotes { get; set; }
    }
}