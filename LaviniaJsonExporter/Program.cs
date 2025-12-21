using System.Text.Json;
using LaviniaJsonExporter;

try
{
    var outputDir = args.Length > 0 ? args[0] : "output";
    Directory.CreateDirectory(outputDir);

    // Find data directory (look in both current dir and parent Api folder)
    var dataRoot = Directory.Exists("Data") 
        ? Path.Combine("Data", "Countries", "NO")
        : Path.Combine("..", "Api", "Data", "Countries", "NO");

    var peRoot = Path.Combine(dataRoot, "PE");

    if (!Directory.Exists(dataRoot))
    {
        Console.WriteLine($"Error: Data directory not found at {dataRoot}");
        return 1;
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

    // Serialize to JSON with pretty printing
    var options = new JsonSerializerOptions { WriteIndented = true };

    File.WriteAllText(Path.Combine(outputDir, "years.json"), JsonSerializer.Serialize(years, options));
    File.WriteAllText(Path.Combine(outputDir, "parties.json"), JsonSerializer.Serialize(parties, options));
    File.WriteAllText(Path.Combine(outputDir, "districts.json"), JsonSerializer.Serialize(districts, options));
    File.WriteAllText(Path.Combine(outputDir, "votes.json"), JsonSerializer.Serialize(partyVotes, options));
    File.WriteAllText(Path.Combine(outputDir, "metrics.json"), JsonSerializer.Serialize(districtMetrics, options));
    File.WriteAllText(Path.Combine(outputDir, "parameters.json"), JsonSerializer.Serialize(electionParameters, options));

    Console.WriteLine($"JSON files exported to '{outputDir}' directory:");
    Console.WriteLine($"  - years.json ({years.Count} years)");
    Console.WriteLine($"  - parties.json ({parties.Count} parties)");
    Console.WriteLine($"  - districts.json ({districts.Count} districts)");
    Console.WriteLine($"  - votes.json ({partyVotes.Count} records)");
    Console.WriteLine($"  - metrics.json ({districtMetrics.Count} records)");
    Console.WriteLine($"  - parameters.json ({electionParameters.Count} records)");
    
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return 1;
}
