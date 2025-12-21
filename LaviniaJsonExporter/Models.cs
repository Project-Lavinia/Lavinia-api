namespace LaviniaJsonExporter;

public record Party(string Code, string Name);

public record PartyVotes(string Party, int Votes, string District, int ElectionYear, string ElectionType);

public record DistrictMetrics(string District, int ElectionYear, double Area, int Population, int Seats);

public record AlgorithmParameter(string Key, double Value);

public record AlgorithmParameters(string Algorithm, List<AlgorithmParameter> Parameters);

public record ElectionParameters(
    int ElectionYear,
    string ElectionType,
    AlgorithmParameters Algorithm,
    double Threshold,
    double AreaFactor,
    int DistrictSeats,
    int LevelingSeats,
    int TotalVotes
);
