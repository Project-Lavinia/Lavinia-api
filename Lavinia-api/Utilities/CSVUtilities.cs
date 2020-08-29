using System;
using System.Collections.Generic;
using System.IO;

namespace LaviniaApi.Utilities
{
    public class CsvFileFormatException : FormatException
    {
        public CsvFileFormatException(string exText, string path, string line) : base(
            exText + "\nPath: " + path + "\nLine: " + line)
        {
        }
    }

    public static class CsvUtilities
    {
        public static List<T> CsvToList<T>(string filePath) where T : ICsvFormat<T>, new()
        {
            List<T> objects = new List<T>();
            StreamReader file = new StreamReader(filePath);
            FieldParser parser = new FieldParser(filePath, ";");
            file.ReadLine(); // Skip header string
            string currentLine;
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