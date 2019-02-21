namespace LaviniaApi.Utilities
{
    /// <summary>
    ///     A class for parsing the fields from a string array.
    /// </summary>
    public class FieldParser
    {
        /// <summary>
        ///     The constructor takes the path of the file to be parsed, used to provide meaningful exception information.
        /// </summary>
        /// <param name="file">Path of the file being parsed</param>
        /// <param name="separator">The separator used to separate the fields in the file</param>
        public FieldParser(string file, string separator)
        {
            File = file;
            Separator = separator;
        }

        private string File { get; }
        private string Separator { get; }
        private string Line { get; set; }

        /// <summary>
        ///     Verifies that the string array matches the expected length.
        /// </summary>
        /// <param name="line">The line to be parsed</param>
        /// <param name="expectedLength">The number of fields epected</param>
        /// <returns>A string array containing the extracted fields</returns>
        /// <exception cref="CsvFileFormatException">Is thrown if the line has incorrect length</exception>
        public string[] ParseLength(string line, int expectedLength)
        {
            string[] fields = line.Split(Separator);
            if (fields.Length != expectedLength)
            {
                throw new CsvFileFormatException(
                    $"The line has incorrect length, expected: {expectedLength}, got: {fields.Length}.", File, line);
            }

            Line = line;
            return fields;
        }
        
        /// API V1
        /// <summary>
        ///     Parses a string to any of the known algorithms.
        /// </summary>
        /// <param name="value">String representation of the algorith</param>
        /// <param name="field">Name of the field</param>
        /// <returns>The Algorithm parsed from the string</returns>
        /// <exception cref="CsvFileFormatException">Is thrown if the string does not match any known algorithm</exception>
        public Algorithm ParseAlgorithm(string value, string field)
        {
            if (!AlgorithmUtilities.TryParse(value, out Algorithm algorithm))
            {
                throw new CsvFileFormatException($"The {field} \"{value}\" is not a valid algorithm name.", File, Line);
            }

            return algorithm;
        }

        /// API V2
        /// <summary>
        ///     Parses a string to any of the known algorithms.
        /// </summary>
        /// <param name="value">String representation of the algorith</param>
        /// <param name="field">Name of the field</param>
        /// <returns>The Algorithm parsed from the string</returns>
        /// <exception cref="CsvFileFormatException">Is thrown if the string does not match any known algorithm</exception>
        public string ParseAlgorithmToString(string value, string field)
        {
            if (!AlgorithmUtilities.TryParseToString(value, out string algorithm))
            {
                throw new CsvFileFormatException($"The {field} \"{value}\" is not a valid algorithm name.", File, Line);
            }

            return algorithm;
        }

        /// <summary>
        ///     Parses a string to an integer value.
        /// </summary>
        /// <param name="value">String represenation of the integer</param>
        /// <param name="field">Name of the field</param>
        /// <returns>The integer parsed from the string</returns>
        /// <exception cref="CsvFileFormatException">Is thrown if the string cannot be parsed to an integer</exception>
        public int ParseInt(string value, string field)
        {
            if (!int.TryParse(value, out int result))
            {
                throw new CsvFileFormatException($"The {field} \"{value}\" is not a valid integer.", File, Line);
            }

            return result;
        }

        /// <summary>
        ///     Prases a string to a duble value.
        /// </summary>
        /// <param name="value">String representation of the double</param>
        /// <param name="field">Name of the field</param>
        /// <returns>The double parsed from the string</returns>
        /// <exception cref="CsvFileFormatException">Is thrown if the string cannot be parsed to a double</exception>
        public double ParseDouble(string value, string field)
        {
            value = value.Replace(",", ".");

            if (!double.TryParse(value, out double result))
            {
                throw new CsvFileFormatException($"The {field} \"{value}\" is not a valid double.", File, Line);
            }

            return result;
        }

        /// <summary>
        ///     Verifies that a string is within certain length restrictions.
        ///     Checks if the string length is greater or equal to the minLength (inclusive),
        ///     and that it is less than the maxLength (exclusive).
        ///     Set the maxLength to -1 to allow unrestricted length.
        /// </summary>
        /// <param name="value">String to be checked</param>
        /// <param name="field">Name of the field</param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public string ParseString(string value, string field, int minLength, int maxLength)
        {
            if (value.Length < minLength)
            {
                throw new CsvFileFormatException(
                    $"The {field} \"{value}\" is shorter than the minimum length. Expected a value >= {minLength}, got: {value.Length}",
                    File, Line);
            }

            if (maxLength != -1 && value.Length > maxLength)
            {
                throw new CsvFileFormatException(
                    $"The field {field} \"{value}\" is longer than the maximum length. Expected a value < {maxLength}, got: {value.Length}",
                    File, Line);
            }

            return value;
        }
    }
}