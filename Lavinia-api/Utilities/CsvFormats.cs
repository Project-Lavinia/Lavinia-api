namespace LaviniaApi.Utilities
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
        public int Year { get; private set; }
        public Algorithm Algorithm { get; private set; }
        public double FirstDivisor { get; private set; }
        public double Threshold { get; private set; }
        public int Seats { get; private set; }
        public int LevelingSeats { get; private set; }

        /// <summary>
        ///     Parses a line following the ElectionFormat and returns an ElectionFormat object
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="parser">The field parser to use</param>
        /// <returns>An ElectionFormat object containing the parsed values</returns>
        public ElectionFormat Parse(string line, FieldParser parser)
        {
            string[] fields = parser.ParseLength(line, 6);
            int year = parser.ParseInt(fields[0], "Year");
            Algorithm algorithm = parser.ParseAlgorithm(fields[1], "Algorithm");
            double firstDivisor = parser.ParseDouble(fields[2], "FirstDivisor");
            double threshold = parser.ParseDouble(fields[3], "Threshold");
            int seats = parser.ParseInt(fields[4], "Seats");
            int levelingSeats = parser.ParseInt(fields[5], "LevelingSeats");

            return new ElectionFormat
            {
                Year = year,
                Algorithm = algorithm,
                FirstDivisor = firstDivisor,
                Threshold = threshold,
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
        private int Fylkenummer { get; set; }
        public string Fylkenavn { get; private set; }
        public string Partikode { get; private set; }
        public string Partinavn { get; private set; }
        public double OppslutningProsentvis { get; private set; }
        private int AntallStemmeberettigede { get; set; }
        private int AntallForhåndsstemmer { get; set; }
        private int AntallValgtingstemmer { get; set; }
        public int AntallStemmerTotalt { get; private set; }
        private double EndringProsentSisteTilsvarendeValg { get; set; }
        private double EndringProsentSisteEkvivalenteValg { get; set; }
        private int AntallMandater { get; set; }
        private int AntallUtjevningsmandater { get; set; }

        /// <summary>
        ///     Parses a line following the ResultFormat and returns an ResultFormat object
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="parser">The field parser to use</param>
        /// <returns>An ResultFormat object containing the parsed values</returns>
        public ResultFormat Parse(string line, FieldParser parser)
        {
            string[] fields = parser.ParseLength(line, 18);
            int fylkenummer = parser.ParseInt(fields[0], "Fylkenummer");
            string fylkenavn = parser.ParseString(fields[1], "Fylkenavn", 3, -1);
            string partikode = parser.ParseString(fields[6], "Partikode", 1, 10);
            string partinavn = parser.ParseString(fields[7], "Partinavn", 1, -1);
            double oppslutningProsentvis = parser.ParseDouble(fields[8], "Oppslutning prosentvis");
            int antallStemmeberettigde = parser.ParseInt(fields[9], "Antall stemmeberettigde");
            int antallForhåndsstemmer = parser.ParseInt(fields[10], "Antall forhåndsstemmer");
            int antallValgtingstemmer = parser.ParseInt(fields[11], "Antall valgtingstemmer");
            int antallStemmerTotalt = parser.ParseInt(fields[12], "AntallStemmerTotalt");
            double endringProsentSisteTilsvarendeValg =
                parser.ParseDouble(fields[13], "Endring prosent siste tilsvarende valg");
            double endringProsentSisteEkvivalenteValg =
                parser.ParseDouble(fields[14], "Endring prosent siste ekvivalente valg");
            int antallMandater = parser.ParseInt(fields[15], "Antall mandater");
            int antallUtjevningsmandater = parser.ParseInt(fields[16], "Antall utjevningsmandater");

            return new ResultFormat
            {
                Fylkenummer = fylkenummer,
                Fylkenavn = fylkenavn,
                Partikode = partikode,
                Partinavn = partinavn,
                OppslutningProsentvis = oppslutningProsentvis,
                AntallStemmeberettigede = antallStemmeberettigde,
                AntallForhåndsstemmer = antallForhåndsstemmer,
                AntallValgtingstemmer = antallValgtingstemmer,
                AntallStemmerTotalt = antallStemmerTotalt,
                EndringProsentSisteTilsvarendeValg = endringProsentSisteTilsvarendeValg,
                EndringProsentSisteEkvivalenteValg = endringProsentSisteEkvivalenteValg,
                AntallMandater = antallMandater,
                AntallUtjevningsmandater = antallUtjevningsmandater
            };
        }
    }

    /// <summary>
    ///     Represents the parsed values of a line from the Countries.csv file
    /// </summary>
    public class CountryFormat : ICsvFormat<CountryFormat>
    {
        public string CountryCode;
        public string InternationalName;

        /// <summary>
        ///     Parses a line following the CountryFormat and returns an CountryFormat object
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="parser">The field parser to use</param>
        /// <returns>An CountryFormat object containing the parsed values</returns>
        public CountryFormat Parse(string line, FieldParser parser)
        {
            string[] fields = parser.ParseLength(line, 2);
            string countryCode = parser.ParseString(fields[0], "Country code", 2, 5);
            string internationalName = parser.ParseString(fields[1], "International name", 3, 30);

            return new CountryFormat
            {
                CountryCode = countryCode,
                InternationalName = internationalName
            };
        }
    }

    /// <summary>
    ///     Represents the parsed values of a line from the ElectionTypes.csv file
    /// </summary>
    public class ElectionTypeFormat : ICsvFormat<ElectionTypeFormat>
    {
        public string ElectionTypeCode;
        public string InternationalName;

        /// <summary>
        ///     Parses a line following the ElectionTypeFormat and returns an ElectionTypeFormat object
        /// </summary>
        /// <param name="line">The line to parse</param>
        /// <param name="parser">The field parser to use</param>
        /// <returns>An ElectionTypeFormat object containing the parsed values</returns>
        public ElectionTypeFormat Parse(string line, FieldParser parser)
        {
            string[] fields = parser.ParseLength(line, 2);
            string electionTypeCode = parser.ParseString(fields[0], "Election type code", 2, 5);
            string internationalName = parser.ParseString(fields[1], "International name", 3, 30);

            return new ElectionTypeFormat
            {
                ElectionTypeCode = electionTypeCode,
                InternationalName = internationalName
            };
        }
    }

    /// <summary>
    ///     Represents the parsed values of a line from the CountyData.csv file
    /// </summary>
    public class CountyDataFormat : ICsvFormat<CountyDataFormat>
    {
        public double Areal;
        public string County;
        public int Population;
        public int Seats;
        public int Year;

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
            string county = parser.ParseString(fields[1], "County", 3, 20);
            double areal = parser.ParseDouble(fields[2], "Areal");
            int population = parser.ParseInt(fields[3], "Population");
            int seats = parser.ParseInt(fields[4], "Seats");

            return new CountyDataFormat
            {
                Year = year,
                County = county,
                Areal = areal,
                Population = population,
                Seats = seats
            };
        }
    }
}