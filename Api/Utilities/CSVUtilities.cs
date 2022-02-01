using System;
using System.Collections.Generic;
using System.IO;

namespace Lavinia.Api.Utilities
{
     sealed class CsvFileFormatException : FormatException
    {
        public CsvFileFormatException(string exText, string path, string line) : base(
            exText + "\nPath: " + path + "\nLine: " + line)
        {
        }

        public CsvFileFormatException()
        {
        }

        public CsvFileFormatException(string? message) : base(message)
        {
        }

        public CsvFileFormatException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }

    public static class CsvUtilities
    {
        public static List<T> CsvToList<T>(string filePath) where T : ICsvFormat<T>, new()
        {
            List<T> objects = new();
            StreamReader file = new(filePath);
            FieldParser parser = new(filePath, ";");
            file.ReadLine(); // Skip header string
            string? currentLine;
            while ((currentLine = file.ReadLine()) != null)
            {
                if (!currentLine.Contains('#'))
                {
                    objects.Add(new T().Parse(currentLine, parser));
                }
            }
            file.Dispose();
            return objects;
        }
    }
}