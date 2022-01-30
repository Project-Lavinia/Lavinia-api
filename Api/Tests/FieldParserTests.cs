using Lavinia.Api.Utilities;
using Xunit;

namespace Lavinia.Api.Tests
{
    public static class FieldParserTests
    {
        /// <summary>
        /// Tests a normal input for ParseLength
        /// </summary>
        [Fact]
        public static void ParseLengthTest()
        {
            FieldParser fieldParser = new FieldParser("TEST", ";");

            Assert.Collection(fieldParser.ParseLength("A;B;C;", 4),
                field => Assert.Contains("A", field),
                field => Assert.Contains("B", field),
                field => Assert.Contains("C", field),
                field => Assert.Contains("", field));
        }

        /// <summary>
        /// Tests erroneous input for ParseLength
        /// </summary>
        [Fact]
        public static void ParseLengthErroneousTest()
        {
            FieldParser fieldParser = new FieldParser("TEST", ";");

            // Testing 1 field input when expected length is 2
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseLength("", 2));

            // Testing 2 field input when expected length is 1
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseLength(";", 1));
        }

        /// <summary>
        /// Tests normal inputs for ParseAlgorithmToString
        /// </summary>
        [Fact]
        public static void ParseAlgorithmToStringTest()
        {
            FieldParser fieldParser = new FieldParser("TEST", ";");

            Assert.Equal("Sainte Laguës (modified)", fieldParser.ParseAlgorithmToString("sainte Laguës (modified)", "TEST"));
            Assert.Equal("Sainte Laguës", fieldParser.ParseAlgorithmToString("Sainte laguës", "TEST"));
            Assert.Equal("d'Hondt", fieldParser.ParseAlgorithmToString("D'hondt", "TEST"));
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseAlgorithmToString("FAKE ALGORITHM", "Test"));
        }

        /// <summary>
        /// Tests normal inputs for ParseInt
        /// </summary>
        [Fact]
        public static void ParseIntTest()
        {
            FieldParser fieldParser = new FieldParser("TEST", ";");

            // Normal multi-character integer
            Assert.Equal(123, fieldParser.ParseInt("123", "TEST"));

            // Negative integer
            Assert.Equal(-1, fieldParser.ParseInt("-1", "TEST"));

            // Empty input
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseInt("", "Test"));

            // Normal double
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseInt("1.2", "Test"));

            // Negative double
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseInt("-1.2", "Test"));

            // Norwegian normal double
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseInt("1,2", "Test"));

            // Norwegian negative double
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseInt("-1,2", "Test"));

            // Not a number
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseInt("2-3", "Test"));
        }

        /// <summary>
        /// Tests normal inputs for ParseDouble
        /// </summary>
        [Fact]
        public static void ParseDoubleTest()
        {
            FieldParser fieldParser = new FieldParser("TEST", ";");

            // Normal double
            Assert.Equal(0.123, fieldParser.ParseDouble("0.123", "TEST"));

            // Negative double
            Assert.Equal(-0.123, fieldParser.ParseDouble("-0.123", "TEST"));

            // Norwegian normal double
            Assert.Equal(0.123, fieldParser.ParseDouble("0,123", "TEST"));

            // Norwegian negative double
            Assert.Equal(-0.123, fieldParser.ParseDouble("-0,123", "TEST"));

            // Integer to double
            Assert.Equal(1.0, fieldParser.ParseDouble("1", "TEST"));

            // Empty input
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseDouble("", "Test"));

            // Not a double
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseDouble("2-3", "Test"));
        }

        /// <summary>
        /// Tests normal inputs for ParseString
        /// </summary>
        [Fact]
        public static void ParseStringTest()
        {
            FieldParser fieldParser = new FieldParser("TEST", ";");

            // Within bounds
            Assert.Equal("Hey", fieldParser.ParseString("Hey", "TEST", 0, 5));

            // Upper bound
            Assert.Equal("Hello", fieldParser.ParseString("Hello", "TEST", 0, 5));

            // Lower bound
            Assert.Equal("", fieldParser.ParseString("", "TEST", 0, 5));

            // Lower bound -1
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseString("", "TEST", 1, 5));

            // Upper bound +1
            Assert.Throws<CsvFileFormatException>(() => fieldParser.ParseString("Hey", "TEST", 0, 2));
        }
    }
}