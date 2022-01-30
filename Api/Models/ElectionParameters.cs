namespace Lavinia.Api.Models
{
    /// <summary>
    ///  Stores the parameters needed for distributing seats for a particular election.
    /// </summary>
    public class ElectionParameters
    {
        /// <summary>
        /// The election year these parameters are valid for.
        /// </summary>
        public int ElectionYear { get; init; } = default!;

        /// <summary>
        /// The election type of this election
        /// </summary>
        public string ElectionType { get; init; } = default!;

        /// <summary>
        /// Algorithm used during this election
        /// </summary>
        public AlgorithmParameters Algorithm { get; init; } = default!;

        /// <summary>
        /// The minimum percentage of votes a party needs to be considered for leveling seats
        /// </summary>
        public double Threshold { get; init; } = default!;

        /// <summary>
        /// A multiplier applied to the number of square kilometers when distributing district seats
        /// </summary>
        public double AreaFactor { get; init; } = default!;

        /// <summary>
        /// The number of district seats distributed in this election
        /// </summary>
        public int DistrictSeats { get; init; } = default!;

        /// <summary>
        /// The number of leveling seats to be distributed
        /// </summary>
        public int LevelingSeats { get; init; } = default!;

        /// <summary>
        /// The total number of valid votes cast for this election
        /// </summary>
        public int TotalVotes { get; init; } = default!;
    }
}