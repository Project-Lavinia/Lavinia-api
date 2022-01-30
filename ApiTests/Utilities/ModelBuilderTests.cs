using System.Collections.Generic;
using System.Linq;

using Lavinia.Api.Models;

using Xunit;

namespace Lavinia.Api.Utilities.Tests
{
    public static class ModelBuilderTests
    {
        private static readonly FieldParser FieldParser = new("TEST", ";");

        private static readonly IReadOnlyDictionary<int, int> YearTotalVotesMap = new Dictionary<int, int>()
            {
                { 2017, 0 },
                { 1977, 0 }
            };

        /// <summary>
        /// Tests a normal input for BuildDistrictMetrics
        /// </summary>
        [Fact]
        public static void BuildDistrictMetricsTest()
        {
            List<CountyDataFormat> countyData = new() { new CountyDataFormat().Parse("2017;Akershus;4918;556254;16", FieldParser) };
            IEnumerable<DistrictMetrics> dmList = ModelBuilder.BuildDistrictMetrics(countyData);
            DistrictMetrics dm = dmList.First();

            Assert.Equal(16, dm.Seats);
            Assert.Equal(2017, dm.ElectionYear);
            Assert.Equal(4918, dm.Area);
            Assert.Equal(556254, dm.Population);
        }

        /// <summary>
        /// Tests a normal input for BuildElectionParameters
        /// </summary>
        [Fact]
        public static void BuildElectionParametersTest()
        {
            List<ElectionFormat> election = new()
            {
                new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                    FieldParser)
            };

            IReadOnlyDictionary<int, int> yearTotalVotesMap = new Dictionary<int, int>()
            {
                {2017, 0 }
            };

            IEnumerable<ElectionParameters> epList = ModelBuilder.BuildElectionParameters(election, "PE", yearTotalVotesMap);

            ElectionParameters ep = epList.First();
            Assert.Equal(1.8, ep.AreaFactor);
            Assert.Equal(2017, ep.ElectionYear);
            Assert.Equal(19, ep.LevelingSeats);
            Assert.Equal(4.0, ep.Threshold);
            Assert.Equal(0, ep.TotalVotes);
            Assert.Equal("PE", ep.ElectionType);

            Assert.Equal("Sainte Laguës (modified)", ep.Algorithm.Algorithm);
            ListElement<double> ap = ep.Algorithm.Parameters[0];
            Assert.Equal("First Divisor", ap.Key);
            Assert.Equal(1.4, ap.Value);
            Assert.Equal(150, ep.DistrictSeats);
        }

        /// <summary>
        /// Tests different DistrictSeat input for BuildElectionParameters
        /// </summary>
        [Theory]
        [InlineData("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19", 150)]
        [InlineData("1977;Sainte Laguës (modified);1.4;0.0;-1;155;0", 155)]
        public static void BuildElectionParametersDistrictSeatTest(string line, int expected)
        {
            // Tests behaviour for when DistrictSeats are computed
            List<ElectionFormat> election = new()
            {
                new ElectionFormat().Parse(line,
                    FieldParser)
            };
            IEnumerable<ElectionParameters> epList = ModelBuilder.BuildElectionParameters(election, "PE", YearTotalVotesMap);

            ElectionParameters ep = epList.First();
            Assert.Equal(expected, ep.DistrictSeats);
        }

        /// <summary>
        /// Tests a normal input for BuildAlgorithmParameters
        /// </summary>
        [Fact]
        public static void BuildAlgorithmParametersTest()
        {
            ElectionFormat election = new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                FieldParser);
            AlgorithmParameters ap = ModelBuilder.BuildAlgorithmParameters(election);
            Assert.Equal(AlgorithmUtilities.ModifiedSainteLagues, ap.Algorithm);
            Assert.Equal("First Divisor", ap.Parameters[0].Key);
            Assert.Equal(1.4, ap.Parameters[0].Value);
        }

        /// <summary>
        /// Tests varying algorithm input for BuildAlgorithmParameters
        /// </summary>
        [Fact]
        public static void BuildAlgorithmParametersAlgorithmTest()
        {
            ElectionFormat election = new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                FieldParser);
            AlgorithmParameters ap = ModelBuilder.BuildAlgorithmParameters(election);
            Assert.Equal(AlgorithmUtilities.ModifiedSainteLagues, ap.Algorithm);
            Assert.Equal("First Divisor", ap.Parameters[0].Key);
            Assert.Equal(1.4, ap.Parameters[0].Value);

            election = new ElectionFormat().Parse("2017;Sainte Laguës;1.0;4.0;1.8;150;19",
                FieldParser);
            ap = ModelBuilder.BuildAlgorithmParameters(election);
            Assert.Equal(AlgorithmUtilities.SainteLagues, ap.Algorithm);
            Assert.Empty(ap.Parameters);

            election = new ElectionFormat().Parse("2017;d'Hondt;1.0;4.0;1.8;150;19",
                FieldParser);
            ap = ModelBuilder.BuildAlgorithmParameters(election);
            Assert.Equal(AlgorithmUtilities.DHondt, ap.Algorithm);
            Assert.Empty(ap.Parameters);
        }

        /// <summary>
        /// Tests a normal input for BuildPartyVotes
        /// </summary>
        [Fact]
        public static void BuildPartyVotesTest()
        {
            List<ResultFormat> resultFormat = new() { new ResultFormat().Parse("01;Østfold;;;;;A;Arbeiderpartiet;32,1;216293;12947;38598;51545;-2,9;-8,1;3;0;", FieldParser) };
            IEnumerable<PartyVotes> pvList = ModelBuilder.BuildPartyVotes(resultFormat, "PE", 2017);
            PartyVotes pv = pvList.First();

            Assert.Equal("Østfold", pv.District);
            Assert.Equal("PE", pv.ElectionType);
            Assert.Equal(2017, pv.ElectionYear);
            Assert.Equal("A", pv.Party);
            Assert.Equal(51545, pv.Votes);
        }
    }
}