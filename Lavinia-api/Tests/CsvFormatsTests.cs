using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Models;
using LaviniaApi.Utilities;
using Xunit;

namespace LaviniaApi.Tests
{
    public class CsvFormatsTests
    {
        // Tests a normal input for ElectionFormat
        [Fact]
        public void ParseElectionFormatTest()
        {
            ElectionFormat election = new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                new FieldParser("TEST", ";"));
            Assert.Equal(1.8, election.AreaFactor);
            Assert.Equal(1.4, election.FirstDivisor);
            Assert.Equal(19, election.LevelingSeats);
            Assert.Equal(150, election.Seats);
            Assert.Equal(4.0, election.Threshold);
            Assert.Equal(2017, election.Year);
            Assert.Equal(Algorithm.ModifiedSainteLagues, election.Algorithm);
            Assert.Equal("Sainte Laguës (modified)", election.AlgorithmString);
        }
        
        // Tests different algorithms for ElectionFormat
        [Fact]
        public void ParseElectionFormatAlgorithmTest()
        {
            FieldParser tmpParser = new FieldParser("TEST", ";");

            ElectionFormat election = new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
                tmpParser
            );
            Assert.Equal(Algorithm.ModifiedSainteLagues, election.Algorithm);
            Assert.Equal("Sainte Laguës (modified)", election.AlgorithmString);

            election = new ElectionFormat().Parse("2017;Sainte Laguës;1.4;4.0;1.8;150;19",
                tmpParser);
            Assert.Equal(Algorithm.SainteLagues, election.Algorithm);
            Assert.Equal("Sainte Laguës", election.AlgorithmString);

            election = new ElectionFormat().Parse("2017;d'Hondt;1.4;4.0;1.8;150;19",
                tmpParser);
            Assert.Equal(Algorithm.DHondt, election.Algorithm);
            Assert.Equal("d'Hondt", election.AlgorithmString);

            Assert.Throws<CsvFileFormatException>(() => new ElectionFormat().Parse("2017;MALFORMED INPUT;1.4;4.0;1.8;150;19",
                    tmpParser));
    }

        // Tests a normal input to ResultFormat
        [Fact]
        public void ParseResultFormatTest()
        {
            ResultFormat result = new ResultFormat().Parse("01;Østfold;;;;;A;Arbeiderpartiet;32,1;216293;12947;38598;51545;-2,9;-8,1;3;0;",
                new FieldParser("TEST", ";"));
            Assert.Equal(51545, result.AntallStemmerTotalt);
            Assert.Equal(32.1, result.OppslutningProsentvis);
            Assert.Equal("Østfold", result.Fylkenavn);
            Assert.Equal("A", result.Partikode);
            Assert.Equal("Arbeiderpartiet", result.Partinavn);
        }

        // Tests a normal input to CountryFormat
        [Fact]
        public void ParseCountryFormatTest()
        {
            CountryFormat country = new CountryFormat().Parse("NO;Norway",
                new FieldParser("TEST", ";"));
            Assert.Equal("NO", country.CountryCode);
            Assert.Equal("Norway", country.InternationalName);
        }

        // Tests a normal input to ElectionTypeFormat
        [Fact]
        public void ParseElectionTypeFormatTest()
        {
            ElectionTypeFormat electionType = new ElectionTypeFormat().Parse("PE;Parliamentary Election",
                new FieldParser("TEST", ";"));
            Assert.Equal("PE", electionType.ElectionTypeCode);
            Assert.Equal("Parliamentary Election", electionType.InternationalName);
        }

        // Tests a normal input to CountyDataFormat
        [Fact]
        public void ParseCountyDataFormatTest()
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
