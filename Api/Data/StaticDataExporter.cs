using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Lavinia.Api.Models;

using Microsoft.EntityFrameworkCore;

namespace Lavinia.Api.Data
{
    public static class StaticDataExporter
    {

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public static async Task ExportAsync(
            NOContext context,
            string outputDirectory,
            CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(outputDirectory);

            List<int> years = await context.ElectionParameters
                .Select(parameter => parameter.ElectionYear)
                .Distinct()
                .OrderByDescending(year => year)
                .ToListAsync(cancellationToken);

            Dictionary<string, string> parties = await context.Parties
                .OrderBy(party => party.Code)
                .ToDictionaryAsync(party => party.Code, party => party.Name, cancellationToken);

            List<PartyVotes> votes = await context.PartyVotes
                .OrderByDescending(vote => vote.ElectionYear)
                .ThenBy(vote => vote.District)
                .ThenBy(vote => vote.Party)
                .ToListAsync(cancellationToken);

            List<DistrictMetrics> metrics = await context.DistrictMetrics
                .OrderByDescending(metric => metric.ElectionYear)
                .ThenBy(metric => metric.District)
                .ToListAsync(cancellationToken);

            List<ElectionParameters> parameters = await context.ElectionParameters
                .Include(parameter => parameter.Algorithm)
                .ThenInclude(algorithm => algorithm.Parameters)
                .OrderByDescending(parameter => parameter.ElectionYear)
                .ThenBy(parameter => parameter.ElectionType)
                .ToListAsync(cancellationToken);


            await WriteJsonAsync(Path.Combine(outputDirectory, "years.json"), years, cancellationToken);
            await WriteJsonAsync(Path.Combine(outputDirectory, "parties.json"), parties, cancellationToken);
            await WriteJsonAsync(Path.Combine(outputDirectory, "votes.json"), votes, cancellationToken);
            await WriteJsonAsync(Path.Combine(outputDirectory, "metrics.json"), metrics, cancellationToken);
            await WriteJsonAsync(Path.Combine(outputDirectory, "parameters.json"), parameters, cancellationToken);
        }

        private static async Task WriteJsonAsync<T>(
            string filePath,
            T payload,
            CancellationToken cancellationToken)
        {
            string json = JsonSerializer.Serialize(payload, JsonSerializerOptions);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);
        }
    }
}