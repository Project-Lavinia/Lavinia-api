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

            if (!DatabaseIsReady(context))
            {
                return;
            }

            // Catch all Argument/KeyNotFound/CsvFileFormatExceptions thrown by model validation
            try
            {
                // Parse all DistrictMetrics
                IEnumerable<DistrictMetrics> districtMetrics = ParseDistrictMetrics(root).ToList();
                context.DistrictMetrics.AddRange(districtMetrics);
                
                // Parse all ElectionParameters
                root = Path.Combine(root, "PE");
                List<ElectionParameters> electionParameters = ParseElectionParameters(root, districtMetrics).ToList();
                context.ElectionParameters.AddRange(electionParameters);

                // Parse all PartyVotes
                List<PartyVotes> partyVotes = ParsePartyVotes(root).ToList();
                context.PartyVotes.AddRange(partyVotes);

                // Parse all Parties
                List<Party> parties = ParseParties(root).ToList();
                context.Parties.AddRange(parties);

                // Sum the total number of votes cast in an election
                SumTotalVotes(electionParameters, partyVotes);
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

        // Check whether the DB is ready and empty
        private static bool DatabaseIsReady(NOContext context)
        {
            context.Database.EnsureCreated();
            return !context.PartyVotes.Any();
        }

        // Sum total votes
        private static void SumTotalVotes(IReadOnlyCollection<ElectionParameters> electionParameters, IEnumerable<PartyVotes> partyVotes)
        {
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
                    throw new ArgumentException($"Could not find any ElectionParameter for the year: {year}");
                }
                    
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
        private static IEnumerable<ElectionParameters> ParseElectionParameters(string path, IEnumerable<DistrictMetrics> districtMetrics)
        {
            string filePath = Path.Combine(path, "Elections.csv");
            IEnumerable<ElectionFormat> electionData = CsvUtilities.CsvToList<ElectionFormat>(filePath);
            IEnumerable<ElectionParameters> electionParameterModels = ModelBuilder.BuildElectionParameters(electionData, "PE", districtMetrics);
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

        // Parses <year>.csv -> Party
        private static IEnumerable<Party> ParseParties(string root)
        {
            IEnumerable<Party> parties = new List<Party>();
            HashSet<string> existingParties = new HashSet<string>();

            string[] filePaths = Directory.GetFiles(root);

            foreach (string filePath in filePaths)
            {
                if (Path.GetFileName(filePath) == "Elections.csv")
                {
                    continue;
                }

                List<ResultFormat> result = CsvUtilities.CsvToList<ResultFormat>(filePath);
                List<ResultFormat> filteredResults = new List<ResultFormat>();
                
                result.ForEach(r =>
                {
                    if (!existingParties.Contains(r.Partikode))
                    {
                        existingParties.Add(r.Partikode);
                        filteredResults.Add(r);
                    }
                });
                parties = parties.Concat(ModelBuilder.BuildParties(filteredResults));
            }

            return parties;
        }
    }
}