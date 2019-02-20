using System;
using System.Collections.Generic;
using System.Linq;
using LaviniaApi.Models;

namespace LaviniaApi.Utilities
{
    public static class ModelBuilder
    {
        // API v1
        /// <summary>
        ///     Builds a list of Country object based on a list of CountryFormat objects
        /// </summary>
        /// <param name="countries">A list of CountryFormats</param>
        /// <returns>A list of Countries</returns>
        public static List<Country> BuildCountries(List<CountryFormat> countries)
        {
            List<Country> countryModels = new List<Country>();
            foreach (CountryFormat country in countries)
            {
                Country countryModel = new Country
                {
                    CountryCode = country.CountryCode,
                    InternationalName = country.InternationalName,
                    ElectionTypes = new List<ElectionType>()
                };
                countryModels.Add(countryModel);
            }

            return countryModels;
        }

        /// <summary>
        ///     Takes a list of ElectionFormat and builds a list of Elections
        /// </summary>
        /// <param name="elections">A list of ElectionFormat</param>
        /// <returns>A list of Elections</returns>
        public static List<Election> BuildElections(List<ElectionFormat> elections)
        {
            List<Election> electionModels = new List<Election>();
            foreach (ElectionFormat election in elections)
            {
                Election electionModel = new Election
                {
                    Year = election.Year,
                    Algorithm = election.Algorithm,
                    FirstDivisor = election.FirstDivisor,
                    Threshold = election.Threshold,
                    Seats = election.Seats,
                    LevelingSeats = election.LevelingSeats,
                    Counties = new List<County>()
                };
                electionModels.Add(electionModel);
            }

            return electionModels;
        }

        /// <summary>
        ///     Takes a list of ElectionTypeFormat and builds a list of ElectionTypes
        /// </summary>
        /// <param name="electionTypes">A list of ElectionTypesFormat</param>
        /// <returns>A list of ElectionTypes</returns>
        public static List<ElectionType> BuildElectionTypes(List<ElectionTypeFormat> electionTypes)
        {
            List<ElectionType> electionTypeModels = new List<ElectionType>();
            foreach (ElectionTypeFormat electionType in electionTypes)
            {
                ElectionType electionTypeModel = new ElectionType
                {
                    InternationalName = electionType.InternationalName,
                    ElectionTypeCode = electionType.ElectionTypeCode,
                    Elections = new List<Election>()
                };
                electionTypeModels.Add(electionTypeModel);
            }

            return electionTypeModels;
        }

        /// <summary>
        ///     Takes a list of ResultFormat and a list of CountyDataFormat, and builds a list of Counties
        /// </summary>
        /// <param name="results">A list of ResultFormat</param>
        /// <param name="countyData">A list of CountyDataFormat</param>
        /// <returns>A list of Counties</returns>
        public static List<County> BuildCounties(List<ResultFormat> results, IEnumerable<CountyDataFormat> countyData)
        {
            IEnumerable<CountyDataFormat> countyDataFormats = countyData.ToList();

            Dictionary<string, County> countyModels = new Dictionary<string, County>();
            foreach (ResultFormat resultFormat in results)
            {
                if (countyModels.ContainsKey(resultFormat.Fylkenavn))
                {
                    continue;
                }
                
                try
                {
                    CountyDataFormat curCountyData =
                        countyDataFormats.Single(cD => cD.County.Equals(resultFormat.Fylkenavn));

                    County countyModel = new County
                    {
                        Name = resultFormat.Fylkenavn,
                        Seats = curCountyData.Seats,
                        Results = new List<Result>()
                    };
                    countyModels.Add(resultFormat.Fylkenavn, countyModel);
                }
                catch (InvalidOperationException)
                {
                    throw new ArgumentException($"Found no region with the name: {resultFormat.Fylkenavn}");
                }
            }

            return countyModels.Values.ToList();
        }

        /// <summary>
        ///     Takes a list of ResultFormat and builds a list of Results
        /// </summary>
        /// <param name="results">A list of ResultFormat</param>
        /// <returns>A list of Results</returns>
        public static List<Result> BuildResults(IEnumerable<ResultFormat> results)
        {
            List<Result> resultModels = new List<Result>();
            foreach (ResultFormat result in results)
            {
                Result resultModel = new Result
                {
                    PartyName = result.Partinavn,
                    PartyCode = result.Partikode,
                    Votes = result.AntallStemmerTotalt,
                    Percentage = result.OppslutningProsentvis
                };
                resultModels.Add(resultModel);
            }

            return resultModels;
        }

        // API v2
        /// <summary>
        /// Takes a list of CountyDataFormat and builds a list of DistrictMetrics
        /// </summary>
        /// <param name="countyData">A list of CountyDataFormat</param>
        /// <returns>A list of DistrictMetrics</returns>
        public static IEnumerable<DistrictMetrics> BuildDistrictMetrics(IEnumerable<CountyDataFormat> countyData)
        {
            return countyData.Select(data => new DistrictMetrics
                {Area = data.Area, District = data.County, ElectionYear = data.Year, Population = data.Population, Seats = data.Seats});
        }

        /// <summary>
        /// Takes a list of ElectionFormat, an election type and a list of DistrictMetrics, and returns a list of ElectionParameters.
        /// The returned list contains information about which parameters were used for each election.
        /// </summary>
        /// <param name="electionData"></param>
        /// <param name="electionType"></param>
        /// <param name="districtMetrics"></param>
        /// <returns></returns>
        public static IEnumerable<ElectionParameters> BuildElectionParameters(IEnumerable<ElectionFormat> electionData,
            string electionType, IEnumerable<DistrictMetrics> districtMetrics)
        {
            return electionData.Select(data =>
            {
                List<ListElement<int>> districtSeats = new List<ListElement<int>>{new ListElement<int>
                {
                    Key = "SUM",
                    Value = data.Seats
                }};

                if (data.AreaFactor < 0)
                {
                    districtSeats.AddRange(
                        districtMetrics.Where(dM => dM.ElectionYear == data.Year)
                            .Select(dM => new ListElement<int>
                                {
                                    Key = dM.District,
                                    Value = dM.Seats
                                }));
                }


                return new ElectionParameters
                {
                    Algorithm = BuildAlgorithmParameters(data),
                    AreaFactor = data.AreaFactor,
                    DistrictSeats = districtSeats,
                    ElectionType = electionType,
                    ElectionYear = data.Year,
                    LevelingSeats = data.LevelingSeats,
                    Threshold = data.Threshold,
                    TotalVotes = 0
                };
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
                        Algorithm = data.AlgorithmString, Parameters = new List<ListElement<double>>
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
                        {Algorithm = data.AlgorithmString, Parameters = new List<ListElement<double>>()};
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
                District = data.Fylkenavn, ElectionType = electionType, ElectionYear = electionYear,
                Party = data.Partikode, Votes = data.AntallStemmerTotalt
            });
        }
    }
}