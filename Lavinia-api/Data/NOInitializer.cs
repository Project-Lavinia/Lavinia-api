using System;
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
    public static class NOInitializer
    {
        /// <summary>
        ///     Initializes the database, if the db is empty this method will build a model to seed it.
        /// </summary>
        /// <param name="context">The context to be initialized.</param>
        /// <param name="logger">Where to log any issues.</param>
        public static void Initialize(NOContext context, ILogger logger)
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
                List<ElectionParameters> electionParameters = new List<ElectionParameters>(ParseElectionParameters(root)) ;
                context.ElectionParameters.AddRange(electionParameters);

                List<PartyVotes> partyVotes = new List<PartyVotes>(ParsePartyVotes(root));
                context.PartyVotes.AddRange(partyVotes);

                foreach (PartyVotes partyVote in partyVotes)
                {
                    int year = partyVote.ElectionYear;
                    int votes = partyVote.Votes;
                    try
                    {
                        electionParameters.First(eP => eP.ElectionYear == year).TotalVotes += votes;
                    }
                    catch (InvalidOperationException)
                    {
                        Console.WriteLine("Could not find any ElectionParameter for the year: " + year);
                        Environment.Exit(0);
                    }
                    
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

        // Parses CountyData.csv -> DistrictMetrics
        private static IEnumerable<DistrictMetrics> ParseDistrictMetrics(string path)
        {
            string filePath = Path.Combine(path, "CountyData.csv");
            IEnumerable<CountyDataFormat> countyData = CsvUtilities.CsvToList<CountyDataFormat>(filePath);
            IEnumerable<DistrictMetrics> districtMetricsModels = ModelBuilder.BuildDistrictMetrics(countyData);
            return districtMetricsModels;
        }

        // Parses Elections.csv -> ElectionParameters
        private static IEnumerable<ElectionParameters> ParseElectionParameters(string path)
        {
            string filePath = Path.Combine(path, "Elections.csv");
            IEnumerable<ElectionFormat> electionData = CsvUtilities.CsvToList<ElectionFormat>(filePath);
            IEnumerable<ElectionParameters> electionParameterModels = ModelBuilder.BuildElectionParameters(electionData, "PE");
            return electionParameterModels;
        }

        // Parses <year>.csv -> PartyVotes
        private static IEnumerable<PartyVotes> ParsePartyVotes(string root)
        {
            IEnumerable<PartyVotes> partyVotes = new List<PartyVotes>();

            string electionType = Path.GetFileName(root);
            string[] filePaths = Directory.GetFiles(root);

            foreach (string filePath in filePaths)
            {
                if (Path.GetFileName(filePath) == "Elections.csv")
                {
                    continue;
                }

                int electionYear = int.Parse(Path.GetFileNameWithoutExtension(filePath));
                IEnumerable<ResultFormat> election = CsvUtilities.CsvToList<ResultFormat>(filePath);
                partyVotes = partyVotes.Concat(ModelBuilder.BuildPartyVotes(election, electionType, electionYear));
            }

            return partyVotes;
        }
    }
}