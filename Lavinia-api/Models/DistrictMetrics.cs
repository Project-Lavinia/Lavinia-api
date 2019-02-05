using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaviniaApi.Models
{
    // API v2
    public class DistrictMetrics
    {
        public string District { get; set; }
        public int ElectionYear { get; set; }
        public double Area { get; set; }
        public int Population { get; set; }
        public int Seats { get; set; }
    }
}