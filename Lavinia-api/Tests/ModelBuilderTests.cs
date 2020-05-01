using System.Collections.Generic;
using System.Linq;
using LaviniaApi.Models;
using LaviniaApi.Utilities;
using Xunit;

namespace LaviniaApi.Tests
{
    public static class ModelBuilderTests
    {
        private static readonly FieldParser FieldParser = new FieldParser("TEST", ";");

        // Tests a normal input for BuildDistrictMetrics
        [Fact]
        public static void BuildDistrictMetricsTest()
        {
            List<CountyDataFormat> countyData = new List<CountyDataFormat> { new CountyDataFormat().Parse("2017;Akershus;4918;556254;16", FieldParser) };
            IEnumerable<DistrictMetrics> dmList = ModelBuilder.BuildDistrictMetrics(countyData);
            DistrictMetrics dm = dmList.First();

            Assert.Equal(16, dm.Seats);
            Assert.Equal(2017, dm.ElectionYear);
            Assert.Equal(4918, dm.Area);
            Assert.Equal(556254, dm.Population);
        }

        // Tests a normal input for BuildElectionParameters
        [Fact]
        public static void BuildElectionParametersTest()
        {
            List<ElectionFormat> election = new List<ElectionFormat>
            {
                new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                    FieldParser)

            };
            List<CountyDataFormat> countyData = new List<CountyDataFormat> { new CountyDataFormat().Parse("2017;Akershus;4918;556254;16", FieldParser) };
            IEnumerable<DistrictMetrics> dmList = ModelBuilder.BuildDistrictMetrics(countyData);
            IEnumerable<ElectionParameters> epList = ModelBuilder.BuildElectionParameters(election, "PE", dmList);

            ElectionParameters ep = epList.First();
            Assert.Equal(1.8, ep.AreaFactor);
            Assert.Equal(2017, ep.ElectionYear);
            Assert.Equal(19, ep.LevelingSeats);
            Assert.Equal(4.0, ep.Threshold);
            Assert.Equal(0, ep.TotalVotes);
            Assert.Equal("PE", ep.ElectionType);

            Assert.Equal("Sainte Laguës (modified)", ep.Algorithm.Algorithm);
            ListElement<double> ap = ep.Algorithm.Parameters.First();
            Assert.Equal("First Divisor", ap.Key);
            Assert.Equal(1.4, ap.Value);
            ListElement<int>[] seatList = ep.DistrictSeats.ToArray();
            Assert.Equal("SUM", seatList[0].Key);
            Assert.Equal(150, seatList[0].Value);
        }

        // Tests different DistrictSeat input for BuildElectionParameters
        [Fact]
        public static void BuildElectionParametersDistrictSeatTest()
        {
            // Tests behaviour for when DistrictSeats are computed
            List<ElectionFormat> election = new List<ElectionFormat>
            {
                new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                    FieldParser)
            };
            List<CountyDataFormat> countyData = new List<CountyDataFormat> { new CountyDataFormat().Parse("2017;Akershus;4918;556254;16", FieldParser) };
            IEnumerable<DistrictMetrics> dmList = ModelBuilder.BuildDistrictMetrics(countyData);
            IEnumerable<ElectionParameters> epList = ModelBuilder.BuildElectionParameters(election, "PE", dmList);

            ElectionParameters ep = epList.First();
            ListElement<int>[] seatList = ep.DistrictSeats.ToArray();
            Assert.Equal("SUM", seatList[0].Key);
            Assert.Equal(150, seatList[0].Value);


            // Tests behaviour for when DistrictSeats are determined before the election
            election = new List<ElectionFormat>
            {
                new ElectionFormat().Parse("1977;Sainte Laguës (modified);1.4;0.0;-1;155;0",
                    FieldParser)
            };
            countyData = new List<CountyDataFormat>
            {
                new CountyDataFormat().Parse("1977;Akershus;0;332561;10", FieldParser),
                new CountyDataFormat().Parse("1977;Aust-Agder;0;81734;4", FieldParser)
            };
            dmList = ModelBuilder.BuildDistrictMetrics(countyData);
            epList = ModelBuilder.BuildElectionParameters(election, "PE", dmList);

            ep = epList.First();
            seatList = ep.DistrictSeats.ToArray();
            Assert.Equal("SUM", seatList[0].Key);
            Assert.Equal(155, seatList[0].Value);
            Assert.Equal("Akershus", seatList[1].Key);
            Assert.Equal(10, seatList[1].Value);
            Assert.Equal("Aust-Agder", seatList[2].Key);
            Assert.Equal(4, seatList[2].Value);
        }

        // Tests a normal input for BuildElectionParameters
        [Fact]
        public static void BuildElectionParametersTestV3()
        {
            List<ElectionFormat> election = new List<ElectionFormat>
            {
                new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                    FieldParser)
            };
            IEnumerable<ElectionParametersV3> epList = ModelBuilder.BuildElectionParametersV3(election, "PE");

            ElectionParametersV3 ep = epList.First();
            Assert.Equal(1.8, ep.AreaFactor);
            Assert.Equal(2017, ep.ElectionYear);
            Assert.Equal(19, ep.LevelingSeats);
            Assert.Equal(4.0, ep.Threshold);
            Assert.Equal(0, ep.TotalVotes);
            Assert.Equal("PE", ep.ElectionType);

            Assert.Equal("Sainte Laguës (modified)", ep.Algorithm.Algorithm);
            ListElement<double> ap = ep.Algorithm.Parameters.First();
            Assert.Equal("First Divisor", ap.Key);
            Assert.Equal(1.4, ap.Value);
            Assert.Equal(150, ep.DistrictSeats);
        }

        // Tests different DistrictSeat input for BuildElectionParameters
        [Fact]
        public static void BuildElectionParametersDistrictSeatTestV3()
        {
            // Tests behaviour for when DistrictSeats are computed
            List<ElectionFormat> election = new List<ElectionFormat>
            {
                new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                    FieldParser)
            };
            IEnumerable<ElectionParametersV3> epList = ModelBuilder.BuildElectionParametersV3(election, "PE");

            ElectionParametersV3 ep = epList.First();
            Assert.Equal(150, ep.DistrictSeats);


            // Tests behaviour for when DistrictSeats are determined before the election
            election = new List<ElectionFormat>
            {
                new ElectionFormat().Parse("1977;Sainte Laguës (modified);1.4;0.0;-1;155;0",
                    FieldParser)
            };
            epList = ModelBuilder.BuildElectionParametersV3(election, "PE");

            ep = epList.First();
            Assert.Equal(155, ep.DistrictSeats);
        }

        // Tests a normal input for BuildAlgorithmParameters
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

        // Tests varying algorithm input for BuildAlgorithmParameters
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

        // Tests a normal input for BuildPartyVotes
        [Fact]
        public static void BuildPartyVotesTest()
        {
            List<ResultFormat> resultFormat = new List<ResultFormat> { new ResultFormat().Parse("01;Østfold;;;;;A;Arbeiderpartiet;32,1;216293;12947;38598;51545;-2,9;-8,1;3;0;", FieldParser) };
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