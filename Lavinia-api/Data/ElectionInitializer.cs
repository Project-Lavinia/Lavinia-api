using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LaviniaApi.Models;
using LaviniaApi.Utilities;
using Microsoft.Extensions.Logging;

namespace LaviniaApi.Data
{
    public class ElectionInitializer
    {
        /// <summary>
        ///     Initializes the database, if the db is empty this method will build a model to seed it.
        /// </summary>
        /// <param name="context">The context to be initialized.</param>
        /// <param name="logger">Where to log any issues.</param>
        public static void Initialize(ElectionContext context, ILogger logger)
        {
            string root = Path.Combine("Data", "Countries");

            // Make sure the DB is ready and empty
            context.Database.EnsureCreated();
            if (context.Countries.Any()) return;

            // Catch all Argument/KeyNotFound/CsvFileFormatExceptions thrown by model validation
            try
            {
                string path = Path.Combine(root, "Countries.csv");
                List<CountryFormat> countries = CsvUtilities.CsvToList<CountryFormat>(path);
                List<Country> countryModels = ModelBuilder.BuildCountries(countries);
                context.Countries.AddRange(countryModels);

                // Iterate through countries
                string[] countryDirectories = Directory.GetDirectories(root);
                if (countryDirectories.Length != countryModels.Count)
                    throw new ArgumentException(
                        $"The number of directories in {root} does not match the number found in States.csv");

                foreach (Country country in countryModels)
                {
                    path = Path.Combine(root, country.CountryCode);
                    List<ElectionType> electionTypes = CreateElectionTypes(path);
                    country.ElectionTypes.AddRange(electionTypes);

                    HashSet<int> validated = new HashSet<int>();
                    CustomValidation.ValidateCountry(country, validated);
                }

                context.SaveChanges();
            }
            catch (ArgumentException argumentException)
            {
                logger.LogError(argumentException, "The data results in an illegal model and could not be built.");
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                logger.LogError(keyNotFoundException,
                    "The directory name does not match any ID in the dictionary.");
            }
            catch (CsvFileFormatException csvFileFormatException)
            {
                logger.LogError(csvFileFormatException, "The csv file has a malformed format.");
            }
        }

        private static List<ElectionType> CreateElectionTypes(string root)
        {
            // Check if the countryId is valid
            if (!Directory.Exists(root))
                throw new KeyNotFoundException($"Could not find any directory with the path: {root}");

            // Get a list of ElectionTypeFormats
            List<ElectionTypeFormat> electionTypes =
                CsvUtilities.CsvToList<ElectionTypeFormat>(Path.Combine(root, "ElectionTypes.csv"));
            List<ElectionType> electionTypeModels = ModelBuilder.BuildElectionTypes(electionTypes);

            List<CountyDataFormat> countyData =
                CsvUtilities.CsvToList<CountyDataFormat>(Path.Combine(root, "CountyData.csv"));

            // Iterate through the country's election types
            string[] electionTypeFiles = Directory.GetDirectories(root);
            if (electionTypeFiles.Length != electionTypes.Count)
                throw new ArgumentException(
                    $"The number of directories in {root} does not match the number found in ElectionTypes.csv.");

            foreach (ElectionType electionTypeModel in electionTypeModels)
            {
                string path = Path.Combine(root, electionTypeModel.ElectionTypeCode);
                List<Election> elections = CreateElection(countyData, path);
                electionTypeModel.Elections.AddRange(elections);
            }

            return electionTypeModels;
        }

        private static List<Election> CreateElection(List<CountyDataFormat> countyData, string root)
        {
            // Check if the electionTypeId is valid
            if (!Directory.Exists(root))
                throw new KeyNotFoundException($"Could not find any directory with the path: {root}");

            List<ElectionFormat> elections =
                CsvUtilities.CsvToList<ElectionFormat>(Path.Combine(root, "Elections.csv"));
            List<Election> electionModels = ModelBuilder.BuildElections(elections);

            string[] electionFiles = Directory.GetFiles(root);
            if (electionFiles.Length != elections.Count + 1)
                throw new ArgumentException(
                    $"The number of elections in {root} does not match the number found in Elections.csv.");

            // Iterate through the elections
            foreach (Election electionModel in electionModels)
            {
                string path = Path.Combine(root, electionModel.Year + ".csv");
                List<County> counties = CreateCounties(countyData.Where(cD => cD.Year == electionModel.Year), path);
                electionModel.Counties.AddRange(counties);
            }

            return electionModels;
        }

        private static List<County> CreateCounties(IEnumerable<CountyDataFormat> countyData, string root)
        {
            List<ResultFormat> resultFormat = CsvUtilities.CsvToList<ResultFormat>(root);
            List<County> countyModels = ModelBuilder.BuildCounties(resultFormat, countyData);

            foreach (County county in countyModels)
            {
                List<Result> results = CreateResults(resultFormat.Where(r => r.Fylkenavn.Equals(county.Name)));
                county.Results.AddRange(results);
            }

            return countyModels;
        }

        private static List<Result> CreateResults(IEnumerable<ResultFormat> resultFormat)
        {
            return ModelBuilder.BuildResults(resultFormat);
        }
    }
}