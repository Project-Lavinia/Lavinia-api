using System;
using System.Collections.Generic;
using System.Linq;
using LaviniaApi.Models;

namespace LaviniaApi.Utilities
{
    public static class ModelBuilder
    {
        /// <summary>
        /// Takes a list of CountyDataFormat and builds a list of DistrictMetrics
        /// </summary>
        /// <param name="countyData">A list of CountyDataFormat</param>
        /// <returns>A list of DistrictMetrics</returns>
        public static IEnumerable<DistrictMetrics> BuildDistrictMetrics(IEnumerable<CountyDataFormat> countyData)
        {
            return countyData.Select(data => new DistrictMetrics
            { Area = data.Area, District = data.County, ElectionYear = data.Year, Population = data.Population, Seats = data.Seats });
        }

        /// <summary>
        /// Takes a list of ElectionFormat, an election type and a list of DistrictMetrics, and returns a list of ElectionParameters.
        /// The returned list contains information about which parameters were used for each election.
        /// </summary>
        /// <param name="electionData">List of ElectionFormat</param>
        /// <param name="electionType">Election type code</param>
        /// <returns></returns>
        public static IEnumerable<ElectionParameters> BuildElectionParameters(IEnumerable<ElectionFormat> electionData,
        string electionType)
        {
            return electionData.Select(data => new ElectionParameters
            {
                Algorithm = BuildAlgorithmParameters(data),
                AreaFactor = data.AreaFactor,
                DistrictSeats = data.Seats,
                ElectionType = electionType,
                ElectionYear = data.Year,
                LevelingSeats = data.LevelingSeats,
                Threshold = data.Threshold,
                TotalVotes = 0
            });
        }

        /// <summary>
        /// Takes an ElectionFormat, extracts information about the algorithm used and returns an AlgorithmParameter
        /// </summary>
        /// <param name="data">ElectionFormat - Information about a particular election</param>
        /// <returns>AlgorithmParameters - Parameters used in the election</returns>
        public static AlgorithmParameters BuildAlgorithmParameters(ElectionFormat data)
        {
            switch (data.AlgorithmString)
            {
                case AlgorithmUtilities.Undefined:
                    return null;
                case AlgorithmUtilities.ModifiedSainteLagues:
                    return new AlgorithmParameters
                    {
                        Algorithm = data.AlgorithmString,
                        Parameters = new List<ListElement<double>>
                        {
                            new ListElement<double>
                            {
                                Key = "First Divisor",
                                Value = data.FirstDivisor
                            }
                        }
                    };
                case AlgorithmUtilities.SainteLagues:
                case AlgorithmUtilities.DHondt:
                    return new AlgorithmParameters
                    { Algorithm = data.AlgorithmString, Parameters = new List<ListElement<double>>() };
                default:
                    throw new ArgumentOutOfRangeException("Did not recognize the algorithm: " + data.AlgorithmString);
            }
        }

        /// <summary>
        /// Takes a list of ResultFormat, an election type and an election year, and returns a list of PartyVotes.
        /// The returned list contains information about how many votes each party got in each district for a particular election.
        /// </summary>
        /// <param name="election">List of ResultFormat</param>
        /// <param name="electionType">Type of election</param>
        /// <param name="electionYear">Which year the election was held</param>
        /// <returns>List of PartyVotes</returns>
        public static IEnumerable<PartyVotes> BuildPartyVotes(IEnumerable<ResultFormat> election, string electionType,
            int electionYear)
        {
            return election.Select(data => new PartyVotes
            {
                District = data.Fylkenavn,
                ElectionType = electionType,
                ElectionYear = electionYear,
                Party = data.Partikode,
                Votes = data.AntallStemmerTotalt
            });
        }
    }
}