﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Models;
using LaviniaApi.Utilities;
using Microsoft.Extensions.Logging;

namespace LaviniaApi.Data
{
    // API v2
    public static class ApiInitializer
    {
        /// <summary>
        ///     Initializes the database, if the db is empty this method will build a model to seed it.
        /// </summary>
        /// <param name="context">The context to be initialized.</param>
        /// <param name="logger">Where to log any issues.</param>
        public static void Initialize(ElectionContext context, ILogger logger)
        {
            string root = Path.Combine("Data", "Countries", "NO");

            // Make sure the DB is ready and empty
            context.Database.EnsureCreated();
            if (context.PartyVotes.Any())
            {
                return;
            }

            // Catch all Argument/KeyNotFound/CsvFileFormatExceptions thrown by model validation
            try
            {
                IEnumerable<DistrictMetrics> districtMetrics = ParseDistrictMetrics(root);
                context.DistrictMetrics.AddRange(districtMetrics);

                root = Path.Combine(root, "PE");
                IEnumerable<ElectionParameters> electionParameters = ParseElectionParameters(root);
                context.ElectionParameters.AddRange(electionParameters);

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

        private static IEnumerable<DistrictMetrics> ParseDistrictMetrics(string path)
        {
            string filePath = Path.Combine(path, "CountyData.csv");
            IEnumerable<CountyDataFormat> countyData = CsvUtilities.CsvToList<CountyDataFormat>(filePath);
            IEnumerable<DistrictMetrics> districtMetricsModels = ModelBuilder.BuildDistrictMetrics(countyData);
            return districtMetricsModels;
        }

        private static IEnumerable<ElectionParameters> ParseElectionParameters(string path)
        {
            string filePath = Path.Combine(path, "Elections.csv");
            IEnumerable<ElectionFormat> electionData = CsvUtilities.CsvToList<ElectionFormat>(filePath);
            IEnumerable<ElectionParameters> electionParameterModels = ModelBuilder.BuildElectionParameters(electionData, "PE");
            return electionParameterModels;
        }
    }
}