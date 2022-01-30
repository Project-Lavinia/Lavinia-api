namespace Lavinia.Api.Models
{
    /// <summary>
    /// Contains the metrics for a district, valid for a particular election.
    /// </summary>
    public class DistrictMetrics
    {
        /// <summary>
        /// Name of the district these data are valid for
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// The election year these data are valid for
        /// </summary>
        public int ElectionYear { get; set; }

        /// <summary>
        /// The geographical size of the district
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// The population size of the district
        /// </summary>
        public int Population { get; set; }

        /// <summary>
        /// The number of district seats predetermined for the district
        /// </summary>
        public int Seats { get; set; }
    }
}