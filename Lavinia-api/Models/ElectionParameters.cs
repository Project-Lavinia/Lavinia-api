using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Utilities;

namespace LaviniaApi.Models
{
    // API v2
    // Stores the parameters needed for distributing seats for a particular election.
    public class ElectionParameters
    {
        // The election year these parameters are valid for
        public int ElectionYear { get; set; }

        // The election type of this election
        public string ElectionType { get; set; }

        // Algorithm used during this election
        public AlgorithmParameters Algorithm { get; set; }

        // The minimum percentage of votes a party needs to be considered for leveling seats
        public double Threshold { get; set; }

        // A multiplier applied to the number of square kilometers when distributing district seats
        public double AreaFactor { get; set; }

        // If the district seats are predetermined, this list will contain a tuple with the district name and
        // the number of districts seats for each district in the election
        // This list will always contain an entry "SUM" for the number of district seats distributed in this election
        public List<ListElement<int>> DistrictSeats { get; set; }

        // The number of leveling seats to be distributed
        public int LevelingSeats { get; set; }

        // The total number of valid votes cast for this election
        public int TotalVotes { get; set; }
    }


    // API v3
    // Stores the parameters needed for distributing seats for a particular election.
    public class ElectionParametersV3
    {
        // The election year these parameters are valid for
        public int ElectionYear { get; set; }

        // The election type of this election
        public string ElectionType { get; set; }

        // Algorithm used during this election
        public AlgorithmParameters Algorithm { get; set; }

        // The minimum percentage of votes a party needs to be considered for leveling seats
        public double Threshold { get; set; }

        // A multiplier applied to the number of square kilometers when distributing district seats
        public double AreaFactor { get; set; }

        // The number of district seats distributed in this election
        public int DistrictSeats { get; set; }

        // The number of leveling seats to be distributed
        public int LevelingSeats { get; set; }

        // The total number of valid votes cast for this election
        public int TotalVotes { get; set; }
    }
}