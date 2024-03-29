﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Lavinia.Api.Models;
using Lavinia.Api.Utilities;

using Microsoft.Extensions.Logging;

namespace Lavinia.Api.Data
{
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

                root = Path.Combine(root, "PE");

                // Parse all PartyVotes
                Dictionary<int, List<ResultFormat>> resultFormats = ParseResultFormat(root);
                List<PartyVotes> partyVotes = ParsePartyVotes(resultFormats, "PE").ToList();
                context.PartyVotes.AddRange(partyVotes);

                // Create list of all parties
                List<Party> parties = ParseParties(resultFormats).ToList();
                context.Parties.AddRange(parties);

                // Sum the total number of votes cast in an election
                IReadOnlyDictionary<int, int> yearTotalVotesMap = SumTotalVotes(partyVotes);

                // Parse all ElectionParameters
                List<ElectionParameters> electionParameters = ParseElectionParameters(root, yearTotalVotesMap).ToList();
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

        /// <summary>
        /// Check whether the DB is ready and empty
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static bool DatabaseIsReady(NOContext context)
        {
            context.Database.EnsureCreated();
            return !context.PartyVotes.Any();
        }

        /// <summary>
        /// Sum total votes
        /// </summary>
        /// <param name="partyVotes"></param>
        private static IReadOnlyDictionary<int, int> SumTotalVotes(IEnumerable<PartyVotes> partyVotes)
        {
            return partyVotes.Aggregate(
                new Dictionary<int, int>(),
                (acc, cur) =>
                {
                    if (!acc.TryAdd(cur.ElectionYear, cur.Votes))
                    {
                        acc[cur.ElectionYear] += cur.Votes;
                    }
                    return acc;
                });
        }

        /// <summary>
        /// Parses CountyData.csv -> DistrictMetrics
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static IEnumerable<DistrictMetrics> ParseDistrictMetrics(string path)
        {
            string filePath = Path.Combine(path, "CountyData.csv");
            IEnumerable<CountyDataFormat> countyData = CsvUtilities.CsvToList<CountyDataFormat>(filePath);
            IEnumerable<DistrictMetrics> districtMetricsModels = ModelBuilder.BuildDistrictMetrics(countyData);
            return districtMetricsModels;
        }

        /// <summary>
        /// Parses Elections.csv -> ElectionParameters
        /// </summary>
        private static IEnumerable<ElectionParameters> ParseElectionParameters(
            string path,
            IReadOnlyDictionary<int, int> yearTotalVotesMap)
        {
            string filePath = Path.Combine(path, "Elections.csv");
            IEnumerable<ElectionFormat> electionData = CsvUtilities.CsvToList<ElectionFormat>(filePath);
            return ModelBuilder.BuildElectionParameters(
                electionData,
                "PE",
                yearTotalVotesMap);
        }

        /// <summary>
        /// Parses <c>&lt;year&gt;.csv</c> -> ResultFormat
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private static Dictionary<int, List<ResultFormat>> ParseResultFormat(string root)
        {
            Dictionary<int, List<ResultFormat>> resultFormats = new();

            string[] filePaths = Directory.GetFiles(root);

            foreach (string filePath in filePaths)
            {
                if (Path.GetFileName(filePath) == "Elections.csv")
                {
                    continue;
                }

                int electionYear = int.Parse(Path.GetFileNameWithoutExtension(filePath));
                resultFormats[electionYear] = CsvUtilities.CsvToList<ResultFormat>(filePath);
            }

            return resultFormats;
        }

        /// <summary>
        /// Parses ResultFormat -> PartyVotes
        /// </summary>
        /// <param name="resultFormat"></param>
        /// <param name="electionType"></param>
        /// <returns></returns>
        private static IEnumerable<PartyVotes> ParsePartyVotes(Dictionary<int, List<ResultFormat>> resultFormat, string electionType)
        {
            IEnumerable<PartyVotes> partyVotes = new List<PartyVotes>();

            foreach (KeyValuePair<int, List<ResultFormat>> pair in resultFormat)
            {
                partyVotes = partyVotes.Concat(ModelBuilder.BuildPartyVotes(pair.Value, electionType, pair.Key));
            }

            return partyVotes;
        }

        /// <summary>
        /// Parses ResultFormat -> Party
        /// </summary>
        /// <param name="resultFormat"></param>
        /// <returns></returns>
        private static IEnumerable<Party> ParseParties(Dictionary<int, List<ResultFormat>> resultFormat)
        {
            Dictionary<string, string> filteredParties = new();

            foreach (KeyValuePair<int, List<ResultFormat>> pair in resultFormat)
            {
                filteredParties = UpdateFilter(pair.Value, filteredParties);
            }

            IEnumerable<Party> parties = filteredParties.Keys.Select(partyCode => new Party
            {
                Code = partyCode,
                Name = filteredParties[partyCode]
            });

            return parties;
        }

        private static Dictionary<string, string> UpdateFilter(List<ResultFormat> parties, Dictionary<string, string> partyDict)
        {
            Dictionary<string, string> currentMap = new(partyDict);
            parties.ForEach(party =>
            {
                if (!currentMap.ContainsKey(party.Partikode))
                {
                    currentMap.Add(party.Partikode, party.Partinavn);
                }
                else if (!currentMap[party.Partikode].Equals(party.Partinavn))
                {
                    throw new ArgumentException($"Found duplicate party code mapping! Existing mapping: {party.Partikode} - {currentMap[party.Partikode]}, new mapping: {party.Partikode} - {party.Partinavn}");
                }
            });

            return currentMap;
        }
    }
}