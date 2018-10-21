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
            Dictionary<string, County> countyModels = new Dictionary<string, County>();
            foreach (ResultFormat resultFormat in results)
            {
                if (!countyModels.ContainsKey(resultFormat.Fylkenavn))
                {
                    CountyDataFormat curCountyData = countyData.Single(cD => cD.County.Equals(resultFormat.Fylkenavn));

                    County countyModel = new County
                    {
                        Name = resultFormat.Fylkenavn,
                        Seats = curCountyData.Seats,
                        Results = new List<Result>()
                    };
                    countyModels.Add(resultFormat.Fylkenavn, countyModel);
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
        public static List<DistrictMetrics> BuildDistrictMetrics(IEnumerable<CountyDataFormat> countyData)
        {
            return countyData.Select(data => new DistrictMetrics {Area = data.Area, District = data.County, ElectionYear = data.Year, Population = data.Population}).ToList();
        }
    }
}