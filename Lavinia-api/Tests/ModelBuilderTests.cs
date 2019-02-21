using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LaviniaApi.Models;
using LaviniaApi.Utilities;
using Xunit;

namespace LaviniaApi.Tests
{
    public class ModelBuilderTests
    {
        private static readonly FieldParser _fieldParser = new FieldParser("TEST", ";");

        private static readonly ElectionFormat _election = new ElectionFormat().Parse("2017;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
            _fieldParser);

        private static readonly CountyDataFormat _countyData = new CountyDataFormat().Parse("2017;Akershus;4918;556254;16", _fieldParser);
        
        // Tests a normal input for BuildAlgorithmParameters
        [Fact]
        public void BuildAlgorithmParametersTest()
        {
            AlgorithmParameters ap = ModelBuilder.BuildAlgorithmParameters(_election);
            Assert.Equal(AlgorithmUtilities.ModifiedSainteLagues, ap.Algorithm);
            Assert.Equal(1.4, ap.Parameters[0].Value);
        }
    }
}