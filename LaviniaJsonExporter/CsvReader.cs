namespace LaviniaJsonExporter;

public static class CsvReader
{
    public static List<T> ReadCsv<T>(string filePath, Func<string[], T> parser)
    {
        var results = new List<T>();
        var lines = File.ReadAllLines(filePath);
        
        // Skip header
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;
                
            var fields = line.Split(';');
            results.Add(parser(fields));
        }
        
        return results;
    }
}
