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
    /// <param name="report">-r, Generate a summary report for CI/CD review (default: false)</param>
    [Command("")]
    public void Export(
        string outputDir = "output",
        string? dataDir = null,
        bool report = false)
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

        if (report)
        {
            GenerateReport(years, parties, districts, partyVotes, districtMetrics, electionParameters, outputDir);
        }
    }

    private void GenerateReport(
        List<int> years,
        Dictionary<string, string> parties,
        List<string> districts,
        List<PartyVotes> partyVotes,
        List<DistrictMetrics> districtMetrics,
        List<ElectionParameters> electionParameters,
        string outputDir)
    {
        var report = new System.Text.StringBuilder();
        
        report.AppendLine();
        report.AppendLine("═══════════════════════════════════════════════════════════");
        report.AppendLine("           DATA EXPORT SUMMARY REPORT");
        report.AppendLine("═══════════════════════════════════════════════════════════");
        report.AppendLine();
        
        // Overview section
        report.AppendLine("OVERVIEW");
        report.AppendLine("─────────────────────────────────────────────────────────");
        report.AppendLine($"Export Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        report.AppendLine($"Output Directory: {outputDir}");
        report.AppendLine();
        
        // Years section
        report.AppendLine("ELECTION YEARS");
        report.AppendLine("─────────────────────────────────────────────────────────");
        report.AppendLine($"Total Years: {years.Count}");
        report.AppendLine($"Year Range: {years.Min()} - {years.Max()}");
        report.AppendLine($"Years: {string.Join(", ", years)}");
        report.AppendLine();
        
        // Parties section
        report.AppendLine("POLITICAL PARTIES");
        report.AppendLine("─────────────────────────────────────────────────────────");
        report.AppendLine($"Total Parties: {parties.Count}");
        var topParties = partyVotes
            .GroupBy(v => v.Party)
            .Select(g => new { Party = g.Key, TotalVotes = g.Sum(v => v.Votes) })
            .OrderByDescending(p => p.TotalVotes)
            .Take(5)
            .ToList();
        report.AppendLine("Top 5 Parties by Total Votes:");
        foreach (var p in topParties)
        {
            var partyName = parties.ContainsKey(p.Party) ? parties[p.Party] : p.Party;
            report.AppendLine($"  {p.Party,-6} {partyName,-30} {p.TotalVotes,12:N0} votes");
        }
        report.AppendLine();
        
        // Districts section
        report.AppendLine("DISTRICTS");
        report.AppendLine("─────────────────────────────────────────────────────────");
        report.AppendLine($"Total Districts: {districts.Count}");
        report.AppendLine($"Districts: {string.Join(", ", districts.OrderBy(d => d))}");
        report.AppendLine();
        
        // Votes section
        report.AppendLine("VOTE DATA");
        report.AppendLine("─────────────────────────────────────────────────────────");
        report.AppendLine($"Total Vote Records: {partyVotes.Count}");
        report.AppendLine($"Total Votes Cast: {partyVotes.Sum(v => v.Votes):N0}");
        var votesPerYear = partyVotes
            .GroupBy(v => v.ElectionYear)
            .Select(g => new { Year = g.Key, Votes = g.Sum(v => v.Votes) })
            .OrderByDescending(v => v.Year)
            .ToList();
        report.AppendLine("Votes by Year:");
        foreach (var y in votesPerYear)
        {
            report.AppendLine($"  {y.Year}: {y.Votes,12:N0} votes");
        }
        report.AppendLine();
        
        // District Metrics section
        report.AppendLine("DISTRICT METRICS");
        report.AppendLine("─────────────────────────────────────────────────────────");
        report.AppendLine($"Total Metric Records: {districtMetrics.Count}");
        var latestMetrics = districtMetrics.Where(m => m.ElectionYear == years.Max()).ToList();
        report.AppendLine($"Latest Year ({years.Max()}) Metrics:");
        report.AppendLine($"  Total Population: {latestMetrics.Sum(m => m.Population):N0}");
        report.AppendLine($"  Total Area: {latestMetrics.Sum(m => m.Area):N0} km²");
        report.AppendLine($"  Total Seats: {latestMetrics.Sum(m => m.Seats):N0}");
        report.AppendLine();
        
        // Election Parameters section
        report.AppendLine("ELECTION PARAMETERS");
        report.AppendLine("─────────────────────────────────────────────────────────");
        report.AppendLine($"Total Parameter Records: {electionParameters.Count}");
        var latestParams = electionParameters.FirstOrDefault(p => p.ElectionYear == years.Max());
        if (latestParams != null)
        {
            report.AppendLine($"Latest Election ({latestParams.ElectionYear}) Parameters:");
            report.AppendLine($"  Algorithm: {latestParams.Algorithm.Algorithm}");
            report.AppendLine($"  Threshold: {latestParams.Threshold}%");
            report.AppendLine($"  District Seats: {latestParams.DistrictSeats}");
            report.AppendLine($"  Leveling Seats: {latestParams.LevelingSeats}");
            report.AppendLine($"  Total Seats: {latestParams.DistrictSeats + latestParams.LevelingSeats}");
        }
        report.AppendLine();
        
        // File sizes section
        report.AppendLine("OUTPUT FILES");
        report.AppendLine("─────────────────────────────────────────────────────────");
        var files = new[]
        {
            "years.json", "parties.json", "districts.json",
            "votes.json", "metrics.json", "parameters.json"
        };
        long totalSize = 0;
        foreach (var file in files)
        {
            var path = Path.Combine(outputDir, file);
            if (File.Exists(path))
            {
                var size = new FileInfo(path).Length;
                totalSize += size;
                report.AppendLine($"  {file,-18} {FormatFileSize(size),10}");
            }
        }
        report.AppendLine($"  {"Total:",-18} {FormatFileSize(totalSize),10}");
        report.AppendLine();
        
        report.AppendLine("═══════════════════════════════════════════════════════════");
        
        var reportText = report.ToString();
        Console.WriteLine(reportText);
        
        // Save report to file
        var reportPath = Path.Combine(outputDir, "export-report.txt");
        File.WriteAllText(reportPath, reportText);
        Console.WriteLine($"Report saved to: {reportPath}");
    }

    private string FormatFileSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        return $"{bytes / (1024.0 * 1024.0):F1} MB";
    }
}
