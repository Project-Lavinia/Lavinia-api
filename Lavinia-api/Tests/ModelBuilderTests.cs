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
        private readonly ElectionFormat _election = new ElectionFormat().Parse("2013;Sainte Laguës (modified);1.4;4.0;1.8;150;19",
            new FieldParser("TEST", ";"));

        [Fact]
        public void BuildAlgorithmParametersTest()
        {
            AlgorithmParameters ap = ModelBuilder.BuildAlgorithmParameters(_election);
            Assert.Equal(AlgorithmUtilities.ModifiedSainteLagues, ap.Algorithm);
            //Assert.Equal(1.4, ap.Parameters["FirstDivisor"]);
        }
    }
}