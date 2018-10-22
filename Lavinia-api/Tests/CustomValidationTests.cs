using System;
using System.Collections.Generic;
using LaviniaApi.Data;
using LaviniaApi.Models;
using Xunit;

namespace LaviniaApi.Tests
{
    public static class CustomValidationTests
    {
        [Theory]
        [InlineData(true, "Norway", "NO")]
        [InlineData(false, null, "NO")]
        [InlineData(false, "Norway", null)]
        public static void ValidateCountryMissingDataTest(bool useNull, string internationalName, string shortName)
        {
            // Testing attempt on adding model with missing data
            Country country = new Country {InternationalName = internationalName, CountryCode = shortName};
            if (useNull)
            {
                country = null;
            }

            Assert.Throws<ArgumentException>(() => CustomValidation.ValidateCountry(country, new HashSet<int>()));
        }

        private static Country GetCountry(int useCountry)
        {
            switch (useCountry)
            {
                case 1:
                    return new Country {InternationalName = "Norway", CountryCode = "NO"};
                case 0:
                    return new Country {InternationalName = "Norway"};
                default:
                    return null;
            }
        }

        [Fact]
        public static void ValidateCountryTest()
        {
            Country country = GetCountry(1);
            CustomValidation.ValidateCountry(country, new HashSet<int>());
        }
    }
}