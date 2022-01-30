using Xunit;

namespace Lavinia.Api.Utilities.Tests
{
    public static class CsvFormatsTests
    {
        /// <summary>
        /// Tests a normal input for ElectionFormat
        /// </summary>
        [Fact]
        public static void ParseElectionFormatTest()
        {
            ElectionFormat election = new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                new FieldParser("TEST", ";"));
            Assert.Equal(1.8, election.AreaFactor);
            Assert.Equal(1.4, election.FirstDivisor);
            Assert.Equal(19, election.LevelingSeats);
            Assert.Equal(150, election.Seats);
            Assert.Equal(4.0, election.Threshold);
            Assert.Equal(2017, election.Year);
            Assert.Equal("Sainte Laguës (modified)", election.AlgorithmString);
        }

        /// <summary>
        /// Tests a normal input to ResultFormat
        /// </summary>
        [Fact]
        public static void ParseResultFormatTest()
        {
            ResultFormat result = new ResultFormat().Parse("01;Østfold;;;;;A;Arbeiderpartiet;32,1;216293;12947;38598;51545;-2,9;-8,1;3;0;",
                new FieldParser("TEST", ";"));
            Assert.Equal(51545, result.AntallStemmerTotalt);
            Assert.Equal(32.1, result.OppslutningProsentvis);
            Assert.Equal("Østfold", result.Fylkenavn);
            Assert.Equal("A", result.Partikode);
            Assert.Equal("Arbeiderpartiet", result.Partinavn);
        }

        /// <summary>
        /// Tests a normal input to CountryFormat
        /// </summary>
        [Fact]
        public static void ParseCountryFormatTest()
        {
            CountryFormat country = new CountryFormat().Parse("NO;Norway",
                new FieldParser("TEST", ";"));
            Assert.Equal("NO", country.CountryCode);
            Assert.Equal("Norway", country.InternationalName);
        }

        /// <summary>
        /// Tests a normal input to ElectionTypeFormat
        /// </summary>
        [Fact]
        public static void ParseElectionTypeFormatTest()
        {
            ElectionTypeFormat electionType = new ElectionTypeFormat().Parse("PE;Parliamentary Election",
                new FieldParser("TEST", ";"));
            Assert.Equal("PE", electionType.ElectionTypeCode);
            Assert.Equal("Parliamentary Election", electionType.InternationalName);
        }

        /// <summary>
        /// Tests a normal input to CountyDataFormat
        /// </summary>
        [Fact]
        public static void ParseCountyDataFormatTest()
        {
            CountyDataFormat countyData = new CountyDataFormat().Parse("2017;Akershus;4918;556254;16",
                new FieldParser("TEST", ";"));
            Assert.Equal(16, countyData.Seats);
            Assert.Equal(4918, countyData.Area);
            Assert.Equal(556254, countyData.Population);
            Assert.Equal(2017, countyData.Year);
            Assert.Equal("Akershus", countyData.County);
        }
    }
}