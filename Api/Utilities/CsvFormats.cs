namespace Lavinia.Api.Utilities
{
    public interface ICsvFormat<out T>
    {
        T Parse(string line, FieldParser parser);
    }

    /// <summary>
    ///     Represents the parsed values of a line from the Elections.csv file
    /// </summary>
    public class ElectionFormat : ICsvFormat<ElectionFormat>
    {
        public int Year { get; private set; } = default!;
        public string AlgorithmString { get; private set; } = default!;
        public double FirstDivisor { get; private set; } = default!;
        public double Threshold { get; private set; } = default!;
        public double AreaFactor { get; private set; } = default!;
        public int Seats { get; private set; } = default!;
        public int LevelingSeats { get; private set; } = default!;

        /// <summary>
        ///     Parses a line following the ElectionFormat and returns an ElectionFormat object
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="parser">The field parser to use</param>
        /// <returns>An ElectionFormat object containing the parsed values</returns>
        public ElectionFormat Parse(string line, FieldParser parser)
        {
            string[] fields = parser.ParseLength(line, 7);
            int year = parser.ParseInt(fields[0], "Year");
            string algorithmString = parser.ParseAlgorithmToString(fields[1], "Algorithm");
            double firstDivisor = parser.ParseDouble(fields[2], "FirstDivisor");
            double threshold = parser.ParseDouble(fields[3], "Threshold");
            double areaFactor = parser.ParseDouble(fields[4], "AreaFactor");
            int seats = parser.ParseInt(fields[5], "Seats");
            int levelingSeats = parser.ParseInt(fields[6], "LevelingSeats");

            return new ElectionFormat
            {
                Year = year,
                AlgorithmString = algorithmString,
                FirstDivisor = firstDivisor,
                Threshold = threshold,
                AreaFactor = areaFactor,
                Seats = seats,
                LevelingSeats = levelingSeats
            };
        }
    }

    /// <summary>
    ///     Represents the parsed values of a line from the results files
    /// </summary>
    public class ResultFormat : ICsvFormat<ResultFormat>
    {
        public string Fylkenavn { get; private set; } = default!;
        public string Partikode { get; private set; } = default!;
        public string Partinavn { get; private set; } = default!;
        public double OppslutningProsentvis { get; private set; } = default!;
        public int AntallStemmerTotalt { get; private set; } = default!;

        /// <summary>
        ///     Parses a line following the ResultFormat and returns an ResultFormat object
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="parser">The field parser to use</param>
        /// <returns>An ResultFormat object containing the parsed values</returns>
        public ResultFormat Parse(string line, FieldParser parser)
        {
            string[] fields = parser.ParseLength(line, 18);
            string fylkenavn = parser.ParseString(fields[1], "Fylkenavn", 3, -1);
            string partikode = parser.ParseString(fields[6], "Partikode", 1, 10);
            string partinavn = parser.ParseString(fields[7], "Partinavn", 1, -1);
            double oppslutningProsentvis = parser.ParseDouble(fields[8], "Oppslutning prosentvis");
            int antallStemmerTotalt = parser.ParseInt(fields[12], "AntallStemmerTotalt");

            return new ResultFormat
            {
                Fylkenavn = fylkenavn,
                Partikode = partikode,
                Partinavn = partinavn,
                OppslutningProsentvis = oppslutningProsentvis,
                AntallStemmerTotalt = antallStemmerTotalt
            };
        }
    }

    /// <summary>
    ///     Represents the parsed values of a line from the CountyData.csv file
    /// </summary>
    public class CountyDataFormat : ICsvFormat<CountyDataFormat>
    {
        public double Area { get; private set; } = default!;
        public string County { get; private set; } = default!;
        public int Population { get; private set; } = default!;
        public int Seats { get; private set; } = default!;
        public int Year { get; private set; } = default!;

        /// <summary>
        ///     Parses a line following the CountyDataFormat and returns an CountyDataFormat object
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="parser">The field parser to use</param>
        /// <returns>An CountyDataFormat object containing the parsed values</returns>
        public CountyDataFormat Parse(string line, FieldParser parser)
        {
            string[] fields = parser.ParseLength(line, 5);
            int year = parser.ParseInt(fields[0], "Year");
            string county = parser.ParseString(fields[1], "County", 3, 35);
            double areal = parser.ParseDouble(fields[2], "Area");
            int population = parser.ParseInt(fields[3], "Population");
            int seats = parser.ParseInt(fields[4], "Seats");

            return new CountyDataFormat
            {
                Year = year,
                County = county,
                Area = areal,
                Population = population,
                Seats = seats
            };
        }
    }
}