using System.Text.Json;
using ConsoleAppFramework;
using LaviniaJsonExporter;

var app = ConsoleApp.Create();
app.Add<ExportCommands>();
app.Run(args);

/// <summary>
/// Commands for exporting Norwegian election data to JSON files
/// </summary>
public class ExportCommands
{
    /// <summary>
    /// Export election data from CSV files to JSON format
    /// </summary>
    /// <param name="outputDir">-o, Output directory for JSON files (default: output)</param>
    /// <param name="dataDir">-d, Data directory containing CSV files (default: ../Api/Data/Countries/NO)</param>
    [Command("")]
    public void Export(
        string outputDir = "output",
        string? dataDir = null)
    {
        Directory.CreateDirectory(outputDir);

        // Find data directory
        var dataRoot = dataDir ?? (Directory.Exists("Data")
            ? Path.Combine("Data", "Countries", "NO")
            : Path.Combine("..", "Api", "Data", "Countries", "NO"));

        var peRoot = Path.Combine(dataRoot, "PE");

        if (!Directory.Exists(dataRoot))
        {
            throw new DirectoryNotFoundException($"Data directory not found at {dataRoot}");
        }

        // Load district metrics
        var districtMetrics = CsvReader.ReadCsv(
            Path.Combine(dataRoot, "CountyData.csv"),
            fields => new DistrictMetrics(
                District: fields[1],
                ElectionYear: int.Parse(fields[0]),
                Area: double.Parse(fields[2]),
                Population: int.Parse(fields[3]),
                Seats: int.Parse(fields[4])
            )
        );

        // Load all election result files and build party votes
        var partyVotes = new List<PartyVotes>();
        var parties = new Dictionary<string, string>();

        foreach (var file in Directory.GetFiles(peRoot, "*.csv").Where(f => !f.EndsWith("Elections.csv")))
        {
            var year = int.Parse(Path.GetFileNameWithoutExtension(file));
            var results = CsvReader.ReadCsv(file, fields => new
            {
                District = fields[1],      // Fylkenavn
                PartyCode = fields[6],     // Partikode
                PartyName = fields[7],     // Partinavn
                Votes = int.Parse(fields[12])  // Antall stemmer totalt
            });

            foreach (var result in results)
            {
                partyVotes.Add(new PartyVotes(
                    Party: result.PartyCode,
                    Votes: result.Votes,
                    District: result.District,
                    ElectionYear: year,
                    ElectionType: "PE"
                ));

                if (!parties.ContainsKey(result.PartyCode))
                    parties[result.PartyCode] = result.PartyName;
            }
        }

        // Calculate total votes per year
        var yearTotalVotes = partyVotes
            .GroupBy(v => v.ElectionYear)
            .ToDictionary(g => g.Key, g => g.Sum(v => v.Votes));

        // Load election parameters
        var electionParameters = CsvReader.ReadCsv(
            Path.Combine(peRoot, "Elections.csv"),
            fields =>
            {
                var year = int.Parse(fields[0]);
                var algorithm = fields[1];
                var firstDivisor = string.IsNullOrEmpty(fields[2]) ? 0.0 : double.Parse(fields[2]);
                var parameters = new List<AlgorithmParameter>();

                if (algorithm == "Sainte Laguës (modified)" && firstDivisor > 0)
                    parameters.Add(new AlgorithmParameter("First Divisor", firstDivisor));

                return new ElectionParameters(
                    ElectionYear: year,
                    ElectionType: "PE",
                    Algorithm: new AlgorithmParameters(algorithm, parameters),
                    Threshold: double.Parse(fields[3]),
                    AreaFactor: double.Parse(fields[4]),
                    DistrictSeats: int.Parse(fields[5]),
                    LevelingSeats: int.Parse(fields[6]),
                    TotalVotes: yearTotalVotes.GetValueOrDefault(year, 0)
                );
            }
        );

        // Get years
        var years = electionParameters.Select(e => e.ElectionYear).OrderByDescending(y => y).ToList();

        // Get districts
        var districts = districtMetrics.Select(d => d.District).Distinct().ToList();

        // Serialize to JSON using source-generated serialization for AOT
        File.WriteAllText(Path.Combine(outputDir, "years.json"), JsonSerializer.Serialize(years, AppJsonContext.Default.ListInt32));
        File.WriteAllText(Path.Combine(outputDir, "parties.json"), JsonSerializer.Serialize(parties, AppJsonContext.Default.DictionaryStringString));
        File.WriteAllText(Path.Combine(outputDir, "districts.json"), JsonSerializer.Serialize(districts, AppJsonContext.Default.ListString));
        File.WriteAllText(Path.Combine(outputDir, "votes.json"), JsonSerializer.Serialize(partyVotes, AppJsonContext.Default.ListPartyVotes));
        File.WriteAllText(Path.Combine(outputDir, "metrics.json"), JsonSerializer.Serialize(districtMetrics, AppJsonContext.Default.ListDistrictMetrics));
        File.WriteAllText(Path.Combine(outputDir, "parameters.json"), JsonSerializer.Serialize(electionParameters, AppJsonContext.Default.ListElectionParameters));

        Console.WriteLine($"JSON files exported to '{outputDir}' directory:");
        Console.WriteLine($"  - years.json ({years.Count} years)");
        Console.WriteLine($"  - parties.json ({parties.Count} parties)");
        Console.WriteLine($"  - districts.json ({districts.Count} districts)");
        Console.WriteLine($"  - votes.json ({partyVotes.Count} records)");
        Console.WriteLine($"  - metrics.json ({districtMetrics.Count} records)");
        Console.WriteLine($"  - parameters.json ({electionParameters.Count} records)");
    }
}
