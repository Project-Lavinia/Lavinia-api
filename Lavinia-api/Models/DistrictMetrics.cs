using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaviniaApi.Models
{
    // API v2
    // Contains the metrics for a district, valid for a particular election.
    public class DistrictMetrics
    {
        // Name of the district these data are valid for
        public string District { get; set; }

        // The election year these data are valid for
        public int ElectionYear { get; set; }

        // The geographical size of the district
        public double Area { get; set; }

        // The population size of the district
        public int Population { get; set; }

        // The number of district seats predetermined for the district
        public int Seats { get; set; }
    }
}